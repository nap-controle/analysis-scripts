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
            if (organization == null) continue;

            if (string.IsNullOrWhiteSpace(organization.Title)) results.Add(new RequiredFieldsFilledInResult(_dataHandler.GetClient(), stakeholder,
                    "organization.title not filled in", ""));
            if (string.IsNullOrWhiteSpace(organization.Name)) results.Add(new RequiredFieldsFilledInResult(_dataHandler.GetClient(), stakeholder,
                    "organization.name not filled in", ""));
            if (string.IsNullOrWhiteSpace(organization.image_url)) results.Add(new RequiredFieldsFilledInResult(_dataHandler.GetClient(), stakeholder,
                    "organization.image_url not filled in", ""));
            if (string.IsNullOrWhiteSpace(organization.country)) results.Add(new RequiredFieldsFilledInResult(_dataHandler.GetClient(), stakeholder,
                "organization.country not filled in", ""));
            if (string.IsNullOrWhiteSpace(organization.administrative_area)) results.Add(new RequiredFieldsFilledInResult(_dataHandler.GetClient(), stakeholder,
                "organization.administrative_area not filled in", ""));
            if (string.IsNullOrWhiteSpace(organization.postal_code)) results.Add(new RequiredFieldsFilledInResult(_dataHandler.GetClient(), stakeholder,
                "organization.postal_code not filled in", ""));
            if (string.IsNullOrWhiteSpace(organization.city)) results.Add(new RequiredFieldsFilledInResult(_dataHandler.GetClient(), stakeholder,
                "organization.city not filled in", ""));
            if (string.IsNullOrWhiteSpace(organization.street_address)) results.Add(new RequiredFieldsFilledInResult(_dataHandler.GetClient(), stakeholder,
                "organization.street_address not filled in", ""));
            if (string.IsNullOrWhiteSpace(organization.do_email)) results.Add(new RequiredFieldsFilledInResult(_dataHandler.GetClient(), stakeholder,
                "organization.do_email not filled in", ""));
            if (string.IsNullOrWhiteSpace(organization.do_tel)) results.Add(new RequiredFieldsFilledInResult(_dataHandler.GetClient(), stakeholder,
                "organization.do_tel not filled in", ""));
            if (organization.agreement_declaration_mmtis == null || organization.agreement_declaration_mmtis.Length == 0 ||
                string.IsNullOrWhiteSpace(organization.agreement_declaration_mmtis[0]))
            {
                results.Add(new RequiredFieldsFilledInResult(_dataHandler.GetClient(), stakeholder,
                    "organization.agreement_declaration_mmtis not filled in", ""));
            }

            // ReSharper disable once PossibleMultipleEnumeration
            foreach (var package in packages.Where(x => x.Organization.Id == organization.Id))
            {
                if (string.IsNullOrWhiteSpace(package.Name))
                {
                    results.Add(new RequiredFieldsFilledInResult(_dataHandler.GetClient(), stakeholder, package, "name_not_filled_in",
                        "name empty"));
                }

                if (string.IsNullOrWhiteSpace(package.Title))
                {
                    results.Add(new RequiredFieldsFilledInResult(_dataHandler.GetClient(), stakeholder, package, "name_not_filled_in",
                        "name empty"));
                }

                if (!this.CheckLanguage(package, out var message))
                {
                    results.Add(new RequiredFieldsFilledInResult(_dataHandler.GetClient(), stakeholder, package, "language_not_filled_in",
                        message));
                }

                if (!this.CheckMobilityTheme(package, out message))
                {
                    results.Add(new RequiredFieldsFilledInResult(_dataHandler.GetClient(), stakeholder, package, "mobility_theme_error",
                        message));
                }

                if (!this.CheckNotesTranslated(package, out message))
                {
                    results.Add(new RequiredFieldsFilledInResult(_dataHandler.GetClient(), stakeholder, package, "notes_translated_not_filled_in",
                        message));
                }

                if (!this.CheckContRes(package, out message))
                {
                    results.Add(new RequiredFieldsFilledInResult(_dataHandler.GetClient(), stakeholder, package, "cont_res_not_filled_in",
                        message));
                }

                if (package.Owner_Org == null)
                {
                    results.Add(new RequiredFieldsFilledInResult(_dataHandler.GetClient(), stakeholder, package, "owner_org_not_filled_in",
                        $"Empty"));
                    continue;
                }

                if (package.Private)
                {
                    results.Add(new RequiredFieldsFilledInResult(_dataHandler.GetClient(), stakeholder, package, "package_is_private",
                        $"Empty"));
                    continue;
                }

                if (string.IsNullOrWhiteSpace(package.contact_point_name))
                {
                    results.Add(new RequiredFieldsFilledInResult(_dataHandler.GetClient(), stakeholder, package, "contact_point_name_empty",
                        $"Empty"));
                    continue;
                }

                if (string.IsNullOrWhiteSpace(package.contact_point_email))
                {
                    results.Add(new RequiredFieldsFilledInResult(_dataHandler.GetClient(), stakeholder, package, "contact_point_email_empty",
                        $"Empty"));
                    continue;
                }

                if (string.IsNullOrWhiteSpace(package.publisher_firstname))
                {
                    results.Add(new RequiredFieldsFilledInResult(_dataHandler.GetClient(), stakeholder, package, "publisher_firstname_empty",
                        $"Empty"));
                    continue;
                }

                if (string.IsNullOrWhiteSpace(package.publisher_surname))
                {
                    results.Add(new RequiredFieldsFilledInResult(_dataHandler.GetClient(), stakeholder, package, "publisher_surname_empty",
                        $"Empty"));
                    continue;
                }

                if (!this.CheckCountries(package, out message))
                {
                    results.Add(new RequiredFieldsFilledInResult(_dataHandler.GetClient(), stakeholder, package, "countries_covered_error",
                        message));
                }

                if (!this.CheckRegionsCovered(package, out message))
                {
                    results.Add(new RequiredFieldsFilledInResult(_dataHandler.GetClient(), stakeholder, package, "regions_covered_not_filled_in",
                        message));
                    continue;
                }

                if (!this.CheckFluentTagsAsync(package, out message))
                {
                    results.Add(new RequiredFieldsFilledInResult(_dataHandler.GetClient(), stakeholder, package, "fluent_tags_not_filled_in",
                        message));
                }

                if (!this.CheckFrequency(package, out message))
                {
                    results.Add(new RequiredFieldsFilledInResult(_dataHandler.GetClient(), stakeholder, package, "frequency_not_filled_in",
                        message));
                }

                // THESE FIELDS ARE CHECK BECAUSE THE CB NEEDS THEM:

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


                if (package.Resources == null || package.Resources.Length == 0)
                {
                    results.Add(new RequiredFieldsFilledInResult(_dataHandler.GetClient(), stakeholder, package, "no_resource",
                        "There has be at least one resource"));
                    continue;
                }

                foreach (var resource in package.Resources)
                {
                    if (string.IsNullOrWhiteSpace(resource.Url))
                    {
                        results.Add(new RequiredFieldsFilledInResult(_dataHandler.GetClient(), stakeholder, package, resource,
                            "resource_url_not_filled_in",
                            message));
                    }

                    if (string.IsNullOrWhiteSpace(resource.Name))
                    {
                        results.Add(new RequiredFieldsFilledInResult(_dataHandler.GetClient(), stakeholder, package, resource,
                            "resource_name_not_filled_in",
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

                    // if (!this.CheckAccCon(resource, out message))
                    // {
                    //     results.Add(new RequiredFieldsFilledInResult(_dataHandler.GetClient(), stakeholder, package, resource,
                    //         "acc_con_not_filled_in",
                    //         message));
                    // }

                    if (!this.CheckConditionsAccess(resource, out message))
                    {
                        results.Add(new RequiredFieldsFilledInResult(_dataHandler.GetClient(), stakeholder, package, resource,
                            "conditions_access_not_filled_in",
                            message));
                    }

                    if (!this.CheckConditionsUsage(resource, out message))
                    {
                        results.Add(new RequiredFieldsFilledInResult(_dataHandler.GetClient(), stakeholder, package, resource,
                            "conditions_usage_not_filled_in",
                            message));
                    }

                    if (!this.CheckLicenseType(resource, out message))
                    {
                        results.Add(new RequiredFieldsFilledInResult(_dataHandler.GetClient(), stakeholder, package, resource,
                            "license_type_error",
                            message));
                    }

                    if (!this.CheckLicenseTextTranslated(resource, out message))
                    {
                        results.Add(new RequiredFieldsFilledInResult(_dataHandler.GetClient(), stakeholder, package, resource,
                            "license_text_translated",
                            message));
                    }
                }
            }
        }

        await _dataHandler.WriteResultAsync("evaluation_1.1_stakeholders_with_missing_fields.xlsx", results);
    }
    //
    // private bool CheckAccCon(Resource resource, out string message)
    // {
    //     var values = _dataHandler.GetPossibleAccCons();
    //
    //     if (string.IsNullOrWhiteSpace(resource.Acc_Con))
    //     {
    //         message = "No value";
    //         return false;
    //     }
    //
    //     if (!values.Contains(resource.Acc_Con))
    //     {
    //         message = $"Invalid value: {resource.Acc_Con}";
    //         return false;
    //     }
    //
    //     message = string.Empty;
    //     return true;
    // }

    private bool CheckAccInt(Resource resource, out string message)
    {
        if (string.IsNullOrWhiteSpace(resource.Acc_Int))
        {
            message = "No value";
            return false;
        }

        if (!DataHandler.PossibleApplicationLayerProtocols.Contains(resource.Acc_Int))
        {
            message = $"Invalid value: {resource.Acc_Int}";
            return false;
        }

        message = string.Empty;
        return true;
    }

    private bool CheckConditionsAccess(Resource resource, out string message)
    {
        if (string.IsNullOrWhiteSpace(resource.conditions_access))
        {
            message = "No value";
            return false;
        }

        if (!DataHandler.PossibleAccessConditions.Contains(resource.conditions_access))
        {
            message = $"Invalid value: {resource.conditions_access}";
            return false;
        }

        message = string.Empty;
        return true;
    }

    private bool CheckConditionsUsage(Resource resource, out string message)
    {
        if (string.IsNullOrWhiteSpace(resource.conditions_usage))
        {
            message = "No value";
            return false;
        }

        if (!DataHandler.PossibleUsageConditions.Contains(resource.conditions_usage))
        {
            message = $"Invalid value: {resource.conditions_usage}";
            return false;
        }

        message = string.Empty;
        return true;
    }

    private bool CheckLicenseType(Resource resource, out string message)
    {
        message = string.Empty;
        if (resource.conditions_usage ==
            "https://w3id.org/mobilitydcat-ap/conditions-for-access-and-usage/licence-provided") return true;

        if (string.IsNullOrWhiteSpace(resource.license_type))
        {
            message = "No value";
            return false;
        }

        if (!DataHandler.PossibleLicenseTypes.Contains(resource.license_type))
        {
            message = $"Invalid value: {resource.license_type}";
            return false;
        }

        message = string.Empty;
        return true;
    }

    private bool CheckLicenseTextTranslated(Resource resource, out string message)
    {
        message = string.Empty;
        if (resource.license_type !=
            "Other") return true;
        if (resource.conditions_usage !=
            "https://w3id.org/mobilitydcat-ap/conditions-for-access-and-usage/licence-provided") return true;

        throw new NotImplementedException();

        // if (string.IsNullOrWhiteSpace(resource.license_text_translated))
        // {
        //     message = "No value";
        //     return false;
        // }
        //
        // if (!DataHandler.PossibleLicenseTypes.Contains(resource.license_text_translated))
        // {
        //     message = $"Invalid value: {resource.license_text_translated}";
        //     return false;
        // }

        message = string.Empty;
        return true;
    }

    private bool CheckAccMod(Resource resource, out string message)
    {
        if (string.IsNullOrWhiteSpace(resource.Acc_Mod))
        {
            message = "No value";
            return false;
        }

        if (!DataHandler.PossibleDataModels.Contains(resource.Acc_Mod))
        {
            message = $"Invalid value: {resource.Acc_Mod}";
            return false;
        }

        message = string.Empty;
        return true;
    }

    private bool CheckFormat(Resource resource, out string message)
    {
        var formats = DataHandler.PossibleFormats;

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

    private bool CheckMobilityTheme(Package package, out string message)
    {
        var mobilityThemes = _dataHandler.GetMobilityThemes();

        if (string.IsNullOrEmpty(package.mobility_theme))
        {
            message = "No themes found";
            return false;
        }

        throw new NotImplementedException();

        // foreach (var theme in package.mobility_theme)
        // {
        //     if (mobilityThemes.ContainsKey(theme.Key)) continue;
        //
        //     message = $"Invalid theme {theme.Key}";
        //     return false;
        // }

        message = string.Empty;
        return true;
    }

    private bool CheckCountries(Package package, out string message)
    {
        var countries = DataHandler.PossibleCountries;

        if (package.countries_covered == null || package.countries_covered.Length == 0)
        {
            message = "No countries found";
            return false;
        }

        foreach (var country in package.countries_covered)
        {
            if (countries.ContainsKey(country)) continue;

            message = $"Invalid country {country}";
            return false;
        }

        message = string.Empty;
        return true;
    }

    private bool CheckFrequency(Package package, out string message)
    {
        var values = DataHandler.PossibleFrequencies;

        if (package.Frequency == null)
        {
            message = "No value found";
            return false;
        }

        if (!values.ContainsKey(package.Frequency))
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

    private bool CheckFluentTagsAsync(Package package, out string message)
    {
        var tags = DataHandler.PossibleFluentTags;

        if (string.IsNullOrEmpty(package.Fluent_Tags))
        {
            message = "No tags found";
            return false;
        }
        var invalidValues = new HashSet<string>();

        throw new NotImplementedException();

        // foreach (var value in package.Fluent_Tags)
        // {
        //     // ReSharper disable once PossibleMultipleEnumeration
        //     if (!tags.Contains(value))
        //     {
        //         invalidValues.Add(value);
        //     }
        // }

        if (invalidValues.Count > 0)
        {
            message = $"At least one invalid value: {string.Join(",", invalidValues)}";
            return false;
        }

        message = string.Empty;
        return true;
    }

    private bool CheckRegionsCovered(Package package, out string message)
    {
        var regions = DataHandler.PossibleRegions;

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