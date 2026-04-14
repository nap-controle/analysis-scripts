using ClosedXML.Excel;
using NAP.AutoChecks.API;
using NAP.AutoChecks.Domain;
using NAP.AutoChecks.Evaluation2_1;
using NAP.AutoChecks.Task1.A;
using NAP.AutoChecks.Task1.B;
using NAP.AutoChecks.Task1.C;

namespace NAP.AutoChecks.Task1;

public class Task1
{
    private readonly CheckStakeholdersRegistered _checkStakeholdersRegistered;
    private readonly CheckStakeholderHasPackages _checkStakeholderHasPackages;
    private readonly CheckSelfDeclarations _checkSelfDeclarations;
    private readonly DataHandler _dataHandler;

    public Task1(CheckStakeholdersRegistered checkStakeholdersRegistered, CheckStakeholderHasPackages checkStakeholderHasPackages, DataHandler dataHandler, CheckSelfDeclarations checkSelfDeclarations)
    {
        _checkStakeholdersRegistered = checkStakeholdersRegistered;
        _checkStakeholderHasPackages = checkStakeholderHasPackages;
        _dataHandler = dataHandler;
        _checkSelfDeclarations = checkSelfDeclarations;
    }

    public async Task Run()
    {
        // // 1 A - check if all stakeholders have registered and out non-registered organizations.
        // var registeredResults = await _checkStakeholdersRegistered.Check();
        // await _dataHandler.WriteResultAsync("task1-stakeholders_registration_status.xlsx", registeredResults);
        //
        // // 1 B - check if the registered stakeholders have packages.
        // var hasPackages = await _checkStakeholderHasPackages.Check();
        // await _dataHandler.WriteResultAsync("task1-stakeholder_has_packages.xlsx", hasPackages);
        //
        // // 1 C - have completely submitted a declaration of compliance
        // var selfDeclared = await _checkSelfDeclarations.Check();
        // await _dataHandler.WriteResultAsync("task1-stakeholders_with_declarations.xlsx", selfDeclared);

        var registry = Excel.Read("/Users/xivk/ANYWAYS Cloud/projects/Y999-NAP3/2025/work/task 1 - stakeholders list checks/Registry of processed self-declaration_2025.xlsx");
        
        await this.CheckForNapType(registry, NAPType.MMTIS);
        await this.CheckForNapType(registry, NAPType.SSTP);
        await this.CheckForNapType(registry, NAPType.SRTI);
        await this.CheckForNapType(registry, NAPType.RTTI);

        registry.SaveAs("/Users/xivk/ANYWAYS Cloud/projects/Y999-NAP3/2025/work/task 1 - stakeholders list checks/Registry of processed self-declaration_2025_results.xlsx");
    }

    private string SheetTypeForNapType(NAPType napType)
    {
        switch (napType)
        {
            case NAPType.MMTIS:
                return "MMTIS";
            case NAPType.SSTP:
                return "SSTP";
            case NAPType.RTTI:
                return "RTTI";
            case NAPType.SRTI:
                return "SRTI";
        }

        throw new Exception();
    }

    private async Task CheckForNapType(XLWorkbook registry, NAPType napType)
    {
        var organizations = (await _dataHandler.GetOrganizations()).ToList();
        var packages = (await _dataHandler.GetPackages()).ToList();

        var sheetName = this.SheetTypeForNapType(napType);
        var sheet = registry.Worksheets.Worksheet(sheetName) ?? throw new Exception();
        var row = 1;
        while (true)
        {
            row++;
            
            var organization = sheet.Cell(row, "C").GetString();
            if (string.IsNullOrWhiteSpace(organization)) break;
            
            if (!Guid.TryParse(organization, out var organizationGuid)) continue;
            
            var o = organizations.FirstOrDefault(x => x.Id == organizationGuid);
            if (o == null) continue;
            
            var hasPackage = packages.Any(x => x.Organization.Id == organizationGuid && 
                                               x.IsNapType(napType));
            if (hasPackage)
            {
                sheet.Cell(row, "D").SetValue("YES");
            }
            else
            {
                sheet.Cell(row, "D").SetValue("NO");
            }

            if (o.HasSelfDeclaration(napType))
            {
                sheet.Cell(row, "E").SetValue("2025");
            }
            else
            {
                sheet.Cell(row, "E").SetValue("NO SD");
            }
        }
    }
}