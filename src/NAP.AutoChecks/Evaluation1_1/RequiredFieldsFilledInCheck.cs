using NAP.AutoChecks.API;
using TransportDataBe.Client.Models;

namespace NAP.AutoChecks.Evaluation1_1;

public class RequiredFieldsFilledInCheck
{
    private readonly DataHandler _dataHandler;

    public RequiredFieldsFilledInCheck(DataHandler dataHandler)
    {
        _dataHandler = dataHandler;
    }

    public async Task Check()
    {
        var stakeholders = await _dataHandler.GetStakeholders();
        var organizations = await _dataHandler.GetOrganizations();
        var packages = await _dataHandler.GetPackages();

        var results = new List<RequiredFieldsFilledInResult>();
        foreach (var stakeholder in stakeholders)
        {
            if (stakeholder.ParsedOrganizationId == null)
            {
                //results.Add(new StakeholderHasPackagesResult(stakeholder, "no_organization_id"));
                continue;
            }

            // ReSharper disable once PossibleMultipleEnumeration
            var organization = organizations.FirstOrDefault(x => x.Id == stakeholder.ParsedOrganizationId);
            if (organization == null)
            {
                continue;
            }

            // ReSharper disable once PossibleMultipleEnumeration
            foreach (var package in packages.Where(x => x.Organization.Id == organization.Id))
            {
                if (!this.CheckLanguage(package, out var message))
                {
                    results.Add(new RequiredFieldsFilledInResult(_dataHandler.GetClient(), stakeholder, package, "language_not_filled_in",
                        message));
                }

                if (string.IsNullOrWhiteSpace(package.Name))
                {
                    results.Add(new RequiredFieldsFilledInResult(_dataHandler.GetClient(), stakeholder, package, "name_not_filled_in",
                        "name empty"));
                }

                if (package.Metadata_Created == null)
                {
                    results.Add(new RequiredFieldsFilledInResult(_dataHandler.GetClient(), stakeholder, package, "metadata_created_not_filled_in",
                        "metadata_created empty"));
                }

                if (package.Metadata_Modified == null)
                {
                    results.Add(new RequiredFieldsFilledInResult(_dataHandler.GetClient(), stakeholder, package,
                        "metadata_modified_not_filled_in", "metadata_modified empty"));
                }

                if (package.Temporal_Start == null)
                {
                    results.Add(new RequiredFieldsFilledInResult(_dataHandler.GetClient(), stakeholder, package, "temporal_start_not_filled_in",
                        "temporal_start empty"));
                }

                if (!this.CheckNotesTranslated(package, out message))
                {
                    results.Add(new RequiredFieldsFilledInResult(_dataHandler.GetClient(), stakeholder, package, "notes_translated_not_filled_in",
                        message));
                }

                var fluentTagsCheck = await this.CheckFluentTagsAsync(package);
                if (!fluentTagsCheck.result)
                {
                    results.Add(new RequiredFieldsFilledInResult(_dataHandler.GetClient(), stakeholder, package, "fluent_tags_not_filled_in",
                        fluentTagsCheck.message));
                }

                if (!this.CheckContRes(package, out message))
                {
                    results.Add(new RequiredFieldsFilledInResult(_dataHandler.GetClient(), stakeholder, package, "cont_res_not_filled_in",
                        message));
                }

                if (!this.CheckFrequency(package, out message))
                {
                    results.Add(new RequiredFieldsFilledInResult(_dataHandler.GetClient(), stakeholder, package, "frequency_not_filled_in",
                        message));
                }
                
                if (!this.CheckRegionsCovered(package, out message))
                {
                    results.Add(new RequiredFieldsFilledInResult(_dataHandler.GetClient(), stakeholder, package, "regions_covered_not_filled_in",
                        message));
                    continue;
                }

                if (package.Theme != "http://publications.europa.eu/resource/authority/data-theme/TRAN")
                {
                    results.Add(new RequiredFieldsFilledInResult(_dataHandler.GetClient(), stakeholder, package, "theme_not_filled_in",
                        $"Invalid theme: {package.Theme}"));
                    continue;
                }

                if (string.IsNullOrWhiteSpace(package.Publisher_Name))
                {
                    results.Add(new RequiredFieldsFilledInResult(_dataHandler.GetClient(), stakeholder, package, "publisher_name_not_filled_in",
                        $"Empty"));
                    continue;
                }

                if (string.IsNullOrWhiteSpace(package.Publisher_Email))
                {
                    results.Add(new RequiredFieldsFilledInResult(_dataHandler.GetClient(), stakeholder, package, "publisher_email_not_filled_in",
                        $"Empty"));
                    continue;
                }

                if (package.Owner_Org == null)
                {
                    results.Add(new RequiredFieldsFilledInResult(_dataHandler.GetClient(), stakeholder, package, "owner_org_not_filled_in",
                        $"Empty"));
                    continue;
                }

                if (!this.CheckContractLicense(package, out message, out var hasLicense))
                {
                    results.Add(new RequiredFieldsFilledInResult(_dataHandler.GetClient(), stakeholder, package, "contract_license_not_filled_in",
                        message));
                    continue;
                }

                if (hasLicense)
                {
                    if (string.IsNullOrWhiteSpace(package.License_Id))
                    {
                        results.Add(new RequiredFieldsFilledInResult(_dataHandler.GetClient(), stakeholder, package, "license_id_not_filled_in",
                            "No licence given but license contract set"));
                        continue;
                    }
                }

                if (package.Resources == null || package.Resources.Length == 0)
                {
                    results.Add(new RequiredFieldsFilledInResult(_dataHandler.GetClient(), stakeholder, package, "no_resource",
                        "There has be at least one resource"));
                    continue;
                }

                foreach (var resource in package.Resources)
                {
                    if (!this.CheckLanguage(resource, out message))
                    {
                        results.Add(new RequiredFieldsFilledInResult(_dataHandler.GetClient(), stakeholder, package, resource,
                            "resource_language_not_filled_in",
                            message));
                    }
                    
                    if (!this.CheckFormat(resource, out message))
                    {
                        results.Add(new RequiredFieldsFilledInResult(_dataHandler.GetClient(), stakeholder, package, resource,
                            "format_not_filled_in",
                            message));
                    }
                    
                    if (!this.CheckAccMod(resource, out message))
                    {
                        results.Add(new RequiredFieldsFilledInResult(_dataHandler.GetClient(), stakeholder, package, resource,
                            "acc_mod_not_filled_in",
                            message));
                    }
                    
                    if (!this.CheckAccInt(resource, out message))
                    {
                        results.Add(new RequiredFieldsFilledInResult(_dataHandler.GetClient(), stakeholder, package, resource,
                            "acc_int_not_filled_in",
                            message));
                    }
                    
                    if (!this.CheckAccCon(resource, out message))
                    {
                        results.Add(new RequiredFieldsFilledInResult(_dataHandler.GetClient(), stakeholder, package, resource,
                            "acc_con_not_filled_in",
                            message));
                    }

                    if (string.IsNullOrWhiteSpace(resource.Url))
                    {
                        results.Add(new RequiredFieldsFilledInResult(_dataHandler.GetClient(), stakeholder, package, resource, "url_not_filled_in",
                            "No value"));
                    }
                }
            }
        }

        await _dataHandler.WriteResultAsync("evaluation_1.1_stakeholders_with_missing_fields.xlsx", results);
    }

