using DevConsole.Infrastructure;
using DevConsole.Infrastructure.Commands;
using System;
using System.CommandLine.NamingConventionBinder;
using System.IO;
using System.Threading.Tasks;

namespace DevConsole.Commands.Backend;

public sealed class RestoreAllCommand : DevConsoleCommand
{
    public RestoreAllCommand() : base("restore-all", "Restore NuGet packages in all solutions")
    {
        Handler = CommandHandler.Create(DoCommand);
    }

    private Task DoCommand()
    {
        foreach (var solutionFile in Directory.EnumerateFiles(Environment.CurrentDirectory, "*.sln", SearchOption.AllDirectories))
        {
            ColorConsole.WriteLine($"Solution: {solutionFile}", ConsoleColor.DarkCyan);
            ProcessHelper.Run("dotnet restore", Path.GetDirectoryName(solutionFile));
        }

        return Task.CompletedTask;
    }
}