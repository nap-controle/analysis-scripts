using NAP.AutoChecks.Task0.Checks;

namespace NAP.AutoChecks.Task0;

public class Task0
{
    private readonly CheckOrganizationStakeholder _checkOrganizationStakeholder;

    public Task0(CheckOrganizationStakeholder checkOrganizationStakeholder)
    {
        _checkOrganizationStakeholder = checkOrganizationStakeholder;
    }

    public async Task Run()
    {
        await _checkOrganizationStakeholder.Check();
    }
}