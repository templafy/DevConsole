using System.Diagnostics;

namespace DevConsole.Infrastructure.Output;

public class RecordOutputHandler : IProcessOutputHandler
{
    public string Output { get; private set; } = string.Empty;

    public string Error { get; private set; } = string.Empty;

    void IProcessOutputHandler.Prepare(ProcessStartInfo processInfo)
    {
        processInfo.RedirectStandardOutput = true;
        processInfo.RedirectStandardError = true;
        processInfo.UseShellExecute = false;
    }

    void IProcessOutputHandler.Handle(Process output)
    {
        Output = output.StandardOutput.ReadToEnd();
        Error = output.StandardError.ReadToEnd();
    }
}