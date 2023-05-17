using DevConsole.Infrastructure.Commands;
using System;
using System.Text.RegularExpressions;

namespace DevConsole.Infrastructure.Services.SoftwareVersions;

public class GitSoftwareAssertion : ISoftwareAssertion
{
    public Version? MaximumVersion => null;

    public Version MinimumVersion => new(2, 29, 0);

    public void Assert()
    {
        if (!ColorConsole.CheckFails("Git", !IsSupportedGitVersionInstalled(out var gitVersion)))
        {
            return;
        }

        if (gitVersion is not null)
        {
            throw new UserActionException($"Please use at least git version {MinimumVersion} (choco upgrade git)");
        }

        throw new UserActionException("Git not found, please install git (choco install git)");
    }

    private bool IsSupportedGitVersionInstalled(out string? version)
    {
        // Command outputs "git version {version}", i.e., "git version 2.31.1.windows.1".
        var output = ProcessHelper.GetOutput("git version", expectExitCodeToBeZero: false).Output;
        var match = Regex.Match(output, "git version (\\d+\\.\\d+\\.\\d+)").Groups[1];
        if (!output.StartsWith("git") || !match.Success)
        {
            version = null;
            return false;
        }

        if (MaximumVersion is not null && new Version(match.Value) > MaximumVersion)
        {
            version = MinimumVersion.ToString();
        }

        if (new Version(match.Value) < MinimumVersion)
        {
            version = MinimumVersion.ToString();
            return false;
        }

        version = MinimumVersion.ToString();
        return true;
    }
}