    private bool CheckContractLicense(Package package, out string message, out bool hasLicense)
    {
        hasLicense = false;
        var contractLicenses = _dataHandler.GetPossibleContractLicenses();

        if (package.Contract_License == null)
        {
            message = "No data found";
            return false;
        }

        var contractLicense = contractLicenses.FirstOrDefault(x => x.value == package.Contract_License);
        if (contractLicense.value == null)
        {
            message = $"Invalid value: {package.Contract_License}";
            return false;
        }

        hasLicense = contractLicense.hasLicense;
        message = string.Empty;
        return true;
    }

    private bool CheckAccCon(Resource resource, out string message)
    {
        var values = _dataHandler.GetPossibleAccCons();

        if (string.IsNullOrWhiteSpace(resource.Acc_Con))
        {
            message = "No value";
            return false;
        }

        if (!values.Contains(resource.Acc_Con))
        {
            message = $"Invalid value: {resource.Acc_Con}";
            return false;
        }

        message = string.Empty;
        return true;
    }

    private bool CheckAccInt(Resource resource, out string message)
    {
        var values = _dataHandler.GetPossibleAccInts();

        if (string.IsNullOrWhiteSpace(resource.Acc_Int))
        {
            message = "No value";
            return false;
        }

        if (!values.Contains(resource.Acc_Int))
        {
            message = $"Invalid value: {resource.Acc_Int}";
            return false;
        }

        message = string.Empty;
        return true;
    }

