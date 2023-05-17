using Jab;
using Microsoft.Extensions.Configuration;

namespace DevConsole;

[ServiceProvider]
[Import<ICommandsModule>]
[Import<IServicesModule>]
public partial class DevConsoleServiceProvider
{
    public required IConfiguration Configuration { get; init; }
}