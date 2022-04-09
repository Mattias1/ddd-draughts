using Draughts.Command.Seeders;
using Draughts.Repositories;
using Draughts.Repositories.Database;
using Draughts.Test.TestHelpers;
using SqlQueryBuilder.Exceptions;
using System;
using System.Threading;

namespace Draughts.Command;

public sealed class DraughtsConsole {
    private readonly EssentialDataSeeder _essentialDataSeeder;
    private readonly DummyDataSeeder _dummyDataSeeder;
    private readonly IIdGenerator _idGenerator;

    public DraughtsConsole(EssentialDataSeeder essentialDataSeeder, DummyDataSeeder dummyDataSeeder,
            IIdGenerator idGenerator) {
        _essentialDataSeeder = essentialDataSeeder;
        _dummyDataSeeder = dummyDataSeeder;
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

    public static void WaitForDatabaseConnection(int maxSeconds) {
        for (int i = 1; i <= maxSeconds + 1; i++) {
            try {
                using (var tranFlavor = DbContext.Get.BeginMiscTransaction()) {
                    DbContext.Get.Query(tranFlavor).Select().CountAll().From("id_generation").SingleLong();
                    tranFlavor.Commit();
                    return;
                }
            }
            catch (SqlQueryBuilderException) {
                if (i <= maxSeconds) {
                    Console.WriteLine($"Wait for database connection ({i}).");
                    Thread.Sleep(1000);
                }
            }
        }
        throw new InvalidOperationException("Database connection failed.");
    }

    // TODO: How to actually call this commandline utility from outside an IDE?
    private static void PrintHelp() => Console.WriteLine("usage: <draughts.command> [data:essential|data:dummy]");
}
