using NAP.AutoChecks.Domain;

namespace NAP.AutoChecks.API.Stakeholders._2023;

internal static class StakeholderLoader
{
    public static async Task<IEnumerable<Stakeholder>> GetStakeholders(string stakeholdersPath)
    {
        await using var stream =
            File.OpenRead(Path.Combine(stakeholdersPath, "CKAN-ID.csv"));
        var ckanIds = await CkanId.Load(stream);

        var stakeholders = new List<Stakeholder>();
        foreach (var ckanId in ckanIds)
        {
            MMTISType? mmtisType = ckanId.Category switch
            {
                "RA" => MMTISType.TransportAuthority,
                "MOD" => MMTISType.TransportOnDemand,
                "IM" => MMTISType.InfrastructureManager,
                "PTO" => MMTISType.TransportOperator,
                _ => null
            };

            stakeholders.Add(new Stakeholder
            {
                Id = ckanId.Organization,
                OrganizationId = ckanId.OrganizationId,
                IsMMTIS = false,
                IsRTTI = false,
                IsSRTI = false,
                IsSSTP = false,
                MMTISType = mmtisType,
                Name = ckanId.Organization
            });
        }
        
        await using var streamMmtis =
            File.OpenRead(Path.Combine(stakeholdersPath, "MMTIS.csv"));
        var mmtisOrgs = await NapTypeOrganization.Load(streamMmtis);
        foreach (var mmtisOrg in mmtisOrgs)
        {
            var mmtisStakeholder = stakeholders.FirstOrDefault(x => x.Id == mmtisOrg.Organization);
            if (mmtisStakeholder == null)
            {
                mmtisStakeholder = new Stakeholder()
                {
                    Id = mmtisOrg.Organization,
                    Name = mmtisOrg.Organization
                };
                stakeholders.Add(mmtisStakeholder);
            }
            
            mmtisStakeholder.IsMMTIS = true;
        }
        
        await using var streamRtti =
            File.OpenRead(Path.Combine(stakeholdersPath, "RTTI.csv"));
        var rttiOrgs = await NapTypeOrganization.Load(streamRtti);
        foreach (var rttiOrg in rttiOrgs)
        {
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
            
            mmtisStakeholder.IsRTTI = true;
        }
        
        await using var streamSrti =
            File.OpenRead(Path.Combine(stakeholdersPath, "SRTI.csv"));
        var srtiOrgs = await NapTypeOrganization.Load(streamSrti);
        foreach (var srtiOrg in srtiOrgs)
        {
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
            
            mmtisStakeholder.IsSRTI = true;
        }
        
        await using var streamSstp =
            File.OpenRead(Path.Combine(stakeholdersPath, "SSTP.csv"));
        var sstpOrgs = await NapTypeOrganization.Load(streamSstp);
        foreach (var sstpOrg in sstpOrgs)
        {
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
            
            mmtisStakeholder.IsSSTP = true;
        }
        
        return stakeholders;
    }
}