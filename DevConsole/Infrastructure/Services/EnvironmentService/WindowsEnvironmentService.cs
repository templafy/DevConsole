using DevConsole.Infrastructure.Commands;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace DevConsole.Infrastructure.Services.EnvironmentService;

public class WindowsEnvironmentService : IEnvironmentService
{
    public void AssertIsAdministrator()
    {
#pragma warning disable CA1416
        using var identity = WindowsIdentity.GetCurrent();
        WindowsPrincipal principal = new(identity);

        if (!principal.IsInRole(WindowsBuiltInRole.Administrator))
        {
            throw new UserActionException("Please run again as administrator.");
        }
    }

    public bool IsWindows()
    {
        return RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
    }
}