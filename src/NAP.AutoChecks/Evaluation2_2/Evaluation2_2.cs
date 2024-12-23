namespace NAP.AutoChecks.Evaluation2_2;

public class Evaluation2_2
{
    private readonly RandomizeOrganizationsWithDeclarations _randomizeOrganizationsWithDeclarations;

    public Evaluation2_2(RandomizeOrganizationsWithDeclarations randomizeOrganizationsWithDeclarations)
    {
        _randomizeOrganizationsWithDeclarations = randomizeOrganizationsWithDeclarations;
    }

    public async Task Run()
    {
        await _randomizeOrganizationsWithDeclarations.Run();
    }
}