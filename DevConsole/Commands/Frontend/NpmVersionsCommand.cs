using DevConsole.Infrastructure;
using DevConsole.Infrastructure.Commands;
using DevConsole.Infrastructure.Services;
using System;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Linq;

namespace DevConsole.Commands.Frontend;

public class NpmVersionsCommand : DevConsoleCommand
{
    private readonly PackageReferenceService _packageReferenceService;

    public NpmVersionsCommand(PackageReferenceService packageReferenceService)
        : base("npm-versions", "Search npm versions of packages in all child directories")
    {
        _packageReferenceService = packageReferenceService;
        Handler = CommandHandler.Create<string, bool>(DoCommand);

        AddArgument(new Argument<string>("filter"));
        AddOption(new Option<bool>("--exact", "Match exact name."));
    }

    private void DoCommand(string filter, bool exact)
    {
        var packageReferences = _packageReferenceService.GetNpmPackageReferences();

        foreach (var referenceGroup in packageReferences
                                       .Where(pr =>
                                           exact
                                               ? pr.ReferenceName == filter
                                               : pr.ReferenceName.Contains(filter, StringComparison.InvariantCultureIgnoreCase))
                                       .GroupBy(x => new { x.ReferenceName, x.ReferenceVersion })
                                       .OrderByDescending(x => x.Key.ReferenceVersion))
        {
            ColorConsole.WriteLine($"{referenceGroup.Key.ReferenceName}: {referenceGroup.Key.ReferenceVersion}",
                ConsoleColor.DarkCyan);

            var systemGroups = referenceGroup.GroupBy(x => x.Project).ToArray();

            foreach (var systemGroup in systemGroups)
            {
                Console.WriteLine($"- {systemGroup.Key}");
            }
        }
    }
}