library("rjson")

# We assume the following:
# - the download script has run and we have organizations loaded.
#     - organizations_path has been set by download script.
# - the stakeholders-get-all-the-orgs has run and we have stake_orgs loaded.

# for every organization from the API, check if they are in the stakeholders list.
organizations_not_in_stakeholders = data.frame(Characters=character(), Characters=character())
names(organizations_not_in_stakeholders) <- c("organization_id", "organization_name")
for (i in 1:length(organizations$result)) {     
  organization_name<-organizations$result[i]
  organization <- fromJSON(file=file.path(organizations_path, paste(organization_name, ".json", sep="")))
  # print(organization) 
  organization_id = organization$result$id
  # print(organization_id) 
  stakeholder_id = stake_orgs[stake_orgs[,6]==organization_id,1]
  
  if (identical(stakeholder_id, integer(0))) {
    organizations_not_in_stakeholders[nrow(organizations_not_in_stakeholders) + 1,] = c(organization_id, organization_name)
  }
}

write.csv(organizations_not_in_stakeholders, file.path(data_path, "organizations_not_in_stakeholders.csv"), quote=TRUE)