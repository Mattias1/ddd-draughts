using Draughts.Application.Shared;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Configuration;
using Serilog.Events;
using System;
using System.IO;

namespace Draughts {
    public class Program {
        private static readonly object _lock = new object();
        private volatile static bool _logIsConfigured = false;

        public static void Main(string[] args) => CreateHostBuilder(args).Build().Run();

        public static IHostBuilder CreateHostBuilder(string[] args) {
            return Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(configuration => ConfigureAppsettings(configuration))
                .ConfigureLogging((ctx, log) => ConfigureSerilog(ctx.Configuration, log, "draughts-app"))
                .ConfigureWebHostDefaults(webBuilder => {
                    webBuilder.UseStartup<Startup>();
                });
        }

        public static void ConfigureAppsettings(IConfigurationBuilder configuration) {
            configuration.AddJsonFile("appsettings.json");
            configuration.AddJsonFile("appsettings.env.json", optional: true);
        }

        public static void ConfigureSerilog(IConfiguration configuration, ILoggingBuilder logBuilder, string logName) {
            var settings = configuration.Get<AppSettings?>();

            string pathFormat = GetLogPathFormat(settings?.LogDir, $"{logName}-log-{{Date}}.log");

            if (!_logIsConfigured) {
                lock(_lock) {
                    if (!_logIsConfigured) {
                        var configurationBuilder = new LoggerConfiguration()
                            .MinimumLevel.FromString(settings?.LogLevel)
                            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                            .MinimumLevel.Override("System", LogEventLevel.Warning)
                            .WriteTo.RollingFile(pathFormat: pathFormat);
                        Log.Logger = configurationBuilder.CreateLogger();
                        _logIsConfigured = true;
                    }
                }
            }

            logBuilder.ClearProviders();
            logBuilder.AddSerilog();

            Log.Information($"Starting Draughts application...");
        }

        private static string GetLogPathFormat(string? rootDir, string fileName) {
            string logRootPath = ExpandHome(rootDir) ?? "logs";
            return Path.Join(logRootPath, fileName);
        }

        private static string? ExpandHome(string? dir) {
            if (dir is not null && dir.StartsWith('~')) {
                string homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                return Path.Join(homeDir, dir.Substring(1));
            }
            return dir;
        }
    }

    public static class LoggerMinimumLevelConfigurationExtensions {
        public static LoggerConfiguration FromString(this LoggerMinimumLevelConfiguration minimumLevel, string? logLevel) {
            return logLevel?.ToLower() switch
            {
                "fatal" => minimumLevel.Fatal(),
                "error" => minimumLevel.Error(),
                "warning" => minimumLevel.Warning(),
                "information" => minimumLevel.Information(),
                "debug" => minimumLevel.Debug(),
                "verbose" => minimumLevel.Verbose(),
                null => minimumLevel.Information(),
                _ => throw new ArgumentException("Unknown log level", nameof(logLevel))
            };
        }
    }
}
