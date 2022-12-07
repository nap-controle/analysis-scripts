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
        this.StakeholderMMTIS = stakeholder.IsMMTIS;
        this.StakeholderRTTI = stakeholder.IsRTTI;
        this.StakeholderSRTI = stakeholder.IsSRTI;
        this.StakeholderSSTP = stakeholder.IsSSTP;
    }

    /// <summary>
    /// The id.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// The name.
    /// </summary>
    public string Name { get; set; }

    public bool StakeholderSSTP { get; set; }

    public bool StakeholderSRTI { get; set; }

    public bool StakeholderRTTI { get; set; }

    public bool StakeholderMMTIS { get; set; }
}