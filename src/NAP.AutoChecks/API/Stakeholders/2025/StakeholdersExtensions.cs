using NAP.AutoChecks.Domain;

namespace NAP.AutoChecks.API.Stakeholders._2025;

public static class StakeholdersExtensions
{
    public static void AddForType(this List<Stakeholder> stakeholders, StakeholderCsv2025 stakeholderCsv2025, NAPType type)
    {
        if (!Guid.TryParse(stakeholderCsv2025.OrganizationId, out Guid organizationId))
        {
            organizationId = Guid.Empty;
        }

        var stakeholder = stakeholders.FirstOrDefault(x => x.Id == stakeholderCsv2025.Id);
        if (stakeholder == null)
        {
            stakeholders.Add(new Stakeholder()
            {
                Id = stakeholderCsv2025.Id,
                Name = stakeholderCsv2025.Name,
                IsMMTIS = type == NAPType.MMTIS,
                IsRTTI = type == NAPType.RTTI,
                IsSRTI = type == NAPType.SRTI,
                IsSSTP = type == NAPType.SSTP,
                OrganizationId = organizationId != Guid.Empty ? organizationId.ToString() : null,
            });
        }
        else
        {
            stakeholder.IsMMTIS |= type == NAPType.MMTIS;
            stakeholder.IsRTTI |= type == NAPType.RTTI;
            stakeholder.IsSRTI |= type == NAPType.SRTI;
            stakeholder.IsSSTP |= type == NAPType.SSTP;
        }
    }
}