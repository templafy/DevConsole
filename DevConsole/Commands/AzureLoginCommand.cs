using DevConsole.Infrastructure.Commands;
using DevConsole.Infrastructure.Services;
using System.CommandLine.NamingConventionBinder;
using System.Threading.Tasks;

namespace DevConsole.Commands;

public class AzureLoginCommand : DevConsoleCommand
{
    private readonly AzureLoginService _azureLoginService;

    public AzureLoginCommand(AzureLoginService azureLoginService) : base("azure-login", "Logs into Azure")
    {
        _azureLoginService = azureLoginService;
        Handler = CommandHandler.Create(DoCommand);
    }

    private async Task DoCommand()
    {
        await _azureLoginService.LoginToAzureAndSetSubscription();
    }
}