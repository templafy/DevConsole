using DevConsole.Infrastructure;
using DevConsole.Infrastructure.Commands;
using DevConsole.Infrastructure.Services;
using System;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.IO;
using System.Linq;

namespace DevConsole.Commands.Backend;

public class ReSharperLintCommand : DevConsoleCommand
{
    private readonly Prompts _prompts;

    public ReSharperLintCommand(Prompts prompts) : base("resharper-lint",
        "Runs the ReSharper linting tools against a solution file")
    {
        _prompts = prompts;
        Handler = CommandHandler.Create<string>(DoCommand);
        AddOption(new Option<string>(new[] { "-v", "--version" },
            "Use specific version instead of the latest one. E.g. \"2020.3.0\""));
    }

    private void DoCommand(string version)
    {
        var solutionFullPath = ChooseSolutionToAnalyze();
        if (solutionFullPath == null)
        {
            return;
        }

        InstallResharperGlobalTools(version);

        var outputFile = AnalyzeSolution(solutionFullPath);

        ViewOutputFile(outputFile);
    }

    private void InstallResharperGlobalTools(string version)
    {
        var versionArgument = string.IsNullOrWhiteSpace(version) ? "" : $"--version {version}";
        Run($"dotnet tool update JetBrains.ReSharper.GlobalTools --global {versionArgument} --ignore-failed-sources");
    }

    private string? ChooseSolutionToAnalyze()
    {
        var solutionFiles = Directory.GetFiles(Environment.CurrentDirectory).Where(f => f.EndsWith(".sln")).ToArray();
        if (solutionFiles.Length == 0)
        {
            ColorConsole.WriteLine("Failed. No solution file(s) found in the current directory.", ConsoleColor.Red);
            return null;
        }

        if (solutionFiles.Length == 1)
        {
            return solutionFiles[0];
        }

        var solutionFile = _prompts.Select("Select solution file", solutionFiles);

        try
        {
            return Path.GetFullPath(solutionFile);
        }
        catch (Exception e)
        {
            ColorConsole.WriteLine($"Failed. {e.Message}.", ConsoleColor.Red);
            return null;
        }
    }

    private string AnalyzeSolution(string solutionFullPath)
    {
        var outputFile = $"{Path.GetTempPath()}/inspect-code-log.xml";
        Run(
            $"jb inspectcode \"{solutionFullPath}\" --output=\"{outputFile}\" --severity=WARNING --properties=\"Configuration=Release\" --no-swea --verbosity=OFF");

        return outputFile;
    }

    private void ViewOutputFile(string outputFile)
    {
        Console.WriteLine("View result...");

        Run($"type {outputFile}");
    }
}