using System;

namespace DevConsole.Infrastructure.Commands;

public class ExitCodeException : Exception
{
    public ExitCodeException(string command, int exitCode)
        : base($"Command \"{command}\" returned unexpected exit code {exitCode}")
    {
        Command = command;
        ExitCode = exitCode;
    }

    public string Command { get; }

    public int ExitCode { get; }
}