using Sharprompt;
using System;
using System.Collections.Generic;

namespace DevConsole.Infrastructure.Services;

public class VersionBumper
{
    private static readonly Dictionary<VersionBump, string> VersionChangeStrings = new()
    {
        { VersionBump.Patch, "Patch version bump - Small change or bug fix" },
        { VersionBump.Minor, "Minor version bump - New feature" },
        { VersionBump.Major, "Major version bump - Breaking change" }
    };

    public bool BumpVersion(VersionBump versionBump)
    {
        if (versionBump == VersionBump.Patch)

            // Patch version automatically bumped by GitVersion.
        {
            return false;
        }

        var stagedChanges = ProcessHelper.GetOutput("git diff --staged");
        if (stagedChanges.Output.Length > 0)
        {
            ColorConsole.WriteFailure(
                "There are staged changes. Please commit or unstage them before bumping the version.");

            return false;
        }

        ProcessHelper.Run($"git commit -m \"+semver: {versionBump.ToString().ToLowerInvariant()}\" --allow-empty");
        return true;
    }

    public void PromptForVersionBump()
    {
        if (HasPreviousVersioningCommit())
        {
            return;
        }

        var versionChange = Prompt.Select("Which kind of version bump do you want to make?",
            Enum.GetValues<VersionBump>(),
            textSelector: VersionChangeStrings.GetValueOrDefault, defaultValue: VersionBump.Patch);

        if (BumpVersion(versionChange))
        {
            ProcessHelper.Run("git push origin");
        }
    }

    private bool HasPreviousVersioningCommit()
    {
        return ProcessHelper.GetOutput("git log --grep +semver master..HEAD --oneline").Output.Length > 0;
    }
}