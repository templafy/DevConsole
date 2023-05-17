using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DevConsole.Infrastructure.Services;

public class Paths : IPaths
{
    private static readonly Lazy<string> MetaRootDirectory = new(() =>
    {
        var directory = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ??
                                          throw new InvalidOperationException());

        string metaRootDirectory = null!;
        while (directory is not null)
        {
            if (directory.EnumerateFiles(".meta").Any())
            {
                metaRootDirectory = directory.FullName;
                break;
            }

            directory = directory.Parent;
        }

        return metaRootDirectory;
    });

    public string DevConsoleFolder => Path.Combine(MetaRootDirectory.Value, "DevConsole");
}