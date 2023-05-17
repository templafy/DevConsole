namespace DevConsole.Infrastructure.Services.AzureDevOps;

public class PackageVersion
{
    public PackageVersion(string version)
    {
        Version = version;
    }

    public string Version { get; }
}