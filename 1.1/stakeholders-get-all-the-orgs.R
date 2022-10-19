
# set the data path here.
data_path<-"data"

# load orgs csvs.
stake_orgs <- read.csv(file.path(data_path, "stakeholders", "organisations.csv"), header=TRUE, stringsAsFactors=FALSE, sep = ";")
stake_orgs_mmtis <- read.csv(file.path(data_path, "stakeholders", "organisations_MMTIS.csv"), header=TRUE, stringsAsFactors=FALSE, sep = ";")
stake_orgs_rtti <- read.csv(file.path(data_path, "stakeholders", "organisations_RTTI.csv"), header=TRUE, stringsAsFactors=FALSE, sep = ";")
stake_orgs_srti <- read.csv(file.path(data_path, "stakeholders", "organisations_SRTI.csv"), header=TRUE, stringsAsFactors=FALSE, sep = ";")
stake_orgs_sstp <- read.csv(file.path(data_path, "stakeholders", "organisations_SSTP.csv"), header=TRUE, stringsAsFactors=FALSE, sep = ";")

# join in other tables.
stake_orgs <- merge(x=stake_orgs, y=stake_orgs_mmtis, by="Organisation.ID", all.x=TRUE)
stake_orgs <- merge(x=stake_orgs, y=stake_orgs_rtti, by="Organisation.ID", all.x=TRUE)
stake_orgs <- merge(x=stake_orgs, y=stake_orgs_srti, by="Organisation.ID", all.x=TRUE)
stake_orgs <- merge(x=stake_orgs, y=stake_orgs_sstp, by="Organisation.ID", all.x=TRUE)

# add column for all da stakeholders.
stake_orgs$DA.stakeholder <- stake_orgs$DA.stakeholder.MMTIS == "Yes" | stake_orgs$DA.stakeholder.SSTP == "Yes" | stake_orgs$DA.stakeholder.RTTI == "Yes" | stake_orgs$DA.stakeholder.SRTI == "Yes"

# get all orgs that are a DA stakeholder but without a "geregistreerd" value.
stake_orgs_not_registered <- stake_orgs[!is.na(stake_orgs$DA.stakeholder),]
stake_orgs_not_registered <- stake_orgs_not_registered[nchar(stake_orgs_not_registered$geregistreerd) < 4,]

# write result.
stake_orgs_not_registered$website <- gsub('\n', '', stake_orgs_not_registered$website)
stake_orgs_not_registered$Description <- gsub('\n', '', stake_orgs_not_registered$Description)
write.csv(stake_orgs_not_registered, file.path(data_path, "stakeholders-not-registered.csv"), quote=TRUE)