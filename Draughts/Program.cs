using Draughts.Application.Shared;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using System;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace Draughts;

public static class Program {
    private static readonly object _lock = new object();
    private static volatile bool _logIsConfigured = false;

    public static void Main(string[] args) => CreateHostBuilder(args).Build().Run();

    private static IWebHostBuilder CreateHostBuilder(string[] args) {
        var config = LoadAppSettingsConfiguration();
        var appSettings = config.Get<AppSettings?>();

        return new WebHostBuilder()
            .UseConfiguration(config)
            .ConfigureLogging(log => ConfigureSerilog(appSettings, log, "app"))
            .UseKestrel(options => ConfigureKestrel(appSettings, options))
            .UseContentRoot(Directory.GetCurrentDirectory())
            .UseWebRoot("wwwroot")
            .UseStartup<Startup>();
    }

    public static IConfiguration LoadAppSettingsConfiguration() {
        return new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddJsonFile("appsettings.env.json", optional: true)
            .Build();
    }

    public static void ConfigureSerilog(AppSettings? appSettings, ILoggingBuilder logBuilder, string logName) {
        string pathFormat = GetLogPathFormat(appSettings?.LogDir, $"{logName}-log-{{Date}}.log");

        if (!_logIsConfigured) {
            lock (_lock) {
                if (!_logIsConfigured) {
                    var configurationBuilder = new LoggerConfiguration()
                        .MinimumLevel.Is(FromString(appSettings?.LogLevel))
                        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                        // In order to see SignalR exceptions, you need to log at the debug loglevel.
                        .MinimumLevel.Override("Microsoft.AspNetCore.SignalR", FromString(appSettings?.LogLevel))
                        .MinimumLevel.Override("Microsoft.AspNetCore.Http.Connections", FromString(appSettings?.LogLevel))
                        .MinimumLevel.Override("System", LogEventLevel.Warning)
                        .WriteTo.RollingFile(pathFormat: pathFormat);
                    Log.Logger = configurationBuilder.CreateLogger();
                    _logIsConfigured = true;
                }
            }
        }

        logBuilder.ClearProviders();
        logBuilder.AddSerilog();

        Log.Information("Starting Draughts application...");
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

    private static LogEventLevel FromString(string? logLevel) {
        return logLevel?.ToLowerInvariant() switch {
            "fatal" => LogEventLevel.Fatal,
            "error" => LogEventLevel.Error,
            "warning" => LogEventLevel.Warning,
            "information" => LogEventLevel.Information,
            "debug" => LogEventLevel.Debug,
            "verbose" => LogEventLevel.Verbose,
            null => LogEventLevel.Information,
            _ => throw new ArgumentException("Unknown log level", nameof(logLevel))
        };
    }

    private static void ConfigureKestrel(AppSettings? appSettings, KestrelServerOptions options) {
        options.AddServerHeader = false;
        options.Listen(IPAddress.Any, appSettings?.Port ?? 8000);

        if (!string.IsNullOrEmpty(appSettings?.CertificateFile)) {
            try {
                var certificate = new X509Certificate2(appSettings.CertificateFile, appSettings.CertificatePassword);

                options.Listen(IPAddress.Any, appSettings.HttpsPort ?? 443, listenOptions => {
                    listenOptions.UseHttps(certificate);
                });
            }
            catch (SystemException e) {
                Log.Error(e, "Error while reading the certificate, not using https.");
            }
        }
    }
}
