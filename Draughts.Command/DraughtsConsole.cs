using Draughts.Application.ModPanel.Services;
using Draughts.Command.Seeders;
using Draughts.Domain.AuthContext.Models;
using Draughts.Domain.UserContext.Models;
using Draughts.Repositories.Misc;
using Draughts.Test.TestHelpers;
using Microsoft.Extensions.Logging;
using SqlQueryBuilder.Exceptions;
using System;
using System.Threading;

namespace Draughts.Command;

public sealed class DraughtsConsole {
    private readonly EssentialDataSeeder _essentialDataSeeder;
    private readonly DummyDataSeeder _dummyDataSeeder;
    private readonly SystemEventQueueService _systemEventQueueService;
    private readonly IIdGenerator _idGenerator;
    private readonly ILogger<DraughtsConsole> _logger;

    private string[] MigrationDatabases => new string[] {
        DbContext.AUTH_DATABASE, DbContext.GAME_DATABASE, DbContext.MISC_DATABASE, DbContext.USER_DATABASE
    };

    public DraughtsConsole(EssentialDataSeeder essentialDataSeeder, DummyDataSeeder dummyDataSeeder,
            SystemEventQueueService systemEventQueueService, IIdGenerator idGenerator,
            ILogger<DraughtsConsole> logger) {
        _essentialDataSeeder = essentialDataSeeder;
        _dummyDataSeeder = dummyDataSeeder;
        _systemEventQueueService = systemEventQueueService;
        _idGenerator = idGenerator;
        _logger = logger;
    }

    public void ExecuteCommand(string[] args) {
        IdTestHelper.IdGenerator = _idGenerator;

        for (int i = 0; i < args.Length; i++) {
            string arg = args[i];
            string? nextArg = args.Length > i + 1 ? args[i + 1] : null;
            switch (arg) {
                case "data:dev":
                    WaitForDatabaseConnection(5);

                    Console.WriteLine("Initializing dev database (all migrations and seeds)...");
                    MigrateUp();

                    if (_essentialDataSeeder.DatabaseNeedsSeeding()) {
                        Console.WriteLine();
                        Console.WriteLine("Start seeding essential and dummy data...");
                        _essentialDataSeeder.SeedData();
                        _dummyDataSeeder.SeedData();
                        Console.WriteLine("Successfully initialized the database for development.");
                    }
                    else {
                        Console.WriteLine("The database is not empty, so no seeding required.");
                    }
                    break;

                case "migrate":
                case "migrate:up":
                    WaitForDatabaseConnection(5);

                    Console.WriteLine("Start database migration...");
                    MigrateUp();
                    Console.WriteLine("Successfully migrated the database.");
                    break;
                case "migrate:down":
                    WaitForDatabaseConnection(5);

                    if (nextArg is null || !long.TryParse(nextArg, out long migrationVersion)) {
                        throw new InvalidOperationException("migrate:down requires the migration-version (number) as argument");
                    }
                    i++;

                    Console.WriteLine($"Revert database migration to version {migrationVersion}...");
                    MigrateDown(migrationVersion);
                    Console.WriteLine("Successfully reverted the database migration.");
                    break;

                case "data:essential":
                    WaitForDatabaseConnection(5);

                    Console.WriteLine("Start seeding essential data...");
                    _essentialDataSeeder.SeedData();
                    Console.WriteLine("Successfully seeded the database with essential data.");
                    break;
                case "data:dummy":
                    Console.WriteLine("Start seeding dummy data...");
                    _dummyDataSeeder.SeedData();
                    Console.WriteLine("Successfully seeded the database with dummy data.");
                    break;

                case "events:syncstatus":
                case "events:sync":
                    Console.WriteLine("Start event queue status sync...");
                    _systemEventQueueService.SyncEventQueueStatus(new UserId(UserId.ADMIN), new Username(Username.ADMIN));
                    Console.WriteLine("Successfully synced the event queue status...");
                    break;
                case "events:redispatch":
                case "events:dispatch":
                    Console.WriteLine("Start event queue status redispatch...");
                    _systemEventQueueService.RedispatchEventQueue(new UserId(UserId.ADMIN), new Username(Username.ADMIN));
                    Console.WriteLine("Successfully dispatched the unhandled events...");
                    break;

                case "-v":
                case "--version":
                    Console.WriteLine("Draughts console - version alpha");
                    break;
                case "-h":
                case "--help":
                    PrintHelp();
                    break;
                default:
                    Console.Write($"Unknown argument given ('{arg}'); ");
                    PrintHelp();
                    break;
            }
        }
        if (args.Length == 0) {
            Console.Write("No argument given; ");
            PrintHelp();
        }
    }

    private void WaitForDatabaseConnection(int maxSeconds) {
        for (int i = 1; i <= maxSeconds + 1; i++) {
            try {
                using (var tranFlavor = DbContext.Get.BeginMiscTransaction()) {
                    // If we can start a transaction, we have a connection
                    tranFlavor.Commit();
                    return;
                }
            } catch (SqlQueryBuilderException e) {
                string connectionString = DbContext.Get.ConnectionStringForMigrations(DbContext.MISC_DATABASE);
                _logger.LogWarning(e, $"Database connection failure ({i}; {connectionString}).");
                if (i <= maxSeconds) {
                    Console.WriteLine($"Wait for database connection ({i}: {e.Message})");
                    Thread.Sleep(1000);
                }
            }
        }
        throw new InvalidOperationException("Database connection failed.");
    }

    private void MigrateUp() {
        foreach (string database in MigrationDatabases) {
            Program.WithMigrationRunner(database, runner => runner.MigrateUp());
        }
    }

    private void MigrateDown(long migrationVersion) {
        foreach (string database in MigrationDatabases) {
            Program.WithMigrationRunner(database, runner => runner.MigrateDown(migrationVersion));
        }
    }

    private static void PrintHelp() {
        Console.WriteLine("usage: dotnet Draughts.Command.dll ["
            + "data:dev|data:essential|data:dummy|migrate:up|migrate:down|events:sync|events:dispatch"
            + "]");
    }
}
