namespace NAP.AutoChecks.Evaluation0_DataValidation;

public class Evaluation0
{
    private readonly StakeholderWithoutNAPTypeCheck _stakeholderWithoutNapTypeCheck;
    private readonly OrganizationsNotInStakeholdersCheck _organizationsNotInStakeholdersCheck;

    public Evaluation0(StakeholderWithoutNAPTypeCheck stakeholderWithoutNapTypeCheck, OrganizationsNotInStakeholdersCheck organizationsNotInStakeholdersCheck)
    {
        _stakeholderWithoutNapTypeCheck = stakeholderWithoutNapTypeCheck;
        _organizationsNotInStakeholdersCheck = organizationsNotInStakeholdersCheck;
    }
    
    public async Task Run()
    {
        // check if all organizations have at least an entry in the stakeholders list.
        await _organizationsNotInStakeholdersCheck.Check();
        
        // check if all stakeholders have a NAP type.
        await _stakeholderWithoutNapTypeCheck.Check();
    }
}