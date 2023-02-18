using Draughts.Application.ModPanel.Services;
using Draughts.Command.Seeders;
using Draughts.Domain.AuthContext.Models;
using Draughts.Domain.UserContext.Models;
using Draughts.Repositories.Misc;
using Draughts.Test.TestHelpers;
using SqlQueryBuilder.Exceptions;
using System;
using System.Threading;

namespace Draughts.Command;

public sealed class DraughtsConsole {
    private readonly EssentialDataSeeder _essentialDataSeeder;
    private readonly DummyDataSeeder _dummyDataSeeder;
    private readonly SystemEventQueueService _systemEventQueueService;
    private readonly IIdGenerator _idGenerator;

    public DraughtsConsole(EssentialDataSeeder essentialDataSeeder, DummyDataSeeder dummyDataSeeder,
            SystemEventQueueService systemEventQueueService, IIdGenerator idGenerator) {
        _essentialDataSeeder = essentialDataSeeder;
        _dummyDataSeeder = dummyDataSeeder;
        _systemEventQueueService = systemEventQueueService;
        _idGenerator = idGenerator;
    }

    public void ExecuteCommand(string[] args) {
        IdTestHelper.IdGenerator = _idGenerator;

        foreach (string arg in args) {
            switch (arg) {
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
                default:
                    if (arg != "--help") {
                        Console.Write($"Unknown argument given ('{arg}'); ");
                    }
                    PrintHelp();
                    break;
            }
        }
        if (args.Length == 0) {
            Console.Write("No argument given; ");
            PrintHelp();
        }
    }

    public static void WaitForDatabaseConnection(int maxSeconds) {
        for (int i = 1; i <= maxSeconds + 1; i++) {
            try {
                using (var tranFlavor = DbContext.Get.BeginMiscTransaction()) {
                    DbContext.Get.Query(tranFlavor).CountAllFrom("id_generation").SingleLong();
                    tranFlavor.Commit();
                    return;
                }
            } catch (SqlQueryBuilderException e) {
                if (i <= maxSeconds) {
                    Console.WriteLine($"Wait for database connection ({i}: {e.Message})");
                    Thread.Sleep(1000);
                }
            }
        }
        throw new InvalidOperationException("Database connection failed.");
    }

    // TODO: How to actually call this commandline utility from outside an IDE?
    private static void PrintHelp() {
        Console.WriteLine("usage: <draughts.command> ["
            + "data:essential|data:dummy|events:sync|events:dispatch"
            + "]");
    }
}
