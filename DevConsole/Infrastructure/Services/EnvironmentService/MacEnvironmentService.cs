using System.Runtime.InteropServices;

namespace DevConsole.Infrastructure.Services.EnvironmentService;

public class MacEnvironmentService : IEnvironmentService
{
    public void AssertIsAdministrator()
    {
        // Mac users will be prompted to enter root password
    }

    public bool IsWindows()
    {
        return RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
    }
}