namespace DevConsole.Infrastructure.Services.AzureDevOps;

public class BuildTimeline
{
    public BuildTimeline(BuildRecord[] records)
    {
        Records = records;
    }

    public BuildRecord[] Records { get; }
}