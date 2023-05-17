using System.Diagnostics;

namespace DevConsole.Infrastructure.Output;

public interface IProcessOutputHandler
{
    void Prepare(ProcessStartInfo processInfo)
    {
    }

    void Handle(Process output)
    {
    }
}