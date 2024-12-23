using NAP.AutoChecks.API;

namespace NAP.AutoChecks.Queries;

public class AllStakeholders
{
    private readonly DataHandler _dataHandler;

    public AllStakeholders(DataHandler dataHandler)
    {
        _dataHandler = dataHandler;
    }

    public async Task Get()
    {
        var stakeholders = await _dataHandler.GetStakeholders();
        
        await _dataHandler.WriteResultAsync("stakeholders.xlsx", stakeholders);
    }
}