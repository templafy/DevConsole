namespace DevConsole.Infrastructure.Services.SoftwareVersions;

public class SoftwareVersionService
{
    public void AssertSupported<T>() where T : ISoftwareAssertion, new()
    {
        new T().Assert();
    }
}