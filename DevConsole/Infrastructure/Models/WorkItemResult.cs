using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DevConsole.Infrastructure.Models;

public class WorkItemResult
{
    public WorkItemResult(WorkItemResultFields fields)
    {
        Fields = fields ?? throw new ArgumentNullException(nameof(fields));
    }

    public WorkItemResultFields Fields { get; }

    public string GetBranchName(bool isExperimental = false)
    {
        var prefix = isExperimental ? "experiment" : "feature";
        var title = Sanitize(Fields.Title);
        return $"{prefix}/{Fields.Id}-{title}";
    }

    private string Sanitize(string input)
    {
        var patterns = new Dictionary<Regex, string>
        {
            { new Regex("['\":()[\\],*]"), string.Empty },
            { new Regex("[ \\\\/]"), "_" },
            { new Regex("[&]"), "and" }
        };

        input = patterns.Aggregate(input, (current, pair) => pair.Key.Replace(current, pair.Value));
        const string invalidCharactersPattern = @"[\.!\?`]";
        return Regex.Replace(input, invalidCharactersPattern, string.Empty);
    }
}

public class WorkItemResultFields
{
    public WorkItemResultFields(
        [JsonProperty("System.Id")] long id,
        [JsonProperty("System.WorkItemType")] string workItemType,
        [JsonProperty("System.Title")] string title,
        [JsonProperty("System.State")] string state,
        [JsonProperty("System.AssignedTo")] WorkItemAssignedTo? assignedTo)
    {
        Id = id;
        WorkItemType = workItemType ?? throw new ArgumentNullException(nameof(workItemType));
        Title = title ?? throw new ArgumentNullException(nameof(title));
        State = state ?? throw new ArgumentNullException(nameof(state));
        AssignedTo = assignedTo;
    }

    public long Id { get; }

    public string WorkItemType { get; }

    public string Title { get; }

    public string State { get; }

    public WorkItemAssignedTo? AssignedTo { get; set; }

    public record WorkItemAssignedTo(string DisplayName, string UniqueName);
}