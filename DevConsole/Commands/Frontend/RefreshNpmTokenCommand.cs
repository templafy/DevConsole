using DevConsole.Infrastructure;
using DevConsole.Infrastructure.Commands;
using System;
using System.CommandLine.NamingConventionBinder;
using System.IO;
using System.Runtime.InteropServices;

namespace DevConsole.Commands.Frontend;

public class RefreshNpmTokenCommand : DevConsoleCommand
{
    public RefreshNpmTokenCommand()
        : base("refresh-npm-token", "Refresh npm feed Azure DevOps token in current directory")
    {
        Handler = CommandHandler.Create(DoCommand);
    }

    private void DoCommand()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Console.WriteLine("Refresh of npm token is only supported on Windows.");
            return;
        }

        if (!File.Exists(".npmrc"))
        {
            ColorConsole.WriteLine("Must be run in a directory with .npmrc", ConsoleColor.Red);
            return;
        }

        Run("Remove-Item .npmrc -ErrorAction SilentlyContinue", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), expectExitCodeToBeZero: false);

        Run("npx vsts-npm-auth -config .npmrc");
    }
}