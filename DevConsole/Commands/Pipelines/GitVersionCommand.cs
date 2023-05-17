using DevConsole.Infrastructure.Commands;
using System;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;

namespace DevConsole.Commands.Pipelines;

public class GitVersionCommand : DevConsoleCommand
{
    public GitVersionCommand() : base("gitversion", "Calculate version number using gitversion")
    {
        Handler = CommandHandler.Create<string, string>(DoCommand);

        var pathArgument = new Argument<string>("path");
        pathArgument.SetDefaultValue(Environment.CurrentDirectory);
        AddArgument(pathArgument);

        AddOption(new Option<string>(new[] { "-v", "--version" }, () => string.Empty, "Version of gitversion"));
    }

    private void DoCommand(string path, string version)
    {
        if (string.IsNullOrWhiteSpace(version))
        {
            version = "latest";
        }

        Run(
            $"docker run --rm -v \"{path}:/repo\" gittools/gitversion:{version} /repo /config git-version.yml /nocache /diag");
    }
}