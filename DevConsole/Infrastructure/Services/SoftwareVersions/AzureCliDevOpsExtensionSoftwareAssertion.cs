using DevConsole.Infrastructure.Services.SoftwareVersions.Models;
using System;
using System.Linq;

namespace DevConsole.Infrastructure.Services.SoftwareVersions;

public class AzureCliDevOpsExtensionSoftwareAssertion : ISoftwareAssertion
{
    public Version? MaximumVersion => null;

    public Version MinimumVersion => new(0, 25, 0);

    public void Assert()
    {
        string? version = null;
        if (!ColorConsole.CheckFails("Azure CLI DevOps Extension",
                () => !IsSupportedAzureCliDevOpsExtensionVersionInstalled(out version)))
        {
            return;
        }

        if (version is not null)
        {
            Console.WriteLine("Azure DevOps extension outdated. Updating...");
            ProcessHelper.Run("az extension remove --name azure-devops");
            ProcessHelper.Run($"az extension add --name azure-devops --version {version}");
            return;
        }

        Console.WriteLine("Azure DevOps extension not found. Installing...");
        ProcessHelper.Run("az extension add --name azure-devops");
    }

    private bool IsSupportedAzureCliDevOpsExtensionVersionInstalled(out string? version)
    {
        var output = ProcessHelper.GetJsonOutput<AzExtension[]>("az extension list");
        var azDevOpsExtension = output.Output?.FirstOrDefault(e => e.Name == "azure-devops");

        if (azDevOpsExtension is null)
        {
            version = null;
            return false;
        }

        if (MaximumVersion is not null && new Version(azDevOpsExtension.Version) > MaximumVersion)
        {
            version = MinimumVersion.ToString();
            return false;
        }

        if (new Version(azDevOpsExtension.Version) < MinimumVersion)
        {
            version = MinimumVersion.ToString();
            return false;
        }

        version = MinimumVersion.ToString();
        return true;
    }
}