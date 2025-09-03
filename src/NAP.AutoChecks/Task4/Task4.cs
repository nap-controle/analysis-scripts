using NAP.AutoChecks.API;
using NAP.AutoChecks.Task1.C;

namespace NAP.AutoChecks.Task4;

public class Task4
{
    private readonly DataHandler _dataHandler;
    private readonly CheckSelfDeclarations _checkSelfDeclarations;

    public Task4( DataHandler dataHandler, CheckSelfDeclarations checkSelfDeclarations)
    {
        _dataHandler = dataHandler;
        _checkSelfDeclarations = checkSelfDeclarations;
    }

    public async Task Run()
    {
        var packages = await _dataHandler.GetPackages();
        
        // get stakeholders with declarations and with min 1 package.
        var withDeclarations = 
            (await _checkSelfDeclarations.Check())
            .Where(x => 
                packages.Any(p => p.Organization.Id.ToString() == x.OrganizationId))
            .ToList();
        withDeclarations.Shuffle();

        var sstp = withDeclarations.First(x => x.SSTP);
        withDeclarations.Remove(sstp);
        var srti = withDeclarations.First(x => x.SRTI);
        withDeclarations.Remove(srti);
        var rtti = withDeclarations.First(x => x.RTTI);
        withDeclarations.Remove(rtti);
        var mmtis = withDeclarations.First(x => x.MMTIS);
        withDeclarations.Remove(mmtis);
        
        await _dataHandler.WriteResultAsync("task4-sampling.xlsx", [mmtis, rtti, sstp, srti]);
    }
}