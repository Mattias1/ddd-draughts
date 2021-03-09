using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using System;
using System.IO;
using System.Reflection;

namespace Draughts {
    public class Program {
        private static bool logIsConfigured = false;

        public static void Main(string[] args) => CreateHostBuilder(args).Build().Run();

        public static IHostBuilder CreateHostBuilder(string[] args) {
            return Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logBuilder => ConfigureSerilog(logBuilder, "draughts-app"))
                .ConfigureWebHostDefaults(webBuilder => {
                    webBuilder.UseStartup<Startup>();
                });
        }

        public static void ConfigureSerilog(ILoggingBuilder logBuilder, string logName) {
            string pathFormat = PathFromProjectRoot($"logs/{logName}-log-{{Date}}.log");

            if (!logIsConfigured) {
                logIsConfigured = true;
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Information()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                    .MinimumLevel.Override("System", LogEventLevel.Warning)
                    .WriteTo.RollingFile(pathFormat: pathFormat)
                    .CreateLogger();
            }

            logBuilder.ClearProviders();
            logBuilder.AddSerilog();

            Log.Information($"Starting Draughts application...");
        }

        private static string PathFromProjectRoot(string fileName) {
            string? exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (exePath is null) {
                throw new InvalidOperationException("Path for the logfile could not be found.");
            }
            // This works, but based on a few assumptions that are probably not true for a prod release :S
            // I might need an actual config file here :(
            return Path.GetFullPath(Path.Combine(exePath, "../../../../", fileName));
        }
    }
}
