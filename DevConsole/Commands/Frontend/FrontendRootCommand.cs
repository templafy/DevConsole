using DevConsole.Infrastructure.Commands;

namespace DevConsole.Commands.Frontend;

public sealed class FrontendRootCommand : DevConsoleCommand
{
    public FrontendRootCommand() : base("frontend", "Commands for managing frontend")
    {
        AddAlias("f");
    }
}