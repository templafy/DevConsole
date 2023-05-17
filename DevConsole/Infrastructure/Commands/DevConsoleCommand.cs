using DevConsole.Infrastructure.Models;
using DevConsole.Infrastructure.Output;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Runtime.InteropServices;

namespace DevConsole.Infrastructure.Commands;

public abstract class DevConsoleCommand : Command
{
    protected DevConsoleCommand(string name, string? description = null) : base(name, description)
    {
    }

    protected ProcessResult GetOutput(string command, bool outputCommand = false, string? workingDirectory = null,
                                      bool expectExitCodeToBeZero = true)
    {
        return ProcessHelper.GetOutput(command, outputCommand, workingDirectory, expectExitCodeToBeZero);
    }

    protected static JsonProcessResult<T> GetJsonOutput<T>(string command, bool outputCommand = false, string? workingDirectory = null, bool expectExitCodeToBeZero = true, bool ignoreError = false) where T : class
    {
        return ProcessHelper.GetJsonOutput<T>(command, outputCommand, workingDirectory, expectExitCodeToBeZero, ignoreError);
    }

    protected static int Run(string command, string? workingDirectory = null, bool outputCommand = true, bool expectExitCodeToBeZero = true, Dictionary<string, string?>? environmentVariables = null, IProcessOutputHandler? outputHandler = null)
    {
        return ProcessHelper.Run(command, workingDirectory, outputCommand, expectExitCodeToBeZero, environmentVariables, outputHandler);
    }

    protected static int RunSupressed(string command, string? workingDirectory = null, bool outputCommand = false, bool expectExitCodeToBeZero = true, Dictionary<string, string?>? environmentVariables = null)
    {
        return ProcessHelper.Run(command, workingDirectory, outputCommand, expectExitCodeToBeZero, environmentVariables, SuppressOutputHandler.Instance);
    }

    protected static int RunSupressedOutput(string command, string? workingDirectory = null, bool outputCommand = true, bool expectExitCodeToBeZero = true, Dictionary<string, string?>? environmentVariables = null)
    {
        return ProcessHelper.Run(command, workingDirectory, outputCommand, expectExitCodeToBeZero, environmentVariables, SuppressOutputHandler.Instance);
    }

    protected int OpenBrowser(string url)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return Run($"open {url}");
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return Run($"xdg-open {url}");
        }

        return Run($"start {url}");
    }

    protected JsonProcessResult<WorkItemResult[]> GetRecentWorkItems()
    {
        return GetJsonOutput<WorkItemResult[]>(
            "az boards query --wiql 'SELECT Id, Title, State, [System.WorkItemType] FROM WorkItems WHERE [System.AssignedTo] = @me AND [System.State] IN (''Ready'', ''New'', ''Active'') ORDER BY [System.WorkItemType] ASC, [System.ChangedDate] DESC'",
            true, expectExitCodeToBeZero: false);
    }

    protected JsonProcessResult<WorkItemResult> GetWorkItem(long workItemId)
    {
        return GetJsonOutput<WorkItemResult>("az boards work-item show " +
                                             $"--id {workItemId} " +
                                             "--expand none " +
                                             "--fields System.Id,System.WorkItemType,System.Title,System.State,System.AssignedTo " +
                                             "--only-show-errors",
            true, expectExitCodeToBeZero: false);
    }

    protected void AddArgumentWithSuggestions(string name, IReadOnlySet<string> validOptions, string defaultValue, string? description = null)
    {
        var argument = new Argument<string>(name, () => defaultValue, description)
            .AddCompletions(context => validOptions.Where(value => value.Contains(context.WordToComplete)));

        AddArgument(argument);
    }

    protected void AddOptionWithSuggestions(string name, IReadOnlySet<string> validOptions, string defaultValue, string? description = null)
    {
        var option = new Option<string>(name, () => defaultValue, description)
            .AddCompletions(context => validOptions.Where(value => value.Contains(context.WordToComplete)));

        AddOption(option);
    }
}