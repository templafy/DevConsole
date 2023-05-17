using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.CommandLine;

namespace DevConsole.Infrastructure.Commands;

public class CommandNode
{
    private readonly IDictionary<Type, CommandNode> _children = new Dictionary<Type, CommandNode>();
    private readonly Type _commandType;

    public CommandNode() : this(typeof(RootCommand))
    {
        _commandType = typeof(RootCommand);
    }

    private CommandNode(Type type)
    {
        _commandType = type;
    }

    public CommandNode AddCommand<TCommand>(Action<CommandNode>? addChildren = null) where TCommand : DevConsoleCommand
    {
        var type = typeof(TCommand);
        var node = new CommandNode(type);
        _children[type] = node;
        addChildren?.Invoke(node);
        return node;
    }

    public Command LinkCommands(IServiceProvider provider, Command? parentCommand = null)
    {
        var command = (Command)provider.GetRequiredService(_commandType);
        parentCommand?.AddCommand(command);

        foreach (var child in _children.Values)
        {
            child.LinkCommands(provider, command);
        }

        return command;
    }
}