using DevConsole.Infrastructure.Commands;
using DevConsole.Infrastructure.Services;
using System.CommandLine.NamingConventionBinder;

namespace DevConsole.Commands;

public class CheckForUpdateCommand : DevConsoleCommand
{
    private const string MainBranch = "master";
    private readonly IPaths _paths;

    public CheckForUpdateCommand(IPaths paths)
        : base("check-for-updates", "Update DevConsole if master branch is checked out and there is an update available.")
    {
        _paths = paths;
        Handler = CommandHandler.Create(DoCommand);
    }

    private void DoCommand()
    {
        if (GetCurrentBranch() != MainBranch || HasUncommittedChanges())
        {
            return;
        }

        var localCommit = GetLocalCommit();
        var remoteCommit = GetRemoteCommit();

        if (localCommit == remoteCommit)
        {
            return;
        }

        UpdateDevConsole();
    }

    private void UpdateDevConsole()
    {
        GetOutput("git pull", workingDirectory: _paths.DevConsoleFolder);
    }

    private string GetCurrentBranch()
    {
        return GetOutput("git rev-parse --abbrev-ref HEAD", workingDirectory: _paths.DevConsoleFolder).Output;
    }

    private bool HasUncommittedChanges()
    {
        return GetOutput("git status --porcelain", workingDirectory: _paths.DevConsoleFolder).Output != string.Empty;
    }

    private string GetRemoteCommit()
    {
        const int hashLength = 40;
        return GetOutput($"git ls-remote --heads origin refs/heads/{MainBranch}", workingDirectory: _paths.DevConsoleFolder).Output[..hashLength];
    }

    private string GetLocalCommit()
    {
        return GetOutput("git rev-parse HEAD", workingDirectory: _paths.DevConsoleFolder).Output;
    }
}