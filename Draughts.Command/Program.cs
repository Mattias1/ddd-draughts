using Draughts.Application.Shared;
using Draughts.Command.Migrations;
using Draughts.Repositories.Misc;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Initialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SqlQueryBuilder.Options;
using System;

namespace Draughts.Command;

public static class Program {
    private const int EXIT_CODE_SUCCESS = 0;
    private const int EXIT_CODE_FAILURE = -1;

    public static int Main(string[] args) {
        var host = CreateHostBuilder(args).Build();

        using (var serviceScope = host.Services.CreateScope()) {
            try {
                var console = serviceScope.ServiceProvider.GetRequiredService<DraughtsConsole>();
                console.ExecuteCommand(args);
            } catch (Exception e) {
                Console.WriteLine();
                Console.WriteLine("En error occured: " + e.Message);
                Console.WriteLine();
                Console.WriteLine("Stacktrace:");
                Console.WriteLine(e.StackTrace);

                return EXIT_CODE_FAILURE;
            }
        }

        return EXIT_CODE_SUCCESS;
    }

    private static IHostBuilder CreateHostBuilder(string[] args) {
        return Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration(config => Draughts.Program.ConfigureAppsettings(config))
            .ConfigureLogging((ctx, log) => {
                Draughts.Program.ConfigureSerilog(ctx.Configuration, log, "draughts-command");
            })
            .ConfigureServices(services => DraughtsCommandServiceProvider.ConfigureServices(services))
            .ConfigureAppConfiguration((ctx, config) => {
                var settings = ctx.Configuration.Get<AppSettings>();

                DbContext.Init(settings?.DbPort ?? 52506, settings?.DbPassword ?? "devapp");
                QueryBuilderOptions.SetupDapperWithSnakeCaseAndNodaTime();
            });
    }

    public static void WithMigrationRunner(string database, Action<IMigrationRunner> migrationFunc) {
        var services = new ServiceCollection()
            .AddFluentMigratorCore()
            .ConfigureRunner(r => r
                .AddMySql5()
                .WithGlobalConnectionString(DbContext.Get.ConnectionStringForMigrations(database))
                .ScanIn(typeof(Migration_2023_05_05_Misc).Assembly).For.Migrations())
            .Configure<RunnerOptions>(o => o.Tags = new string[] { database })
            .AddLogging(s => s.AddFluentMigratorConsole())
            .BuildServiceProvider();
        var migrationRunner = services.GetRequiredService<IMigrationRunner>();
        migrationFunc.Invoke(migrationRunner);
    }
}
