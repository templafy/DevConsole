using DevConsole.Infrastructure.Commands;
using System.CommandLine.NamingConventionBinder;

namespace DevConsole.Commands.Browse;

public class DiagramCommand : DevConsoleCommand
{
    public DiagramCommand() : base("diagram", "Open diagrams.net - a free online tool to make technical diagrams")
    {
        Handler = CommandHandler.Create(DoCommand);
    }

    private void DoCommand()
    {
        OpenBrowser("https://app.diagrams.net");
    }
}