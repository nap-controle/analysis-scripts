using Microsoft.Extensions.Logging;
using NAP.AutoChecks.Domain;

namespace NAP.AutoChecks.API.Stakeholders._2025;

public class StakeholderLoader2025
{
    private readonly ILogger<StakeholderLoader2025> _logger;

    public StakeholderLoader2025(ILogger<StakeholderLoader2025> logger)
    {
        _logger = logger;
    }
    
    private IList<Stakeholder>? _stakeholders;

    public async Task<IEnumerable<Stakeholder>> GetStakeholders(string stakeholdersPath)
    {
        if (_stakeholders != null) return _stakeholders;
        
        await using var mmtisStream =
            File.OpenRead(Path.Combine(stakeholdersPath, "MMTIS.csv"));
        var mmtisStakeholders = await StakeholderCsv2025.Load(mmtisStream);
        await using var rttiStream =
            File.OpenRead(Path.Combine(stakeholdersPath, "RTTI.csv"));
        var rttiStakeholders = await StakeholderCsv2025.Load(rttiStream);
        await using var srtiStream =
            File.OpenRead(Path.Combine(stakeholdersPath, "SRTI.csv"));
        var srtiStakeholders = await StakeholderCsv2025.Load(srtiStream);
        await using var sstpStream =
            File.OpenRead(Path.Combine(stakeholdersPath, "SSTP.csv"));
        var sstpStakeholders = await StakeholderCsv2025.Load(sstpStream);

        var stakeholders = new List<Stakeholder>();
        foreach (var stakeholderCsv2025 in mmtisStakeholders)
        {
            stakeholders.AddForType(stakeholderCsv2025, NAPType.MMTIS);
        }
        foreach (var stakeholderCsv2025 in rttiStakeholders)
        {
            stakeholders.AddForType(stakeholderCsv2025, NAPType.RTTI);
        }
        foreach (var stakeholderCsv2025 in srtiStakeholders)
        {
            stakeholders.AddForType(stakeholderCsv2025, NAPType.SRTI);
        }
        foreach (var stakeholderCsv2025 in sstpStakeholders)
        {
            stakeholders.AddForType(stakeholderCsv2025, NAPType.SSTP);
        }
        return stakeholders;
    }
}