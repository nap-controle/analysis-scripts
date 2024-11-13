namespace NAP.AutoChecks.Evaluation1_1;

public class Evaluation1_1
{
    private readonly OrganizationsNotInStakeholdersCheck _organizationsNotInStakeholdersCheck;
    private readonly StakeholdersRegisteredCheck _stakeholdersRegisteredCheck;
    private readonly StakeholderHasPackagesCheck _stakeholderHasPackagesCheck;
    private readonly RequiredFieldsFilledInCheck _requiredFieldsFilledInCheck;
    private readonly StakeholdersPackagesAfterDeadline _stakeholdersPackagesAfterDeadline;

    public Evaluation1_1(OrganizationsNotInStakeholdersCheck organizationsNotInStakeholdersCheck, StakeholdersRegisteredCheck stakeholdersRegisteredCheck, StakeholderHasPackagesCheck stakeholderHasPackagesCheck, RequiredFieldsFilledInCheck requiredFieldsFilledInCheck, StakeholdersPackagesAfterDeadline stakeholdersPackagesAfterDeadline)
    {
        _organizationsNotInStakeholdersCheck = organizationsNotInStakeholdersCheck;
        _stakeholdersRegisteredCheck = stakeholdersRegisteredCheck;
        _stakeholderHasPackagesCheck = stakeholderHasPackagesCheck;
        _requiredFieldsFilledInCheck = requiredFieldsFilledInCheck;
        _stakeholdersPackagesAfterDeadline = stakeholdersPackagesAfterDeadline;
    }
    
    public async Task Run()
    {
        // check if all organizations have at least an entry in the stakeholders list.
        await _organizationsNotInStakeholdersCheck.Check();
        
        // 1.1 A - check if all stakeholders have registered and out non-registered organizations.
        await _stakeholdersRegisteredCheck.Check();
        
        // 1.1 B - check if the registered stakeholders have packages.
        await _stakeholderHasPackagesCheck.Check();
        
        // 1.1 C - check if the stakeholders registered before the 01/12/2024 deadline.
        await _stakeholdersPackagesAfterDeadline.Check();
        
        // 1.1 D - check if the registered orgs have all fields filled in.
        await _requiredFieldsFilledInCheck.Check();
    }
}