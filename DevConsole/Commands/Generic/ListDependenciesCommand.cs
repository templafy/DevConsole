using CsvHelper;
using DevConsole.Infrastructure.Commands;
using DevConsole.Infrastructure.Models;
using JetBrains.Annotations;
using System;
using System.CommandLine;
using System.Globalization;
using System.Linq;

namespace DevConsole.Commands.Generic;

public abstract class ListDependenciesCommand : DevConsoleCommand
{
    protected ListDependenciesCommand(string name, string description)
        : base(name, description)
    {
        AddOption(new Option<ListDependenciesFormat>("--format", () => ListDependenciesFormat.Csv, "Output format"));
    }

    protected void List(ListDependenciesFormat format, Func<PackageReference[]> getPackageReferences)
    {
        var packageReferences = getPackageReferences();

        if (format == ListDependenciesFormat.Csv)
        {
            using var csvWriter = new CsvWriter(Console.Out, CultureInfo.InvariantCulture);
            csvWriter.WriteRecords(packageReferences);
        }
        else
        {
            foreach (var project in packageReferences.GroupBy(r => r.Project))
            {
                Console.WriteLine($"{project.Key}");
                foreach (var packageReference in project)
                {
                    Console.WriteLine(
                        $"- {packageReference.ReferenceName} version {packageReference.ReferenceVersion}");
                }
            }
        }
    }

    protected enum ListDependenciesFormat
    {
        Csv,

        [UsedImplicitly]
        Text
    }
}