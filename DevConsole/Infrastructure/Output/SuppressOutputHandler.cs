using System.Diagnostics;

namespace DevConsole.Infrastructure.Output;

public class SuppressOutputHandler : IProcessOutputHandler
{
    public static readonly SuppressOutputHandler Instance = new();

    void IProcessOutputHandler.Prepare(ProcessStartInfo processInfo)
    {
        processInfo.RedirectStandardOutput = true;
        processInfo.RedirectStandardError = true;
        processInfo.UseShellExecute = false;
    }

    public void Handle(Process output)
    {
        output.StandardOutput.ReadToEnd();
        output.StandardError.ReadToEnd();
    }
}