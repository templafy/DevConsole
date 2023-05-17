using DevConsole.Infrastructure.Commands;

namespace DevConsole.Commands.Backend;

public sealed class BackendRootCommand : DevConsoleCommand
{
    public BackendRootCommand() : base("backend", "Commands for managing backend")
    {
        AddAlias("b");
    }
}