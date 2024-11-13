using Microsoft.Extensions.Logging;
using NAP.AutoChecks.Domain;

namespace NAP.AutoChecks.API.Stakeholders._2023;

public class StakeholderLoader
{
    private readonly ILogger<Stakeholder> _logger;

    public StakeholderLoader(ILogger<Stakeholder> logger)
    {
        _logger = logger;
    }
    
    private IList<Stakeholder>? _stakeholders;

    public async Task<IEnumerable<Stakeholder>> GetStakeholders(string stakeholdersPath, DataHandler dataHandler)
    {
        if (_stakeholders != null) return _stakeholders;
        
        await using var stream =
            File.OpenRead(Path.Combine(stakeholdersPath, "CKAN-ID.csv"));
        var ckanIds = await CkanId.Load(stream);

        var stakeholders = new List<Stakeholder>();
        foreach (var ckanId in ckanIds)
        {
            if (string.IsNullOrWhiteSpace(ckanId.Organization)) continue;

            stakeholders.Add(new Stakeholder
            {
                Id = ckanId.Organization,
                OrganizationId = ckanId.OrganizationId,
                IsMMTIS = false,
                IsRTTI = false,
                IsSRTI = false,
                IsSSTP = false,
                MMTISType = null,
                Name = ckanId.Name
            });
        }
        
        await using var streamMmtis =
            File.OpenRead(Path.Combine(stakeholdersPath, "MMTIS.csv"));
        var mmtisOrgs = await NapTypeOrganization.Load(streamMmtis);
        foreach (var mmtisOrg in mmtisOrgs)
        {
            if (string.IsNullOrWhiteSpace(mmtisOrg.Organization)) continue;

            // TODO: figure out if this is a problem, field is missing now.
            MMTISType? mmtisType = null;
            // if (mmtisOrg.Type != null)
            // {
            //     if (mmtisOrg.Type.StartsWith("RA"))
            //     {
            //         mmtisType = MMTISType.TransportAuthority;
            //     }
            //     else if (mmtisOrg.Type.StartsWith("MOD"))
            //     {
            //         mmtisType = MMTISType.TransportOnDemand;
            //     }
            //     else if (mmtisOrg.Type.StartsWith("IM"))
            //     {
            //         mmtisType = MMTISType.InfrastructureManager;
            //     }
            //     else if (mmtisOrg.Type.StartsWith("PTO"))
            //     {
            //         mmtisType = MMTISType.TransportOperator;
            //     }
            // }
            
            var mmtisStakeholder = stakeholders.FirstOrDefault(x => x.Id == mmtisOrg.Organization);
            if (mmtisStakeholder == null)
            {
                mmtisStakeholder = new Stakeholder()
                {
                    Id = mmtisOrg.Organization,
                    Name = mmtisOrg.OrganizationName,
                };
                stakeholders.Add(mmtisStakeholder);
            }
            
            mmtisStakeholder.MMTISType = mmtisType;
            if (mmtisType == null)
            {
                _logger.LogWarning("Could not determine MMTISType for {Organization} - {OrganizationId}: {DataFound}",
                    mmtisStakeholder.Name, mmtisStakeholder.OrganizationId ?? "No CKAN-ID", "Field not in CSV");
            }
            mmtisStakeholder.IsMMTIS = true;
        }
        
        await using var streamRtti =
            File.OpenRead(Path.Combine(stakeholdersPath, "RTTI.csv"));
        var rttiOrgs = await NapTypeOrganization.Load(streamRtti);
        foreach (var rttiOrg in rttiOrgs)
        {
            if (string.IsNullOrWhiteSpace(rttiOrg.Organization)) continue;
            
            var mmtisStakeholder = stakeholders.FirstOrDefault(x => x.Id == rttiOrg.Organization);
            if (mmtisStakeholder == null)
            {
                mmtisStakeholder = new Stakeholder()
                {
                    Id = rttiOrg.Organization,
                    Name = rttiOrg.Organization
                };
                stakeholders.Add(mmtisStakeholder);
            }
            mmtisStakeholder.Name = rttiOrg.OrganizationName;
            mmtisStakeholder.IsRTTI = true;
        }
        
        await using var streamSrti =
            File.OpenRead(Path.Combine(stakeholdersPath, "SRTI.csv"));
        var srtiOrgs = await NapTypeOrganization.Load(streamSrti);
        foreach (var srtiOrg in srtiOrgs)
        {
            if (string.IsNullOrWhiteSpace(srtiOrg.Organization)) continue;
            
            var mmtisStakeholder = stakeholders.FirstOrDefault(x => x.Id == srtiOrg.Organization);
            if (mmtisStakeholder == null)
            {
                mmtisStakeholder = new Stakeholder()
                {
                    Id = srtiOrg.Organization,
                    Name = srtiOrg.Organization
                };
                stakeholders.Add(mmtisStakeholder);
            }
            
            mmtisStakeholder.Name = srtiOrg.OrganizationName;
            mmtisStakeholder.IsSRTI = true;
        }
        
        await using var streamSstp =
            File.OpenRead(Path.Combine(stakeholdersPath, "SSTP.csv"));
        var sstpOrgs = await NapTypeOrganization.Load(streamSstp);
        foreach (var sstpOrg in sstpOrgs)
        {
            if (string.IsNullOrWhiteSpace(sstpOrg.Organization)) continue;
            
            var mmtisStakeholder = stakeholders.FirstOrDefault(x => x.Id == sstpOrg.Organization);
            if (mmtisStakeholder == null)
            {
                mmtisStakeholder = new Stakeholder()
                {
                    Id = sstpOrg.Organization,
                    Name = sstpOrg.Organization
                };
                stakeholders.Add(mmtisStakeholder);
            }
            
            mmtisStakeholder.Name = sstpOrg.OrganizationName;
            mmtisStakeholder.IsSSTP = true;
        }
        
        await dataHandler.WriteResultAsync("stakeholders_no_nap_type.xlsx", stakeholders.Where(x => 
            x is { IsSSTP: false, IsRTTI: false, IsSRTI: false, IsMMTIS: false }).ToList());
        
        _stakeholders = stakeholders;
        return stakeholders;
    }
}