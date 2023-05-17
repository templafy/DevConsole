using System;

namespace DevConsole.Infrastructure.Services.AzureDevOps;

public class BuildRecord
{
    public BuildRecord(string name, BuildRecordType type, DateTime? startTime, DateTime? finishTime, BuildRecordResult result)
    {
        Name = name;
        Type = type;
        StartTime = startTime;
        FinishTime = finishTime;
        Result = result;
    }

    public string Name { get; }

    public BuildRecordType Type { get; }

    public DateTime? StartTime { get; }

    public DateTime? FinishTime { get; }

    public BuildRecordResult Result { get; }

    public TimeSpan ExecutionTime
    {
        get
        {
            if (StartTime is { } start && FinishTime is { } finish)
            {
                return finish - start;
            }

            return TimeSpan.Zero;
        }
    }
}