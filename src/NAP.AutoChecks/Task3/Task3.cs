using NAP.AutoChecks.API;
using NAP.AutoChecks.Task3._2025;

namespace NAP.AutoChecks.Task3;

public class Task3
{
    private readonly RandomSelection _randomSelection;
    private readonly DataHandler _dataHandler;

    public Task3(RandomSelection randomSelection, DataHandler dataHandler)
    {
        _randomSelection = randomSelection;
        _dataHandler = dataHandler;
    }

    public async Task Run()
    {
        var selection = await _randomSelection.Run();
        
        await _dataHandler.WriteResultAsync("task3-sampling.xlsx", selection);
    }
}