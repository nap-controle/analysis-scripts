namespace NAP.AutoChecks.Evaluation1_1;

public class Evaluation1_1
{
    private readonly OrganizationsNotInStakeholdersCheck _organizationsNotInStakeholdersCheck;
    private readonly StakeholdersRegisteredCheck _stakeholdersRegisteredCheck;
    private readonly StakeholderHasPackagesCheck _stakeholderHasPackagesCheck;
    private readonly RequiredFieldsFilledInCheck _requiredFieldsFilledInCheck;

    public Evaluation1_1(OrganizationsNotInStakeholdersCheck organizationsNotInStakeholdersCheck, StakeholdersRegisteredCheck stakeholdersRegisteredCheck, StakeholderHasPackagesCheck stakeholderHasPackagesCheck, RequiredFieldsFilledInCheck requiredFieldsFilledInCheck)
    {
        _organizationsNotInStakeholdersCheck = organizationsNotInStakeholdersCheck;
        _stakeholdersRegisteredCheck = stakeholdersRegisteredCheck;
        _stakeholderHasPackagesCheck = stakeholderHasPackagesCheck;
        _requiredFieldsFilledInCheck = requiredFieldsFilledInCheck;
    }
    
    public async Task Run()
    {
        // check if all organizations have at least an entry in the stakeholders list.
        await _organizationsNotInStakeholdersCheck.Check();
        
        // 1.1 A - check if all stakeholders have registered and out non-registered organizations.
        await _stakeholdersRegisteredCheck.Check();
        
        // 1.1 B - check if the registered stakeholders have packages.
        await _stakeholderHasPackagesCheck.Check();
        
        // 1.1 D - check if the registered orgs have all fields filled in.
        await _requiredFieldsFilledInCheck.Check();
    }
}