namespace DevConsole.Infrastructure.Services.SoftwareVersions.Models;

public class AzExtension
{
    public AzExtension(string name, string version)
    {
        Name = name;
        Version = version;
    }

    public string Name { get; }

    public string Version { get; }
}