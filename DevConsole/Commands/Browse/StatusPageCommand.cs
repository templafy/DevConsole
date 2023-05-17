using DevConsole.Infrastructure.Commands;
using System.CommandLine.NamingConventionBinder;

namespace DevConsole.Commands.Browse;

public class StatusPageCommand : DevConsoleCommand
{
    public StatusPageCommand() : base("statuspage", "Open public status page to see the current status of our app")
    {
        Handler = CommandHandler.Create(DoCommand);
    }

    private void DoCommand()
    {
        OpenBrowser("https://status.templafy.com");
    }
}