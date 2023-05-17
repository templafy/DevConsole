using Newtonsoft.Json;

namespace DevConsole.Infrastructure.Services.SoftwareVersions.Models;

public class AzVersion
{
    public AzVersion(string cliVersion)
    {
        CliVersion = cliVersion;
    }

    [JsonProperty("azure-cli")]
    public string CliVersion { get; }
}