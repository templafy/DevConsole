using DevConsole.Infrastructure.Commands;
using DevConsole.Infrastructure.Models;
using DevConsole.Infrastructure.Services.AzureDevOps;
using System.Threading.Tasks;

namespace DevConsole.Infrastructure.Services;

public class AzureLoginService
{
    private readonly AzureDevOpsService _azureDevOpsService;
    private readonly DevConsoleConfig _devConsoleConfig;

    public AzureLoginService(AzureDevOpsService azureDevOpsService, DevConsoleConfig devConsoleConfig)
    {
        _azureDevOpsService = azureDevOpsService;
        _devConsoleConfig = devConsoleConfig;
    }

    public async Task LoginToAzureAndSetSubscription()
    {
        _azureDevOpsService.EnsureAzCliVersions();

        var azAccountListResult = Task.Run(() => ProcessHelper.GetOutput("az account list", expectExitCodeToBeZero: false));

        var azAccountGetAccessTokenResult = Task.Run(() => ProcessHelper.GetOutput("az account get-access-token --query accessToken --output tsv", expectExitCodeToBeZero: false));

        if (await ColorConsole.CheckFails("Azure account", async () => (await azAccountListResult).Error.Contains("Please run 'az login' to setup account."))
            || await ColorConsole.CheckFails("Azure access token", async () => (await azAccountGetAccessTokenResult).Error.Contains("The refresh token has expired"))
            || await ColorConsole.CheckFails("Azure subscription access", async () => !HasAccessToTenant(await azAccountListResult)))
        {
            var azLoginResult = ProcessHelper.GetOutput($"az login --tenant {_devConsoleConfig.AzureTenant}");

            if (!HasAccessToTenant(azLoginResult))
            {
                throw new UserActionException($"You do not have access to the {_devConsoleConfig.AzureSubscription} subscription.");
            }
        }

        if (ColorConsole.CheckFails("Azure current subscription", () => !ProcessHelper.GetOutput("az account show").Output.Contains(_devConsoleConfig.AzureSubscription)))
        {
            ColorConsole.WriteLine($"Setting account subscription to: {_devConsoleConfig.AzureSubscription}");
            ProcessHelper.Run($"az account set --subscription \"{_devConsoleConfig.AzureSubscription}\"");
        }
    }

    private bool HasAccessToTenant(ProcessResult result)
    {
        return result.Output.Contains($"\"name\": \"{_devConsoleConfig.AzureSubscription}\"");
    }
}