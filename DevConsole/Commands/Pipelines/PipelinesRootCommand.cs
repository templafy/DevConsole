using DevConsole.Infrastructure.Commands;

namespace DevConsole.Commands.Pipelines;

public sealed class PipelinesRootCommand : DevConsoleCommand
{
    public PipelinesRootCommand() : base("pipelines", "Commands for managing Azure DevOps pipelines")
    {
        AddAlias("p");
    }
}