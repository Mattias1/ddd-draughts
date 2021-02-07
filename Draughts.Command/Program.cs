using Draughts.Command.Seeders;
using Draughts.Repositories.Database;
using Draughts.Repositories.Transaction;
using NodaTime;
using System;

namespace Draughts.Command {
    class Program {
        static void Main(string[] args) {
            var clock = SystemClock.Instance;
            var unitOfWork = new DbUnitOfWork(clock);
            var roleRepository = new DbRoleRepository(unitOfWork);
            var authUserRepository = new DbAuthUserRepository(roleRepository, unitOfWork);
            var gameRepository = new DbGameRepository(unitOfWork);
            var playerRepository = new DbPlayerRepository(unitOfWork);
            var userRepository = new DbUserRepository(unitOfWork);

            try {
                foreach (string arg in args) {
                    switch (arg) {
                        case "data:essential":
                            Console.WriteLine("Start seeding essential data...");
                            var essentialSeeder = new EssentialDataSeeder(authUserRepository, roleRepository, unitOfWork, userRepository);
                            essentialSeeder.SeedData();
                            Console.WriteLine("Successfully seeded the database with essential data.");
                            break;
                        case "data:dummy":
                            Console.WriteLine("Start seeding dummy data...");
                            var dummySeeder = new DummyDataSeeder(authUserRepository, gameRepository, playerRepository,
                                roleRepository, unitOfWork, userRepository);
                            dummySeeder.SeedData();
                            Console.WriteLine("Successfully seeded the database with dummy data.");
                            break;
                        default:
                            PrintHelp();
                            break;
                    }
                }
                if (args.Length == 0) {
                    Console.Write("No argument given; ");
                    PrintHelp();
                }
            }
            catch (Exception e) {
                PrintHelp();
                Console.WriteLine();
                Console.WriteLine("En error occured: " + e.Message);
                Console.WriteLine();
                Console.WriteLine("Stacktrace:");
                Console.WriteLine(e.StackTrace);
            }
        }

        // TODO: How to actually call this commandline utility from outside an IDE?
        private static void PrintHelp() => Console.WriteLine("usage: <draughts.command> [data:essential|data:dummy]");
    }
}
