using NAP.AutoChecks.Domain;
using NAP.AutoChecks.Evaluation2_1;
using TransportDataBe.Client.Models;

namespace NAP.AutoChecks.Evaluation2_2;

public class RandomizeOrganizationsWithDeclarationsResults
{
    /// <summary>
    /// Creates a new result.
    /// </summary>
    /// <param name="stakeholder"></param>
    /// <param name="organization"></param>
    /// <param name="packages"></param>
    /// <param name="previousSamplingDate"></param>
    public RandomizeOrganizationsWithDeclarationsResults(Stakeholder stakeholder, Organization organization, 
        IEnumerable<Package> packages, DateTime previousSamplingDate)
    {
        this.Id = stakeholder.Id;
        this.Name = stakeholder.Name;
        this.OrganizationId = stakeholder.OrganizationId;
        this.HasMMTISDeclaration = organization.HasMMTISDeclaration();
        this.HasRTTIDeclaration = organization.HasRTTIDeclaration();
        this.HasSRTIDeclaration = organization.HasSRTIDeclaration();
        this.HasSSTPDeclaration = organization.HasSSTPDeclaration();
        this.StakeholderMMTIS = stakeholder.IsMMTIS;
        this.StakeholderRTTI = stakeholder.IsRTTI;
        this.StakeholderSRTI = stakeholder.IsSRTI;
        this.StakeholderSSTP = stakeholder.IsSSTP;
        this.HasMMTISPackage = organization.HasMMTISPackage(packages);
        this.HasSRTIPackage = organization.HasSRTIPackage(packages);
        this.HasSSTPPackage = organization.HasSSTPPackage(packages);
        this.HasRTTIPackage = organization.HasRTTIPackage(packages);
        this.MMTISWasModified = organization.WasModifiedSince(packages, previousSamplingDate, x => x.IsMMTIS());
        this.SRTIWasModified = organization.WasModifiedSince(packages, previousSamplingDate, x => x.IsSRTI());
        this.SSTPWasModified = organization.WasModifiedSince(packages, previousSamplingDate, x => x.IsSSTP());
        this.RTTIWasModified = organization.WasModifiedSince(packages, previousSamplingDate, x => x.IsRTTI());
    }

    /// <summary>
    /// The id.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// The name.
    /// </summary>
    public string Name { get; set; }
    
    public bool SelectedMMTIS { get; set; }
    
    public bool SelectedSRTI { get; set; }
    
    public bool SelectedRTTI { get; set; }
    
    public bool SelectedSSTP { get; set; }
    
    /// <summary>
    /// The organization id, if any.
    /// </summary>
    public string OrganizationId { get; set; }

    public bool HasSSTPDeclaration { get; set; }

    public bool HasSRTIDeclaration { get; set; }

    public bool HasRTTIDeclaration { get; set; }

    public bool HasMMTISDeclaration { get; set; }
    
    public bool SelectedBefore { get; set; }

    public bool StakeholderSSTP { get; set; }

    public bool HasSSTPPackage { get; set; }
    
    public bool SSTPWasModified { get; set; }
    public bool StakeholderSRTI { get; set; }

    public bool HasSRTIPackage { get; set; }
    
    public bool SRTIWasModified { get; set; }

    public bool StakeholderRTTI { get; set; }

    public bool HasRTTIPackage { get; set; }
    
    public bool RTTIWasModified { get; set; }

    public bool StakeholderMMTIS { get; set; }

    public bool HasMMTISPackage { get; set; }
    
    public bool MMTISWasModified { get; set; }
    
}