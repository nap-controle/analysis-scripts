# r-analysis-scripts

A collection of scripts to analyse the the data in transportdata.be

## Scripts

- download.R : Downloads *all* the data from the API.

## Evaluation 1.1

### Question A

**Is the stakeholder registered in the NAP?**

1. Use the download script.
2. Get the stakeholders list and convert it to csv files readable by R.
3. Use the stakeholders-get-all-the-orgs to merge the files and to get a list of unregistered stakeholders.
4. Use the stakeholders-registered-not-in-list to get all stakeholders registered but not in the the stakeholders list.

### Question B

**Has the stakeholder registered any data or services on the NAP?**

1. Run Question A.
2. Use the stakeholders-has-packages to see if the stakeholders have registered packages.

### Question C

**Have the deadlines been respected?**

This is not to be checked for now, deadlines have passed already.

### Question D

**Was every required field filled in with at least one of the required languages (EN, NL, FR, DE)?**

1. Run Question A and B.
2. Use the stakeholders-has-filled to see if the stakeholders have filled in all required fields.