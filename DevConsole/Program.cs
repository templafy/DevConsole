using DevConsole.Commands;
using DevConsole.Commands.Backend;
using DevConsole.Commands.Browse;
using DevConsole.Commands.Frontend;
using DevConsole.Commands.Pipelines;
using DevConsole.Infrastructure;
using DevConsole.Infrastructure.Commands;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DevConsole;

public static class Program
{
    private const string RootCommandName = "dev";
    private static ParseResult? _parseResult;

    public static async Task<int> Main(string[] args)
    {
        Console.InputEncoding = Encoding.UTF8;
        Console.OutputEncoding = Encoding.UTF8;

        args = EscapeArgumentsStartingWithAt(args);
        var serviceProvider = new DevConsoleServiceProvider
        {
            Configuration = LoadConfiguration()
        };

        var rootNode = ConfigureCommands();
        var rootCommand = ResolveRootCommand(serviceProvider, rootNode);

        var parser = new CommandLineBuilder(rootCommand)
                     .UseVersionOption()
                     .UseHelp()
                     .UseParseDirective()
                     .UseSuggestDirective()
                     .RegisterWithDotnetSuggest()
                     .UseTypoCorrections()
                     .UseParseErrorReporting()
                     .UseExceptionHandler(OnException)
                     .CancelOnProcessTermination()
                     .Build();

        _parseResult = parser.Parse(args);
        var exitCode = await _parseResult.InvokeAsync();

        return exitCode;
    }

    private static void OnException(Exception exception, InvocationContext context)
    {
        while (exception is TargetInvocationException targetInvocationException &&
               targetInvocationException.InnerException is not null)
        {
            exception = targetInvocationException.InnerException;
        }

        if (exception is UserActionException userActionException)
        {
            // User needs to take action to use the command. Don't track the exception.
            ColorConsole.WriteLine($"{userActionException.Message}", ConsoleColor.Red);
            return;
        }

        if (exception is ExitCodeException exitCodeException)
        {
            ColorConsole.WriteLine($"Command failed with unexpected exit code: {exitCodeException.ExitCode}", ConsoleColor.Red);

            ColorConsole.WriteLine($"> {exitCodeException.Command}", ConsoleColor.Red);
        }
        else
        {
            ColorConsole.Write("Unhandled exception: ", ConsoleColor.Red);
            ColorConsole.WriteLine(exception.Message, ConsoleColor.Red);
        }

        context.ExitCode = 1;
    }

    private static IConfiguration LoadConfiguration()
    {
        var builder = new ConfigurationBuilder();
        return builder.Build();
    }

    public static CommandNode ConfigureCommands()
    {
        var rootNode = new CommandNode();

        rootNode.AddCommand<AzureLoginCommand>();
        rootNode.AddCommand<CheckForUpdateCommand>();
        rootNode.AddCommand<CreateBranchCommand>();
        rootNode.AddCommand<CreatePullRequestCommand>();

        rootNode.AddCommand<BackendRootCommand>(backendNode =>
        {
            backendNode.AddCommand<ListNuGetDependenciesCommand>();
            backendNode.AddCommand<NuGetVersionsCommand>();
            backendNode.AddCommand<ReSharperLintCommand>();
            backendNode.AddCommand<RestoreAllCommand>();
        });

        rootNode.AddCommand<FrontendRootCommand>(frontendNode =>
        {
            frontendNode.AddCommand<ListNpmDependenciesCommand>();
            frontendNode.AddCommand<RefreshNpmTokenCommand>();
        });

        rootNode.AddCommand<BrowseRootCommand>(browseNode =>
        {
            browseNode.AddCommand<DiagramCommand>();
            browseNode.AddCommand<ExcalidrawCommand>();
            browseNode.AddCommand<StatusPageCommand>();
            browseNode.AddCommand<SupportCommand>();
        });

        rootNode.AddCommand<PipelinesRootCommand>(pipelinesNode =>
        {
            pipelinesNode.AddCommand<GitVersionCommand>();
            pipelinesNode.AddCommand<ProfilePipelineCommand>();
        });

        return rootNode;
    }

    public static RootCommand ResolveRootCommand(IServiceProvider serviceProvider, CommandNode rootNode)
    {
        if (rootNode.LinkCommands(serviceProvider) is not RootCommand rootCommand)
        {
            throw new ArgumentException("Cannot find root command");
        }

        rootCommand.Name = RootCommandName;
        return rootCommand;
    }

    private static string[] EscapeArgumentsStartingWithAt(string[] args)
    {
        var escapedArgs = new List<string>();
        foreach (var arg in args)
        {
            if (arg.StartsWith("@"))
            {
                escapedArgs.Add("^@" + arg.Substring(1));
                Console.WriteLine("The parameter starts with @. At the moment, System.CommandLine doesn't " +
                                  "support escaping of this character. DevConsole will escape it as '^@'. " +
                                  "Your command may not support custom escaping.");
            }
            else
            {
                escapedArgs.Add(arg);
            }
        }

        return escapedArgs.ToArray();
    }
}