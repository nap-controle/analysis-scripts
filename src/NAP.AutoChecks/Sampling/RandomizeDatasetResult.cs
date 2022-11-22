using NAP.AutoChecks.Domain;
using TransportDataBe.Client.Models;

namespace NAP.AutoChecks.Sampling;

public class RandomizeDatasetResult
{
    public RandomizeDatasetResult(Stakeholder stakeholder, Organization organization, Package package)
    {
        this.PackageId = package.Id;
        this.PackageName = package.Name ?? string.Empty;
        this.IsRTTI = stakeholder.IsRTTI;
        this.IsSRTI = stakeholder.IsSRTI;
        this.IsSSTP = stakeholder.IsSSTP;
        this.IsMMTIS = stakeholder.IsMMTIS;
        this.MMTIStype = stakeholder.MMTISType;
        this.OrganizationId = organization.Id;
        this.OrganizationName = organization.Name;
    }
    public string OrganizationName { get; set; }
    
    public Guid OrganizationId { get; set; }
    
    public Guid PackageId { get; set; }

    public string PackageName { get; set; }
    
    public bool IsMMTIS { get; set; }
    
    public MMTISType? MMTIStype { get; set; }
    
    public bool IsRTTI { get; set; }
    
    public bool IsSRTI { get; set; }
    
    public bool IsSSTP { get; set; }
}