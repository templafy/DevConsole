using DevConsole.Commands;
using DevConsole.Commands.Backend;
using DevConsole.Commands.Browse;
using DevConsole.Commands.Frontend;
using DevConsole.Commands.Pipelines;
using Jab;
using System.CommandLine;

namespace DevConsole;

[ServiceProviderModule]
[Singleton<RootCommand>]
[Singleton<AzureLoginCommand>]
[Singleton<CheckForUpdateCommand>]
[Singleton<CreateBranchCommand>]
[Singleton<CreatePullRequestCommand>]
[Singleton<BackendRootCommand>]
[Singleton<ListNuGetDependenciesCommand>]
[Singleton<NuGetVersionsCommand>]
[Singleton<ReSharperLintCommand>]
[Singleton<RestoreAllCommand>]
[Singleton<FrontendRootCommand>]
[Singleton<ListNpmDependenciesCommand>]
[Singleton<RefreshNpmTokenCommand>]
[Singleton<BrowseRootCommand>]
[Singleton<DiagramCommand>]
[Singleton<ExcalidrawCommand>]
[Singleton<StatusPageCommand>]
[Singleton<SupportCommand>]
[Singleton<PipelinesRootCommand>]
[Singleton<GitVersionCommand>]
[Singleton<ProfilePipelineCommand>]
public interface ICommandsModule
{
}