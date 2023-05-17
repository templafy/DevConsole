using DevConsole.Infrastructure;
using DevConsole.Infrastructure.Commands;
using DevConsole.Infrastructure.Models;
using DevConsole.Infrastructure.Services;
using DevConsole.Infrastructure.Services.AzureDevOps;
using System;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Linq;

namespace DevConsole.Commands;

public sealed class CreateBranchCommand : DevConsoleCommand
{
    private readonly AzureDevOpsService _azureDevOpsService;
    private readonly Prompts _promptService;

    public CreateBranchCommand(AzureDevOpsService azureDevOpsService, Prompts promptService)
        : base("create-branch", "Create a git branch based on Azure DevOps task id")
    {
        _azureDevOpsService = azureDevOpsService;
        _promptService = promptService;
        Handler = CommandHandler.Create<long?, bool, bool, bool, bool>(DoCommand);

        AddAlias("cb");

        AddArgument(new Argument<long?>("task-id"));
        AddOption(new Option<bool>(new[] { "-cm", "--checkout-master" }, "Switch to master branch."));
        AddOption(new Option<bool>(new[] { "-d", "--discard-all-changes" }, "Discard all changes."));
        AddOption(new Option<bool>(new[] { "-ex", "--experiment" }, "Experimental branch."));
        AddOption(new Option<bool>(new[] { "--ignore-work-item-state" }, "Ignores warning regarding work item state."));
    }

    private void DoCommand(long? taskId,
                           bool checkOutMaster = false,
                           bool discardAllChanges = false,
                           bool experiment = false,
                           bool ignoreWorkItemState = false)
    {
        _azureDevOpsService.EnsureAzCliVersions();

        WorkItemResult? workItem;

        if (taskId is null)
        {
            Console.WriteLine("No work item specified, fetching work item history...");
            var recentWorkItemQuery = GetRecentWorkItems();

            if (recentWorkItemQuery.Error.Length > 0)
            {
                ColorConsole.Write(recentWorkItemQuery.Error, ConsoleColor.Red);
                return;
            }

            var recentWorkItems = recentWorkItemQuery.Output;
            if (recentWorkItems == null || !recentWorkItems.Any())
            {
                throw new UserActionException("There are no work items assigned to you in Azure boards.");
            }

            var selectDictionary =
                recentWorkItems.ToDictionary(x => $"{x.Fields.Id,-5} - ({x.Fields.WorkItemType}) {x.Fields.Title}",
                    x => x);

            var selected = _promptService.Select("Select work item", selectDictionary.Keys.ToArray());

            if (!selectDictionary.TryGetValue(selected, out workItem))
            {
                throw new UserActionException($"Item not found: {selected}");
            }
        }
        else
        {
            var getTaskResult = GetWorkItem(taskId.Value);

            if (getTaskResult.Error.Length > 0)
            {
                ColorConsole.Write(getTaskResult.Error, ConsoleColor.Red);
                return;
            }

            workItem = getTaskResult.Output ??
                       throw new InvalidOperationException($"Invalid output while fetching work item {taskId}");

            ColorConsole.Write("Task found: ");
            ColorConsole.WriteLine(workItem.Fields.Title, ConsoleColor.Green);
        }

        if (discardAllChanges)
        {
            Run("git reset --hard");
        }

        if (checkOutMaster)
        {
            Run("git checkout master");
            Run("git pull");
        }

        var branchName = workItem.GetBranchName(experiment);

        var getBranchesResult = GetOutput("git branch");
        if (!getBranchesResult.Output.Contains(branchName))
        {
            Run($"git branch {branchName}");
        }

        Run($"git checkout {branchName}");

        if (!ignoreWorkItemState &&
            workItem.Fields.State != "Active"
            && _promptService.Confirm($"Work item is in state {workItem.Fields.State}. Change the state to Active?",
                true))
        {
            var updateCommand = $"az boards work-item update --id {workItem.Fields.Id} --state Active";
            if (workItem.Fields.AssignedTo is null)
            {
                updateCommand += " --assigned-to me";
            }

            GetOutput(updateCommand, true);
        }
    }
}