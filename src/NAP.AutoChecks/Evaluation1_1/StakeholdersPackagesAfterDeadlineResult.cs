namespace NAP.AutoChecks.Evaluation1_1;

public class StakeholdersPackagesAfterDeadlineResult
{
    public string OrganizationName { get; set; }
    
    public Guid OrganizationId { get; set; }
    
    public Guid PackageId { get; set; }

    public string PackageName { get; set; }
    
    public DateTime Metadata_Created { get; set; }
}