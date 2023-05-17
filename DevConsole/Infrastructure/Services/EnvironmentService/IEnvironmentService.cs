namespace DevConsole.Infrastructure.Services.EnvironmentService;

public interface IEnvironmentService
{
    void AssertIsAdministrator();

    bool IsWindows();
}