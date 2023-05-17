using DevConsole.Infrastructure;
using DevConsole.Infrastructure.Commands;
using DevConsole.Infrastructure.Services.AzureDevOps;
using System;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Linq;

namespace DevConsole.Commands.Pipelines;

public class ProfilePipelineCommand : DevConsoleCommand
{
    private readonly AzureDevOpsService _azureDevOpsService;

    public ProfilePipelineCommand(AzureDevOpsService azureDevOpsService)
        : base("profile", "Analyze an Azure DevOps pipeline run based on build timeline")
    {
        _azureDevOpsService = azureDevOpsService;
        AddArgument(new Argument<int>("build-id"));

        Handler = CommandHandler.Create<int>(DoCommand);
    }

    private void DoCommand(int buildId)
    {
        ColorConsole.WriteLine("Fetching build timeline... ", ConsoleColor.Green);
        var timeline = _azureDevOpsService.GetBuildTimeline(buildId);

        var relevantRecords = timeline.Records
                                      .Where(r => r is { Type: BuildRecordType.Task, Result: BuildRecordResult.Succeeded })
                                      .ToArray();

        Console.WriteLine("Tasks: ");
        Console.WriteLine();

        foreach (var group in relevantRecords.GroupBy(r => r.Name).OrderBy(g => g.Sum(r => r.ExecutionTime.TotalMilliseconds)))
        {
            Console.WriteLine($"{Math.Round(group.Sum(r => r.ExecutionTime.TotalMilliseconds) / 1000),5} seconds - {group.Count(),2} tasks - {group.Key}");
        }

        var sum = relevantRecords.Sum(r => r.ExecutionTime.TotalMilliseconds);

        var jobs = timeline.Records
                           .Where(r => r is { Type: BuildRecordType.Job, Result: BuildRecordResult.Succeeded })
                           .ToArray();

        Console.WriteLine();
        Console.WriteLine("Jobs: ");
        Console.WriteLine();

        foreach (var job in jobs.OrderBy(j => j.ExecutionTime))
        {
            Console.WriteLine($"{Math.Round(job.ExecutionTime.TotalMilliseconds / 1000),5} seconds - {job.Name}");
        }

        Console.WriteLine();
        Console.WriteLine($"Task execution time: {Math.Round(sum / 1000)} seconds");

        var startTime = jobs.Where(j => j.StartTime is not null).Min(j => j.StartTime!);
        var finishTime = jobs.Where(j => j.FinishTime is not null).Max(j => j.FinishTime);

        if (startTime is not null && finishTime is not null)
        {
            Console.WriteLine(
                $"Wall clock time: {Math.Round((finishTime - startTime).Value.TotalMilliseconds / 1000)} seconds");
        }
    }
}