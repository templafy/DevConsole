using DevConsole.Infrastructure.Services;
using DevConsole.Infrastructure.Services.AzureDevOps;
using DevConsole.Infrastructure.Services.EnvironmentService;
using DevConsole.Infrastructure.Services.SoftwareVersions;
using Jab;
using Microsoft.Extensions.Configuration;

namespace DevConsole;

[ServiceProviderModule]
[Transient<IPaths, Paths>]
[Transient<Prompts>]
#if OS_WINDOWS
[Singleton<IEnvironmentService, WindowsEnvironmentService>]
#else
[Singleton<IEnvironmentService, MacEnvironmentService>]
#endif
[Singleton<AzureDevOpsService>]
[Singleton<SoftwareVersionService>]
[Singleton<PackageReferenceService>]
[Singleton<AzureLoginService>]
[Singleton<VersionBumper>]
[Singleton<DevConsoleConfig>]
[Singleton<IConfiguration>(Instance = nameof(Configuration))]
public interface IServicesModule
{
    public IConfiguration Configuration { get; }
}