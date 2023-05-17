using DevConsole.Commands.Generic;
using DevConsole.Infrastructure.Services;
using System.CommandLine.NamingConventionBinder;

namespace DevConsole.Commands.Backend;

public class ListNuGetDependenciesCommand : ListDependenciesCommand
{
    private readonly PackageReferenceService _packageReferenceService;

    public ListNuGetDependenciesCommand(PackageReferenceService packageReferenceService)
        : base("list-dependencies", "List all NuGet packages used as dependencies in self-contained systems")
    {
        _packageReferenceService = packageReferenceService;

        Handler = CommandHandler.Create<ListDependenciesFormat>(DoCommand);
    }

    private void DoCommand(ListDependenciesFormat format)
    {
        List(format, () => _packageReferenceService.GetNuGetPackageReferences());
    }
}