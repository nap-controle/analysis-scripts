# NAP.AutoChecks

Automated validation and auditing tools for the Belgian National Access Point (transportdata.be) as part of the NAP Control Body ("Controleorgaan") contract.

## What this does

Runs automated checks against the transportdata.be CKAN API to verify stakeholder compliance across four EU ITS Directive NAP types: **MMTIS**, **RTTI**, **SRTI**, **SSTP**.

- **Task 0** — Find organizations without matching stakeholders
- **Task 1** — Stakeholder compliance: registration (1A), packages (1B), self-declarations (1C)
- **Task 2** — Change detection year-over-year (TODO)
- **Task 3** — Stratified random sampling of 30 datasets (15 MMTIS, 5 RTTI, 5 SRTI, 5 SSTP)
- **Task 4** — Random sampling of 4 organizations (1 per NAP type)
- **Queries** — Download declaration PDFs and proxy agreements

## Build & run

```bash
# Restore and build
dotnet restore
dotnet build --configuration Release --no-restore

# Run (from repo root)
cd ./src/NAP.AutoChecks/ && dotnet run -c release ../../data/
```

The single CLI argument is the path to the data directory.

## Solution structure

```
src/
  NAP.AutoChecks/             # Main console app (net10.0)
  TransportDataBe.Client/     # HTTP client for CKAN API (net6.0)
data/                         # gitignored — JSON snapshots, Excel outputs, stakeholder CSVs
old_r_scripts/                # Legacy R implementations (superseded)
```

## Configuration

`src/NAP.AutoChecks/appsettings.json`:
- `SamplingDate` — date string for the current snapshot (e.g. `"2025-09-10"`)
- `DataPath` — absolute path to `data/` directory
- `ApiKey` — JWT token for transportdata.be API

## Data directory layout

```
data/
  stakeholders/{year}/        # CSVs: MMTIS.csv, RTTI.csv, SRTI.csv, SSTP.csv (from Dropbox)
  latest/                     # Most recent snapshot + Excel outputs
  {YYYY-MM-DD}/               # Timestamped historical snapshots
```

Stakeholder CSVs use `;` as delimiter with columns: Id, Name, OrganizationId.

## CI

GitHub Actions (`.github/workflows/daily.yml`): runs weekly on Sundays. Downloads stakeholder CSVs from Dropbox, builds, runs, and mirrors results to Google Drive.

## Key dependencies

- **ClosedXML** — Excel output
- **CsvHelper** — stakeholder CSV parsing (semicolon delimiter, InvariantCulture)
- **Microsoft.Extensions.Hosting** — DI and configuration
- **Serilog** — logging

## No test projects

The solution has no test projects. The `dotnet test` step in CI is a no-op.
