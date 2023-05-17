using System;

namespace DevConsole.Infrastructure.Models;

public class ProcessResult
{
    public ProcessResult(string command, string output, string error, int exitCode)
    {
        Command = command ?? throw new ArgumentNullException(nameof(command));
        Output = output ?? throw new ArgumentNullException(nameof(output));
        Error = error ?? throw new ArgumentNullException(nameof(error));
        ExitCode = exitCode;
    }

    public string Command { get; }

    public string Output { get; }

    public string Error { get; }

    public int ExitCode { get; }
}