using NAP.AutoChecks.Task1.A;
using NAP.AutoChecks.Task1.B;

namespace NAP.AutoChecks.Task1;

public class Task1
{
    private readonly CheckStakeholdersRegistered _checkStakeholdersRegistered;
    private readonly CheckStakeholderHasPackages _checkStakeholderHasPackages;

    public Task1(CheckStakeholdersRegistered checkStakeholdersRegistered, CheckStakeholderHasPackages checkStakeholderHasPackages)
    {
        _checkStakeholdersRegistered = checkStakeholdersRegistered;
        _checkStakeholderHasPackages = checkStakeholderHasPackages;
    }
    
    public async Task Run()
    {
        // 1 A - check if all stakeholders have registered and out non-registered organizations.
        await _checkStakeholdersRegistered.Check();
        
        // 1 B - check if the registered stakeholders have packages.
        await _checkStakeholderHasPackages.Check();
        
        // 1 C - have completely submitted a declaration of compliance
        // TODO!
    }
}