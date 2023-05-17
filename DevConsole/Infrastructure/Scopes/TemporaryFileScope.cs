using System;
using System.IO;

namespace DevConsole.Infrastructure.Scopes;

internal class TemporaryFileScope : IDisposable
{
    public TemporaryFileScope()
    {
        TempFile = Path.GetTempFileName();
    }

    public TemporaryFileScope(string filename)
    {
        TempFile = Path.Combine(Path.GetTempPath(), filename);
    }

    public string TempFile { get; }

    public void Dispose()
    {
        File.Delete(TempFile);
    }
}