    private bool CheckAccMod(Resource resource, out string message)
    {
        var accMods = _dataHandler.GetPossibleAccMods();

        if (string.IsNullOrWhiteSpace(resource.Acc_Mod))
        {
            message = "No value";
            return false;
        }

        if (!accMods.Contains(resource.Acc_Mod))
        {
            message = $"Invalid value: {resource.Acc_Mod}";
            return false;
        }

        message = string.Empty;
        return true;
    }

    private bool CheckFormat(Resource resource, out string message)
    {
        var formats = _dataHandler.GetPossibleFormats();

        if (resource.Format == null)
        {
            message = "No value";
            return false;
        }

        if (!formats.Contains(resource.Format))
        {
            message = $"Invalid value: {resource.Format}";
            return false;
        }

        message = string.Empty;
        return true;
    }

    private bool CheckLanguage(Resource resource, out string message)
    {
        var languages = _dataHandler.GetPossibleLanguages();

        if (resource.Resource_Language == null)
        {
            message = "No language found";
            return false;
        }

        if (!languages.Contains(resource.Resource_Language))
        {
            message = $"Invalid language {resource.Resource_Language}";
            return false;
        }

        message = string.Empty;
        return true;
    }

    private bool CheckLanguage(Package package, out string message)
    {
        var languages = _dataHandler.GetPossibleLanguages();

        if (package.Language == null || package.Language.Length == 0)
        {
            message = "No languages found";
            return false;
        }

        foreach (var language in package.Language)
        {
            // ReSharper disable once PossibleMultipleEnumeration
            if (languages.Contains(language)) continue;

            message = $"Invalid language {language}";
            return false;
        }

        message = string.Empty;
        return true;
    }

    private bool CheckFrequency(Package package, out string message)
    {
        var values = _dataHandler.GetPossibleFrequencies();

        if (package.Frequency == null)
        {
            message = "No value found";
            return false;
        }

        if (!values.Contains(package.Frequency))
        {
            message = $"Invalid value: {package.Frequency}";
            return false;
        }

        message = string.Empty;
        return true;
    }

    private bool CheckContRes(Package package, out string message)
    {
        var contRes = _dataHandler.GetPossibleContRes();

        if (package.Cont_Res == null)
        {
            message = "No cont_res found";
            return false;
        }

        if (!contRes.Contains(package.Cont_Res))
        {
            message = $"Invalid data: {package.Cont_Res}";
            return false;
        }

        message = string.Empty;
        return true;
    }

    private bool CheckNotesTranslated(Package package, out string message)
    {
        if (package.Notes_Translated == null)
        {
            message = "No notes found";
            return false;
        }

        if (string.IsNullOrWhiteSpace(package.Notes_Translated.En) &&
            string.IsNullOrWhiteSpace(package.Notes_Translated.De) &&
            string.IsNullOrWhiteSpace(package.Notes_Translated.Fr) &&
            string.IsNullOrWhiteSpace(package.Notes_Translated.Nl))
        {
            message = "Notes all empty";
            return false;
        }

        message = string.Empty;
        return true;
    }

    private async Task<(bool result, string message)> CheckFluentTagsAsync(Package package)
    {
        var tags = await _dataHandler.GetTags();

        if (package.Fluent_Tags == null)
        {
            return (false, "No tags found");
        }

        var values = new[] { package.Fluent_Tags };
        if (package.Fluent_Tags.Contains(","))
        {
            values = package.Fluent_Tags.Split(",");
        }

        foreach (var value in values)
        {
            // ReSharper disable once PossibleMultipleEnumeration
            if (!tags.Contains(value))
            {
                return (false, $"At least one invalid value: {package.Fluent_Tags}");
            }
        }

        return (true, string.Empty);
    }

    private bool CheckRegionsCovered(Package package, out string message)
    {
        var regions = _dataHandler.GetPossibleRegions();

        if (package.Regions_Covered == null || package.Regions_Covered.Length == 0)
        {
            message = "No regions found";
            return false;
        }

        foreach (var region in package.Regions_Covered)
        {
            // ReSharper disable once PossibleMultipleEnumeration
            if (regions.Contains(region)) continue;

            message = $"Invalid region {region}";
            return false;
        }

        message = string.Empty;
        return true;
    }
}