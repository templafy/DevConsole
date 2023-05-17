using DevConsole.Infrastructure.Services.SoftwareVersions;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Newtonsoft.Json;
using System;

namespace DevConsole.Infrastructure.Services.AzureDevOps;

public class AzureDevOpsService
{
    private readonly DevConsoleConfig _devConsoleConfig;
    private readonly SoftwareVersionService _softwareVersionService;

    public AzureDevOpsService(SoftwareVersionService softwareVersionService, DevConsoleConfig devConsoleConfig)
    {
        _softwareVersionService = softwareVersionService;
        _devConsoleConfig = devConsoleConfig;
    }

    public void EnsureAzCliVersions()
    {
        _softwareVersionService.AssertSupported<AzureCliSoftwareAssertion>();
        _softwareVersionService.AssertSupported<AzureCliDevOpsExtensionSoftwareAssertion>();
    }

    public BuildTimeline GetBuildTimeline(int buildId)
    {
        var azDevopsInvokeCommand = $"az devops invoke --organization \"{_devConsoleConfig.AzureDevOpsOrganizationUri}\" --area build --resource timeline --route-parameters project={_devConsoleConfig.AzureDevOpsProjectName} buildId={buildId}";

        var timeline = ProcessHelper.GetJsonOutput<BuildTimeline>(azDevopsInvokeCommand).Output;

        if (timeline is null)
        {
            throw new Exception("Build timeline not found");
        }

        return timeline;
    }

    private class ReleaseOutput
    {
        public ReleaseOutput(int count, Release[] releases)
        {
            Count = count;
            Releases = releases;
        }

        public int Count { get; }

        [JsonProperty("value")]
        public Release[] Releases { get; }
    }
}