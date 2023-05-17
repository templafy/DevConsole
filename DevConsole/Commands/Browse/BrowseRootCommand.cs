using DevConsole.Infrastructure.Commands;

namespace DevConsole.Commands.Browse;

public class BrowseRootCommand : DevConsoleCommand
{
    public BrowseRootCommand() : base("browse", "Commands for browsing useful web sites and tools used in Engineering")
    {
    }
}