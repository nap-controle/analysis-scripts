using NAP.AutoChecks.API;
using NAP.AutoChecks.Task1.C;

namespace NAP.AutoChecks.Task4;

public class Task4
{
    private readonly DataHandler _dataHandler;
    private readonly CheckSelfDeclarations _checkSelfDeclarations;

    public Task4(DataHandler dataHandler, CheckSelfDeclarations checkSelfDeclarations)
    {
        _dataHandler = dataHandler;
        _checkSelfDeclarations = checkSelfDeclarations;
    }

    public async Task Run()
    {
        var packages = await _dataHandler.GetPackages();

        // get stakeholders with declarations and with min 1 package.
        var withDeclarations = (await _checkSelfDeclarations.Check()).ToList();
        withDeclarations.Shuffle();

        // REMARK: this is force to be TomTom, otherwise selection fails.
        var srti = withDeclarations.First(x => x is { SRTI: true, StakeholderIsSRTI: true } && x.OrganizationId == "aa8c51bb-4da2-4b24-9d8c-19461f083627");
        withDeclarations.Remove(srti);

        var sstp = withDeclarations.First(x => x is { SSTP: true, StakeholderIsSSTP: true });
        withDeclarations.Remove(sstp);
        var rtti = withDeclarations.First(x => x is { RTTI: true, StakeholderIsRTTI: true });
        withDeclarations.Remove(rtti);
        var mmtis = withDeclarations.First(x => x is { MMTIS: true, StakeholderIsMMTIS: true });
        withDeclarations.Remove(mmtis);

        await _dataHandler.WriteResultAsync("task4-sampling.xlsx", [mmtis, rtti, sstp, srti]);
    }
}