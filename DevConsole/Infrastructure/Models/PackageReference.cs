namespace DevConsole.Infrastructure.Models;

public class PackageReference
{
    public PackageReference(string project, string referenceName, string referenceVersion,
                            PackageReferenceType packageReferenceType, bool isDevelopmentPackage = false)
    {
        Project = project;
        ReferenceName = referenceName;
        ReferenceVersion = referenceVersion;
        PackageReferenceType = packageReferenceType;
        IsDevelopmentPackage = isDevelopmentPackage;
    }

    public string Project { get; }

    public string ReferenceName { get; }

    public string ReferenceVersion { get; }

    public PackageReferenceType PackageReferenceType { get; set; }

    public bool IsDevelopmentPackage { get; set; }
}