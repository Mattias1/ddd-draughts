using Draughts.Application.Shared;
using Draughts.Repositories;
using Draughts.Repositories.Misc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace Draughts.Command;

public sealed class Program {
    private const int EXIT_CODE_SUCCESS = 0;
    private const int EXIT_CODE_FAILURE = -1;

    static int Main(string[] args) {
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

                return EXIT_CODE_FAILURE;
            }
        }

        return EXIT_CODE_SUCCESS;
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
