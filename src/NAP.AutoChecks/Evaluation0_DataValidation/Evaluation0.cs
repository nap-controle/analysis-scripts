namespace NAP.AutoChecks.Evaluation0_DataValidation;

public class Evaluation0
{
    private readonly StakeholderWithoutNAPTypeCheck _stakeholderWithoutNapTypeCheck;
    private readonly AllOrganizations _allOrganizations;

    public Evaluation0(StakeholderWithoutNAPTypeCheck stakeholderWithoutNapTypeCheck, AllOrganizations allOrganizations)
    {
        _stakeholderWithoutNapTypeCheck = stakeholderWithoutNapTypeCheck;
        _allOrganizations = allOrganizations;
    }
    
    public async Task Run()
    {
        // check if all organizations have at least an entry in the stakeholders list.
        await _allOrganizations.Check();
        
        // check if all stakeholders have a NAP type.
        await _stakeholderWithoutNapTypeCheck.Check();
    }
}