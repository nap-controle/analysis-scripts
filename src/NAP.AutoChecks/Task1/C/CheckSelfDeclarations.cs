using NAP.AutoChecks.API;
using NAP.AutoChecks.Evaluation2_1;

namespace NAP.AutoChecks.Task1.C;

public class CheckSelfDeclarations
{
    private readonly DataHandler _dataHandler;

    public CheckSelfDeclarations(DataHandler dataHandler)
    {
        _dataHandler = dataHandler;
    }

    private List<CheckSelfDeclarationsResult>? _results;

    public async Task<IEnumerable<CheckSelfDeclarationsResult>> Check()
    {
        if (_results != null) return _results;

        var stakeholders = await _dataHandler.GetStakeholders();

        var organizations = await _dataHandler.GetOrganizations();

        var organizationsWithDeclarations = organizations
            .Where(x => x.HasRTTIDeclaration() || x.HasSRTIDeclaration() || x.HasSSTPDeclaration() ||
                                                                     x.HasMMTISDeclaration());

        _results = new List<CheckSelfDeclarationsResult>();
        foreach (var stakeholder in stakeholders)
        {
            if (stakeholder.ParsedOrganizationId == null) continue;

            // ReSharper disable once PossibleMultipleEnumeration
            var organization = organizationsWithDeclarations
                .FirstOrDefault(x => x.Id == stakeholder.ParsedOrganizationId);
            if (organization == null) continue;

            _results.Add(new CheckSelfDeclarationsResult(stakeholder, organization));
        }

        return _results;
    }
}