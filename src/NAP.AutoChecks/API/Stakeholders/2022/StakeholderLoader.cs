using NAP.AutoChecks.Domain;
using Serilog;
using Serilog.Core;

namespace NAP.AutoChecks.API.Stakeholders._2022;

internal static class StakeholderLoader
{
    public static async Task<IEnumerable<Stakeholder>> GetStakeholders(string dataPath)
    {
        await using var stream =
            File.OpenRead(Path.Combine(dataPath, "stakeholders", "organisations.csv"));
        var stakeholders = await Stakeholder.LoadFromCsv(stream);
        
        var stakeholdersMmtis = await Csv.ReadAsync<Stakeholder_MMTIS>(
            Path.Combine(dataPath, "stakeholders", "organisations_MMTIS.csv"));
        foreach (var stakeholderMmtis in stakeholdersMmtis)
        {
            var stakeholder = stakeholders.FirstOrDefault(x => x.Id == stakeholderMmtis.Id);
            if (stakeholder == null) continue;

            stakeholder.IsMMTIS = stakeholderMmtis.IsMMTIS == "Yes";
        }
        
        var rttis = await Csv.ReadAsync<Stakeholder_RTTI>(
            Path.Combine(dataPath, "stakeholders", "organisations_RTTI.csv"));
        foreach (var rtti in rttis)
        {
            var stakeholder = stakeholders.FirstOrDefault(x => x.Id == rtti.Id);
            if (stakeholder == null) continue;

            stakeholder.IsRTTI = rtti.IsRTTI == "Yes";
        }
        
        var srtis = await Csv.ReadAsync<Stakeholder_SRTI>(
            Path.Combine(dataPath, "stakeholders", "organisations_SRTI.csv"));
        foreach (var srti in srtis)
        {
            var stakeholder = stakeholders.FirstOrDefault(x => x.Id == srti.Id);
            if (stakeholder == null) continue;

            stakeholder.IsSRTI = srti.IsSRTI == "Yes";
        }
        
        var sstps = await Csv.ReadAsync<Stakeholder_SSTP>(
            Path.Combine(dataPath, "stakeholders", "organisations_SSTP.csv"));
        foreach (var sstp in sstps)
        {
            var stakeholder = stakeholders.FirstOrDefault(x => x.Id == sstp.Id);
            if (stakeholder == null) continue;

            stakeholder.IsSSTP = sstp.IsSSTP == "Yes";
        }
        
        var mmtisCategories = await Csv.ReadAsync<CategorizedOrganization_MMTIS>(
            Path.Combine(dataPath, "stakeholders", "organizations_mmtis_categories.csv"));
        foreach (var categorizedMmtis in mmtisCategories)
        {
            var stakeholder = stakeholders.FirstOrDefault(x => x.OrganizationId == categorizedMmtis.OrganizationId);
            if (stakeholder == null) continue;

            if (!string.IsNullOrWhiteSpace(categorizedMmtis.IsTransportAuthority))
            {
                stakeholder.MMTISType = MMTISType.TransportAuthority;
            }
            else if (!string.IsNullOrWhiteSpace(categorizedMmtis.IsTransportOperator))
            {
                stakeholder.MMTISType = MMTISType.TransportOperator;
            }
            else if(!string.IsNullOrWhiteSpace(categorizedMmtis.IsInfrastructureManager))
            {
                stakeholder.MMTISType = MMTISType.InfrastructureManager;
            }
            else if(!string.IsNullOrWhiteSpace(categorizedMmtis.IsTransportondemandserviceprovider))
            {
                stakeholder.MMTISType = MMTISType.TransportOnDemand;
            }
        }
        
        var extraRegistrations = await Csv.ReadAsync<Stakeholder_Registrations>(
            Path.Combine(dataPath, "stakeholders", "organizations_registrations.csv"));
        foreach (var extraRegistration in extraRegistrations)
        {
            var stakeholder = stakeholders.FirstOrDefault(x => x.Id == extraRegistration.Id);
            if (stakeholder == null) continue;

            if (!string.IsNullOrWhiteSpace(stakeholder.OrganizationId) && 
                Guid.TryParse(stakeholder.OrganizationId, out _))
            {
                Log.Logger.Warning("Stakeholder {Id} - {Name} from extra registration list already has an organization: {Existing}",
                    stakeholder.Id, stakeholder.Name, stakeholder.OrganizationId);
                continue;
            }
            
            Log.Logger.Warning("Stakeholder {Id} - {Name} has no registration but does exist as an organization: {Existing}",
                stakeholder.Id, stakeholder.Name, extraRegistration.OrganizationId);
            stakeholder.OrganizationId = extraRegistration.OrganizationId;
        }
        
        return stakeholders;
    }
}