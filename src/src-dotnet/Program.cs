// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using System;
using System.IO;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Serilog;
using Serilog.Events;

using ZenMonitor.Core.Hosting;
using ZenMonitor.UserConfig;

namespace ZenMonitor;

internal static class Program
{
    internal static async Task Main(string[] args)
    {
        // Setup paths
        const string appName = "ZenMonitor";
        var configFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), appName);
        var configFilePath = Path.Combine(configFolder, "user.config.json");
        var logFolder = Path.Combine(configFolder, "logs");
        var logFilePath = Path.Combine(logFolder, "dotnet.log");

        // Load config
        var initialConfig = ConfigService.InitConfig(configFilePath);
        var logLevel = ParseSerilogLevel(initialConfig.LogLevel);

        // Configure Serilog
        ConfigureLogging(logLevel, logFolder, logFilePath);

        // Create logger factory
        var loggerFactory = LoggerFactory.Create(builder => builder.AddSerilog());
        var configLogger = loggerFactory.CreateLogger<ConfigService>();
        var configService = new ConfigService(configFilePath, configLogger, initialConfig);

        // Build host
        var builder = Host.CreateApplicationBuilder(args);
        builder.Logging.ClearProviders();
        builder.Logging.AddSerilog();

        builder.Services.AddSingleton<IConfigService>(configService);
        builder.Services.AddZenMonitor();

        var app = builder.Build();
        await app.RunAsync();
    }

    private static void ConfigureLogging(LogEventLevel logLevel, string logFolder, string logFilePath)
    {
        Directory.CreateDirectory(logFolder);

        var loggerConfig = new LoggerConfiguration()
            .MinimumLevel.Is(logLevel)
            .WriteTo.File(
                logFilePath,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}",
                rollOnFileSizeLimit: true,
                fileSizeLimitBytes: 1_000_000)
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}");

        Log.Logger = loggerConfig.CreateLogger();
    }

    private static LogEventLevel ParseSerilogLevel(string? level)
    {
        return level?.ToLowerInvariant() switch
        {
            "verbose" => LogEventLevel.Verbose,
            "debug" => LogEventLevel.Debug,
            "info" or "information" => LogEventLevel.Information,
            "warn" or "warning" => LogEventLevel.Warning,
            "error" => LogEventLevel.Error,
            "fatal" => LogEventLevel.Fatal,
            _ => LogEventLevel.Information
        };
    }
}
