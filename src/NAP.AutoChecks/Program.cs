﻿// See https://aka.ms/new-console-template for more information

using CsvHelper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NAP.AutoChecks;
using NAP.AutoChecks.API;
using NAP.AutoChecks.Checks;
using NAP.AutoChecks.Domain;
using Serilog;
using Serilog.Formatting.Json;
using TransportDataBe.Client;
using TransportDataBe.Client.Models;

public static class Program
{
    static async Task Main(string[] args)
    {
        if (args.Length == 0)
        {
            args = new[] { "/Users/xivk/work/nap/analysis-scripts/data" };
        }
        var dataPath = args[0];
        
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

                services.AddTransportDataClient();
                services.AddSingleton(new DataHandlerSettings()
                {
                    DataPath = dataPath
                });
                services.AddSingleton<DataHandler>();
                services.AddSingleton<OrganizationsNotInStakeholders>();
                services.AddSingleton<StakeholderHasPackages>();
            }).UseConsoleLifetime().Build();

        using var scope = host.Services.CreateScope();

        var check1 = scope.ServiceProvider.GetRequiredService<OrganizationsNotInStakeholders>();
        await check1.Check();

        var check2 = scope.ServiceProvider.GetRequiredService<StakeholderHasPackages>();
        await check2.Check();
    }
}