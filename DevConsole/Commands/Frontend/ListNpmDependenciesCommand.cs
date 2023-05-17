using DevConsole.Commands.Generic;
using DevConsole.Infrastructure.Services;
using System.CommandLine.NamingConventionBinder;

namespace DevConsole.Commands.Frontend;

public class ListNpmDependenciesCommand : ListDependenciesCommand
{
    private readonly PackageReferenceService _packageReferenceService;

    public ListNpmDependenciesCommand(PackageReferenceService packageReferenceService)
        : base("list-dependencies", "List all NPM packages used as dependencies in self-contained systems")
    {
        _packageReferenceService = packageReferenceService;

        Handler = CommandHandler.Create<bool?, ListDependenciesFormat>(DoCommand);
    }

    private void DoCommand(bool? checkoutMasterAndDiscardChanges, ListDependenciesFormat format)
    {
        List(format, () => _packageReferenceService.GetNpmPackageReferences());
    }
}