using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NAP.AutoChecks;
using NAP.AutoChecks.API;
using NAP.AutoChecks.API.Stakeholders._2023;
using NAP.AutoChecks.Evaluation0_DataValidation;
using NAP.AutoChecks.Evaluation1_1;
using NAP.AutoChecks.Evaluation1_2;
using NAP.AutoChecks.Evaluation1_2._2022;
using NAP.AutoChecks.Evaluation1_2._2023;
using NAP.AutoChecks.Evaluation2_1;
using NAP.AutoChecks.Evaluation2_2;
using NAP.AutoChecks.Evaluation2_2._2022;
using NAP.AutoChecks.Evaluation2_2._2023;
using NAP.AutoChecks.Queries;
using Serilog;
using TransportDataBe.Client;

public static class Program
{
    static async Task Main(string[] args)
    {
        var loggerFactory = new LoggerFactory();
        loggerFactory.AddSerilog();

        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostingContext, services) =>
            {
                Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(hostingContext.Configuration)
                    .CreateLogger();
                services.AddLogging(b =>
                {
                    b.ClearProviders();
                    b.AddSerilog();
                });

                var dataPath = hostingContext.Configuration.GetValue<string>("DataPath");
                var sampleDateString = hostingContext.Configuration.GetValue<string>("SamplingDate");
                if (!DateOnly.TryParse(sampleDateString, System.Globalization.CultureInfo.InvariantCulture,
                        System.Globalization.DateTimeStyles.None,
                        out var sampleDate))
                {
                    sampleDate = System.DateOnly.FromDateTime(DateTime.Today);
                }

                var apiKey = hostingContext.Configuration.GetValue<string>("ApiKey");
                services.AddTransportDataClient(s =>
                {
                    s.ApiKey = apiKey;
                });
                services.AddSingleton(new DataHandlerSettings()
                {
                    DataPath = dataPath,
                    SampleDay = sampleDate,
                });
                services.AddSingleton<DataHandler>();
                services.AddSingleton(new MMTISDeadlineSettings());
                services.AddSingleton<StakeholderWithoutNAPTypeCheck>();
                services.AddSingleton<OrganizationsNotInStakeholdersCheck>();
                services.AddSingleton<StakeholderHasPackagesCheck>();
                services.AddSingleton<RequiredFieldsFilledInCheck>();
                services.AddSingleton<StakeholdersWithoutDeclarations>();
                services.AddSingleton<StakeholdersWithDeclarations>();
                services.AddSingleton<StakeholdersRegisteredCheck>();
                services.AddSingleton<StakeholdersPackagesAfterDeadline>();
                services.AddSingleton<RandomizeOrganizationsWithDeclarations>();
                services.AddSingleton(new RandomizeOrganizationsWithDeclarationsSettings());

                services.AddSingleton(new StratifiedSamplingSetting());
                services.AddSingleton<StratifiedSampling>();
                services.AddSingleton<SelectedIn2022DatasetLoader>();
                services.AddSingleton(new SelectedIn2022DatasetsSettings()
                {
                    DataPath = dataPath
                });
                services.AddSingleton<SelectedIn2023DatasetLoader>();
                services.AddSingleton(new SelectedIn2023DatasetsSettings()
                {
                    DataPath = dataPath
                });
                services.AddSingleton<SelectedIn2022OrganizationLoader>();
                services.AddSingleton(new SelectedIn2022OrganizationLoaderSettings()
                {
                    DataPath = dataPath
                });
                services.AddSingleton<SelectedIn2023OrganizationLoader>();
                services.AddSingleton(new SelectedIn2023OrganizationLoaderSettings()
                {
                    DataPath = dataPath
                });

                services.AddSingleton<StakeholderLoader>();
                services.AddSingleton<AllStakeholders>();
                services.AddSingleton<StakeholdersAllDeclarations>();

                services.AddSingleton<Evaluation0>();
                services.AddSingleton<Evaluation1_1>();
                services.AddSingleton<Evaluation1_2>();
                services.AddSingleton<Evaluation2_1>();
                services.AddSingleton<Evaluation2_2>();
            }).UseConsoleLifetime().Build();

        using var scope = host.Services.CreateScope();
        
        var evaluation0 = scope.ServiceProvider.GetRequiredService<Evaluation0>();
        await evaluation0.Run();

        var evaluation1_1 = scope.ServiceProvider.GetRequiredService<Evaluation1_1>();
        await evaluation1_1.Run();

        var evaluation1_2 = scope.ServiceProvider.GetRequiredService<Evaluation1_2>();
        await evaluation1_2.Run();
        
        var evaluation2_1 = scope.ServiceProvider.GetRequiredService<Evaluation2_1>();
        await evaluation2_1.Run();
        
        var evaluation2_2 = scope.ServiceProvider.GetRequiredService<Evaluation2_2>();
        await evaluation2_2.Run();
        
        var allDeclarations = scope.ServiceProvider.GetRequiredService<StakeholdersAllDeclarations>();
        await allDeclarations.Get();
    }
}