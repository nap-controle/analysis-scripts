namespace NAP.AutoChecks.Evaluation2_1;

public class Evaluation2_1
{
    private readonly StakeholdersWithDeclarations _stakeholdersWithDeclarations;
    private readonly StakeholdersWithoutDeclarations _stakeholdersWithoutDeclarations;

    public Evaluation2_1(StakeholdersWithDeclarations stakeholdersWithDeclarations, StakeholdersWithoutDeclarations stakeholdersWithoutDeclarations)
    {
        _stakeholdersWithDeclarations = stakeholdersWithDeclarations;
        _stakeholdersWithoutDeclarations = stakeholdersWithoutDeclarations;
    }

    public async Task Run()
    {
        await _stakeholdersWithoutDeclarations.Check();
        
        await _stakeholdersWithDeclarations.Run();
    }
}