namespace NAP.AutoChecks.Evaluation1_1;

public class Evaluation1_1
{
    private readonly StakeholdersRegisteredCheck _stakeholdersRegisteredCheck;
    private readonly StakeholderHasPackagesCheck _stakeholderHasPackagesCheck;
    private readonly RequiredFieldsFilledInCheck _requiredFieldsFilledInCheck;
    private readonly StakeholdersPackagesAfterDeadline _stakeholdersPackagesAfterDeadline;

    public Evaluation1_1(StakeholdersRegisteredCheck stakeholdersRegisteredCheck, StakeholderHasPackagesCheck stakeholderHasPackagesCheck, RequiredFieldsFilledInCheck requiredFieldsFilledInCheck, StakeholdersPackagesAfterDeadline stakeholdersPackagesAfterDeadline)
    {
        _stakeholdersRegisteredCheck = stakeholdersRegisteredCheck;
        _stakeholderHasPackagesCheck = stakeholderHasPackagesCheck;
        _requiredFieldsFilledInCheck = requiredFieldsFilledInCheck;
        _stakeholdersPackagesAfterDeadline = stakeholdersPackagesAfterDeadline;
    }
    
    public async Task Run()
    {
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