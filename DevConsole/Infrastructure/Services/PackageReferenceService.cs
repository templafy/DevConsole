using DevConsole.Infrastructure.Models;
using JetBrains.Annotations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace DevConsole.Infrastructure.Services;

public class PackageReferenceService
{
    public PackageReference[] GetNuGetPackageReferences()
    {
        var packageReferences = new List<PackageReference>();

        foreach (var projectFile in Directory.EnumerateFiles(Environment.CurrentDirectory, "*.csproj",
                     SearchOption.AllDirectories))
        {
            var projectName = Path.GetFileNameWithoutExtension(projectFile);
            var document = XDocument.Load(projectFile);
            foreach (var packageReference in document.Root?.XPathSelectElements("//PackageReference") ??
                                             Enumerable.Empty<XElement>())
            {
                var name = packageReference.Attribute(XName.Get("Include"))?.Value;
                var version = packageReference.Attribute(XName.Get("Version"))?.Value;

                if (name == null || version == null)
                {
                    continue;
                }

                packageReferences.Add(new PackageReference(projectName, name, version, PackageReferenceType.NuGet));
            }
        }

        return packageReferences.ToArray();
    }

    public PackageReference[] GetNpmPackageReferences()
    {
        var packageReferences = new List<PackageReference>();
        var serializer = new JsonSerializer();

        var packageFiles =
            Directory.EnumerateFiles(Environment.CurrentDirectory, "package.json", SearchOption.AllDirectories);

        foreach (var packageFile in packageFiles)
        {
            var projectName = Path.GetFileName(Path.GetDirectoryName(packageFile))!;

            using var file = File.OpenText(packageFile);
            using var reader = new JsonTextReader(file);
            var packageJson = serializer.Deserialize<PackageJson>(reader);

            foreach (var (name, version) in packageJson?.Dependencies ??
                                            Enumerable.Empty<KeyValuePair<string, string>>())
            {
                packageReferences.Add(new PackageReference(projectName, name, version, PackageReferenceType.Npm));
            }

            foreach (var (name, version) in packageJson?.DevDependencies ??
                                            Enumerable.Empty<KeyValuePair<string, string>>())
            {
                packageReferences.Add(new PackageReference(projectName, name, version, PackageReferenceType.Npm, true));
            }
        }

        return packageReferences.ToArray();
    }

    private class PackageJson
    {
        [UsedImplicitly]
        public PackageJson(Dictionary<string, string>? dependencies, Dictionary<string, string>? devDependencies)
        {
            Dependencies = dependencies;
            DevDependencies = devDependencies;
        }

        public Dictionary<string, string>? Dependencies { get; }

        public Dictionary<string, string>? DevDependencies { get; }
    }
}