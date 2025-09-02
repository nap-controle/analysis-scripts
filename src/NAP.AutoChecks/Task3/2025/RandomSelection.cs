using Microsoft.Extensions.Logging;
using NAP.AutoChecks.API;
using NAP.AutoChecks.Domain;
using NAP.AutoChecks.Evaluation1_2;

namespace NAP.AutoChecks.Task3._2025;

public class RandomSelection
{
    private readonly DataHandler _dataHandler;
    private readonly ILogger<RandomSelection> _logger;

    public RandomSelection(DataHandler dataHandler, ILogger<RandomSelection> logger)
    {
        _dataHandler = dataHandler;
        _logger = logger;
    }

    public async Task Run()
    {
        var packages = (await _dataHandler.GetPackages()).ToList();

        var selectedBefore = new Dictionary<Guid, HashSet<NAPType>>();

        var pools = new[]
        {
            RandomSelectionPool.CreateFrom(packages, NAPType.MMTIS, 15, new HashSet<Guid>()),
            RandomSelectionPool.CreateFrom(packages, NAPType.RTTI, 5, new HashSet<Guid>()),
            RandomSelectionPool.CreateFrom(packages, NAPType.SRTI, 5, new HashSet<Guid>()),
            RandomSelectionPool.CreateFrom(packages, NAPType.SSTP, 5, new HashSet<Guid>()),
        };

        // move extra capacity to the largest pool.
        var biggestPool = pools.OrderByDescending(x => x.Quota).First();
        foreach (var pool in pools)
        {
            if (pool.NAPType == biggestPool.NAPType) continue;
            if (pool.IsFull) continue;

            biggestPool.IncreaseQuota(pool.Free);
        }

        // move duplicate datasets to the biggest pool.
        foreach (var pool in pools)
        {
            if (pool.NAPType == biggestPool.NAPType) continue;

            var selectedInPool = pool.GetSelected();
            var selectedInOtherPools = pools
                .Where(x => x.NAPType != biggestPool.NAPType && x.NAPType != pool.NAPType)
                .SelectMany(x => x.GetSelected())
                .ToDictionary(x => x.Id);
            foreach (var package in selectedInPool)
            {
                if (selectedInOtherPools.ContainsKey(package.Id))
                {
                    biggestPool.IncreaseQuota(1);
                }
            }
        }
    }
}