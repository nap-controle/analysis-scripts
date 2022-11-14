using NAP.AutoChecks.Domain;

namespace NAP.AutoChecks.Queries;

public class StakeholderAndNapType
{
    public StakeholderAndNapType(Stakeholder stakeholder)
    {
        Stakeholder = stakeholder;
    }

    public Stakeholder Stakeholder { get;  }

    public bool HasMMTISDeclaration { get; set; } = false;
    
    public bool HasRTTSDeclaration { get; set; } = false;
    
    public bool HasSRTIDeclaration { get; set; } = false;

    public bool HasSSTPDeclaration { get; set; } = false;
}