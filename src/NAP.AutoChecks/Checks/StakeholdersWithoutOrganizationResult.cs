using NAP.AutoChecks.Domain;

namespace NAP.AutoChecks.Checks;

public class StakeholdersWithoutOrganizationResult
{
    /// <summary>
    /// Creates a new result.
    /// </summary>
    /// <param name="stakeholder"></param>
    public StakeholdersWithoutOrganizationResult(Stakeholder stakeholder)
    {
        this.Id = stakeholder.Id;
        this.Name = stakeholder.Name;
    }
    
    /// <summary>
    /// The id.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// The name.
    /// </summary>
    public string Name { get; set; }
}