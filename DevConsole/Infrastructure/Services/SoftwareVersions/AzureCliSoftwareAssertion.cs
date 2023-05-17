using DevConsole.Infrastructure.Commands;
using DevConsole.Infrastructure.Services.SoftwareVersions.Models;
using System;

namespace DevConsole.Infrastructure.Services.SoftwareVersions;

public class AzureCliSoftwareAssertion : ISoftwareAssertion
{
    public Version? MaximumVersion => null;

    public Version MinimumVersion => new(2, 38, 0);

    public void Assert()
    {
        string? azureCliVersion = null;
        if (!ColorConsole.CheckFails("Azure CLI",
                () => !IsSupportedAzureCliVersionInstalled(out azureCliVersion)))
        {
            return;
        }

        if (azureCliVersion is not null)
        {
            throw new UserActionException(
                $"Please use at least azure-cli version {MinimumVersion} (choco upgrade azure-cli --version {MinimumVersion})");
        }

        throw new UserActionException(
            $"Azure CLI not found, please install azure-cli (choco install azure-cli --version {MinimumVersion})");
    }

    private bool IsSupportedAzureCliVersionInstalled(out string? version)
    {
        var output = ProcessHelper.GetJsonOutput<AzVersion>("az version", expectExitCodeToBeZero: false);
        if (output.Output?.CliVersion is null)
        {
            version = null;
            return false;
        }

        if (MaximumVersion is not null && new Version(output.Output.CliVersion) > MaximumVersion)
        {
            version = MinimumVersion.ToString();
            return false;
        }

        if (new Version(output.Output.CliVersion) < MinimumVersion)
        {
            version = MinimumVersion.ToString();
            return false;
        }

        version = MinimumVersion.ToString();
        return true;
    }
}