library("rjson")

# data path, by default based on today's date
# REMARK: assumes the working directory is the root of the repo
today<-format(Sys.Date(), format="%Y-%m-%d")
data_path<-file.path("data", today)

# load all packages
packages_path<-file.path(data_path, "packages")
package_files<-list.files(file.path(data_path, "packages"))

# keep result.
organizations_with_invalid_packages = data.frame(Characters=character(), Characters=character(), Characters=character(), Characters=character(), Characters=character())
names(organizations_with_invalid_packages) <- c("organization_id", "organization_name", "package_id", "package_name", "Error")

# per organization, check all their packages.
for (i in 1:length(organizations$result)) {   
  organization_name<-organizations$result[i]
  organization <- fromJSON(file=file.path(organizations_path, paste(organization_name, ".json", sep="")))
  organization_id = organization$result$id
  
  for (i in 1:length(package_files)) {
    package <- fromJSON(file = file.path(packages_path, package_files[i]))
    
    if (package$result$organization$id != organization_id) {
      next
    }
    
    # check language
    language<-package$result$language
    if (nchar(language) == 0 || (language != "http://publications.europa.eu/resource/authority/language/FRA" && 
                                  language != "http://publications.europa.eu/resource/authority/language/NLD" &&
                                  language != "http://publications.europa.eu/resource/authority/language/ENG")) {
      organizations_with_invalid_packages[nrow(organizations_with_invalid_packages) + 1,] = 
        c(organization_id, organization_name, package$result$id, package$result$name, "language not filled in")
    }
    
    # check name
    name<-package$result$name
    if (nchar(name) == 0) {
      organizations_with_invalid_packages[nrow(organizations_with_invalid_packages) + 1,] = 
        c(organization_id, organization_name, package$result$id, package$result$name, "name not filled in")
    }
    
    # check metadata_created
    metadata_created<-package$result$metadata_created
    if (nchar(metadata_created) == 0) {
      organizations_with_invalid_packages[nrow(organizations_with_invalid_packages) + 1,] = 
        c(organization_id, organization_name, package$result$id, package$result$name, "metadata_created not filled in")
    }
    
    # check metadata_modified
    metadata_modified<-package$result$metadata_modified
    if (nchar(metadata_modified) == 0) {
      organizations_with_invalid_packages[nrow(organizations_with_invalid_packages) + 1,] = 
        c(organization_id, organization_name, package$result$id, package$result$name, "metadata_modified not filled in")
    }
    
    # check metadata_modified
    notes_translated_fr<-package$result$notes_translated$fr
    notes_translated_de<-package$result$notes_translated$de
    notes_translated_nl<-package$result$notes_translated$nl
    notes_translated_en<-package$result$notes_translated$en
    if (notes_translated_fr == "" && notes_translated_de == "" && notes_translated_nl == "" && notes_translated_en == "") {
      organizations_with_invalid_packages[nrow(organizations_with_invalid_packages) + 1,] = 
        c(organization_id, organization_name, package$result$id, package$result$name, "notes_translated not filled in")
    }
    
    # check fluent_tags
    fluent_tags<-package$result$fluent_tags
    print(fluent_tags)
    fluent_tag_found = tag_list$result[tag_list$result[]==fluent_tags,1]
    print(fluent_tag_found)
    
    # check cont_res
    cont_res<-package$result$cont_res
    if (cont_res != "Service" && cont_res != "Data set") {
      organizations_with_invalid_packages[nrow(organizations_with_invalid_packages) + 1,] = 
        c(organization_id, organization_name, package$result$id, package$result$name, "cont_res not filled in")
    }
    
    print(paste("Checking ", organization_name, " package ", package$result$name))
  }
}

write.csv(organizations_with_invalid_packages, file.path(data_path, "organizations_with_invalid_packages.csv"), quote=TRUE)