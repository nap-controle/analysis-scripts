using NAP.AutoChecks.API;
using NAP.AutoChecks.Task1.A;
using NAP.AutoChecks.Task1.B;
using NAP.AutoChecks.Task1.C;

namespace NAP.AutoChecks.Task1;

public class Task1
{
    private readonly CheckStakeholdersRegistered _checkStakeholdersRegistered;
    private readonly CheckStakeholderHasPackages _checkStakeholderHasPackages;
    private readonly CheckSelfDeclarations _checkSelfDeclarations;
    private readonly DataHandler _dataHandler;

    public Task1(CheckStakeholdersRegistered checkStakeholdersRegistered, CheckStakeholderHasPackages checkStakeholderHasPackages, DataHandler dataHandler, CheckSelfDeclarations checkSelfDeclarations)
    {
        _checkStakeholdersRegistered = checkStakeholdersRegistered;
        _checkStakeholderHasPackages = checkStakeholderHasPackages;
        _dataHandler = dataHandler;
        _checkSelfDeclarations = checkSelfDeclarations;
    }

    public async Task Run()
    {
        // 1 A - check if all stakeholders have registered and out non-registered organizations.
        var registeredResults = await _checkStakeholdersRegistered.Check();
        await _dataHandler.WriteResultAsync("task1-stakeholders_registration_status.xlsx", registeredResults);

        // 1 B - check if the registered stakeholders have packages.
        var hasPackages = await _checkStakeholderHasPackages.Check();
        await _dataHandler.WriteResultAsync("task1-stakeholder_has_packages.xlsx", hasPackages);

        // 1 C - have completely submitted a declaration of compliance
        var selfDeclared = await _checkSelfDeclarations.Check();
        await _dataHandler.WriteResultAsync("task1-stakeholders_with_declarations.xlsx", selfDeclared);
    }
}