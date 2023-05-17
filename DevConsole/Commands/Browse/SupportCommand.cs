using DevConsole.Infrastructure.Commands;
using System.CommandLine.NamingConventionBinder;

namespace DevConsole.Commands.Browse;

public class SupportCommand : DevConsoleCommand
{
    public SupportCommand() : base("support", "Open Support website containing product documentation")
    {
        Handler = CommandHandler.Create(DoCommand);
    }

    private void DoCommand()
    {
        OpenBrowser("https://support.templafy.com");
    }
}