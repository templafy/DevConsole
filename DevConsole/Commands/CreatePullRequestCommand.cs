using DevConsole.Infrastructure;
using DevConsole.Infrastructure.Commands;
using DevConsole.Infrastructure.Models;
using DevConsole.Infrastructure.Output;
using DevConsole.Infrastructure.Services;
using DevConsole.Infrastructure.Services.AzureDevOps;
using System;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;

namespace DevConsole.Commands;

public sealed class CreatePullRequestCommand : DevConsoleCommand
{
    private readonly AzureDevOpsService _azureDevOpsService;
    private readonly DevConsoleConfig _devConsoleConfig;
    private readonly VersionBumper _versionBumper;

    public CreatePullRequestCommand(AzureDevOpsService azureDevOpsService, VersionBumper versionBumper, DevConsoleConfig devConsoleConfig)
        : base("create-pull-request", "Create an Azure DevOps pull request based on the current branch and directory")
    {
        _azureDevOpsService = azureDevOpsService;
        _versionBumper = versionBumper;
        _devConsoleConfig = devConsoleConfig;

        AddOption(new Option<bool>(new[] { "-a", "--auto-create" }, "Automatically create pull request"));
        AddOption(new Option<bool>(new[] { "-d", "--draft" }, "Create pull request in draft mode"));
        AddOption(new Option<bool>(new[] { "-c", "--set-auto-complete" },
            "Sets the pull request to auto-complete and approves it by you. Used with --auto-create"));

        AddOption(new Option<string>(new[] { "-t", "--title" },
            "Define title for automatically created pull request, used with --auto-create"));

        AddOption(new Option<bool>(new[] { "-s", "--stop-open-browser" },
            "Prevents open browser after pull request is created. Used with --auto-create"));

        AddAlias("cpr");

        Handler = CommandHandler.Create<bool, bool, bool, string, bool>(DoCommand);
    }

    private void DoCommand(bool autoCreate, bool draft, bool setAutoComplete, string title, bool stopOpenBrowser)
    {
        _azureDevOpsService.EnsureAzCliVersions();

        if (autoCreate)
        {
            AutoCreatePullRequest(title, stopOpenBrowser, draft, setAutoComplete);
        }
        else
        {
            ManuallyCreatePullRequest();
        }

        MoveWorkItemToCodeReview();
    }

    private void MoveWorkItemToCodeReview()
    {
        var branchName = GetBranchName();
        var workItemId = GetWorkItemIdFromBranchName(branchName);

        if (workItemId is null)
        {
            return;
        }

        Run($"az boards work-item update --id {workItemId} -f Microsoft.VSTS.Common.ResolvedReason=\"Fixed\" System.State=\"Code review\"", outputHandler: SuppressOutputHandler.Instance);
    }

    private void AutoCreatePullRequest(string? title, bool stopOpenBrowser, bool draft, bool setAutoComplete)
    {
        var branchName = GetBranchName();
        var workItemId = GetWorkItemIdFromBranchName(branchName);

        if (workItemId == null)
        {
            throw new UserActionException("Work item not found");
        }

        if (string.IsNullOrWhiteSpace(title))
        {
            title = GetWorkItem(workItemId.Value).Output?.Fields.Title;

            if (string.IsNullOrWhiteSpace(title))
            {
                throw new UserActionException("You must define a title");
            }
        }

        var pullRequest = GetJsonOutput<PullRequest>("az repos pr create " +
                                                     $"--title \"{title}\" " +
                                                     $"--draft {draft} " +
                                                     $"--auto-complete {setAutoComplete} " +
                                                     "--delete-source-branch " +
                                                     "--transition-work-items " +
                                                     $"--work-items {workItemId} " +
                                                     $"{(!stopOpenBrowser ? "--open" : string.Empty)}");

        if (pullRequest.Output != null)
        {
            ColorConsole.WriteLine($"Pull request {pullRequest.Output.PullRequestId} created", ConsoleColor.Green);

            if (setAutoComplete)
            {
                Run($"az repos pr set-vote --id {pullRequest.Output.PullRequestId} --vote approve",
                    outputHandler: SuppressOutputHandler.Instance);

                ColorConsole.WriteLine(
                    $"Pull request {pullRequest.Output.PullRequestId} automatically approved by you.",
                    ConsoleColor.Green);

                Run($"az repos pr update --id {pullRequest.Output.PullRequestId} " +
                    $"--merge-commit-message \"Merged PR {pullRequest.Output.PullRequestId}: {title}. Related work items: #{workItemId}\"",
                    outputHandler: SuppressOutputHandler.Instance);

                ColorConsole.WriteLine(
                    $"Pull request {pullRequest.Output.PullRequestId} is set with merge commit message.",
                    ConsoleColor.Green);
            }
        }
        else
        {
            ColorConsole.Write($"Failed to create pull request: {pullRequest.Error}", ConsoleColor.Red);
        }
    }

    private void ManuallyCreatePullRequest()
    {
        var repositoryName = GetRepositoryName();
        var branchName = GetBranchName();

        _versionBumper.PromptForVersionBump();

        var pullRequestUrl = $"https://dev.azure.com/{_devConsoleConfig.AzureDevOpsOrganizationName}/{_devConsoleConfig.AzureDevOpsProjectName}/_git/{repositoryName}/pullrequestcreate?sourceRef={UrlEncoder.Default.Encode(branchName)}";

        OpenBrowser(pullRequestUrl);
    }

    private string GetRepositoryName()
    {
        var originUrlResult = GetOutput("git remote get-url origin");
        return originUrlResult.Output.Trim().Split("/").Last();
    }

    private string GetBranchName()
    {
        var branchNameResult = GetOutput("git symbolic-ref --short HEAD");
        return branchNameResult.Output.Trim();
    }

    private long? GetWorkItemIdFromBranchName(string branchName)
    {
        var workItemMatch = Regex.Match(branchName, "feature/(\\d+)-");
        if (workItemMatch.Success)
        {
            return long.Parse(workItemMatch.Groups[1].Value);
        }

        return null;
    }
}