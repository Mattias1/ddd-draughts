using Draughts.Application.Shared;
using Draughts.Repositories.Database;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace Draughts.Command {
    class Program {
        static void Main(string[] args) {
            var host = CreateHostBuilder(args).Build();

            using (var serviceScope = host.Services.CreateScope()) {
                try {
                    var console = serviceScope.ServiceProvider.GetRequiredService<DraughtsConsole>();
                    console.ExecuteCommand(args);
                }
                catch (Exception e) {
                    Console.WriteLine();
                    Console.WriteLine("En error occured: " + e.Message);
                    Console.WriteLine();
                    Console.WriteLine("Stacktrace:");
                    Console.WriteLine(e.StackTrace);
                }
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) {
            return Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(config => Draughts.Program.ConfigureAppsettings(config))
                .ConfigureLogging((ctx, log) => {
                    Draughts.Program.ConfigureSerilog(ctx.Configuration, log, "draughts-command");
                })
                .ConfigureServices(services => DraughtsCommandServiceProvider.ConfigureServices(services))
                .ConfigureAppConfiguration((ctx, config) => {
                    var settings = ctx.Configuration.Get<AppSettings>();

                    DbContext.Init(settings?.DbPassword ?? "devapp");
                });
        }
    }
}
