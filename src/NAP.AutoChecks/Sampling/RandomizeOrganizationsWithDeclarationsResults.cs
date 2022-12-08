using NAP.AutoChecks.Checks;
using NAP.AutoChecks.Domain;
using TransportDataBe.Client.Models;

namespace NAP.AutoChecks.Sampling;

public class RandomizeOrganizationsWithDeclarationsResults
{
    /// <summary>
    /// Creates a new result.
    /// </summary>
    /// <param name="stakeholder"></param>
    /// <param name="organization"></param>
    public RandomizeOrganizationsWithDeclarationsResults(Stakeholder stakeholder, Organization organization)
    {
        this.Id = stakeholder.Id;
        this.Name = stakeholder.Name;
        this.OrganizationId = stakeholder.OrganizationId;
        this.HasMMTISDeclaration = organization.HasMMTISDeclaration();
        this.HasRTTIDeclaration = organization.HasRTTIDeclaration();
        this.HasSRTIDeclaration = organization.HasSRTIDeclaration();
        this.HasSSTPDeclaration = organization.HasSSTPDeclaration();
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
}