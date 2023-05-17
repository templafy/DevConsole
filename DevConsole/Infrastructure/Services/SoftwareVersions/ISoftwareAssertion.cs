using System;

namespace DevConsole.Infrastructure.Services.SoftwareVersions;

public interface ISoftwareAssertion
{
    Version? MaximumVersion { get; }

    Version MinimumVersion { get; }

    void Assert();
}