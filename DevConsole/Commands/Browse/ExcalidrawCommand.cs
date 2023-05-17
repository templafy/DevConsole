using DevConsole.Infrastructure.Commands;
using System.CommandLine.NamingConventionBinder;

namespace DevConsole.Commands.Browse;

public class ExcalidrawCommand : DevConsoleCommand
{
    public ExcalidrawCommand() : base("excalidraw", "Open excalidraw.com - a free online tool to draw and collaborate")
    {
        Handler = CommandHandler.Create(DoCommand);
    }

    private void DoCommand()
    {
        OpenBrowser("https://excalidraw.com");
    }
}