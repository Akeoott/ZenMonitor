// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using System.ComponentModel;
using System.Runtime.InteropServices;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Serilog;
using Serilog.Events;

using Spectre.Console.Cli;

using ZenMonitor.Core.Interfaces;

namespace ZenMonitor.Cli;

internal class Program
{
    internal static async Task<int> Main(string[] args)
    {
        var app = new CommandApp<MonitorCommand>();
        return await app.RunAsync(args);
    }
}

public class MonitorSettings : CommandSettings
{
    #region Cli Options
    [CommandOption("-v|--verbosity <LEVEL>")]
    [Description("Set logging verbosity level: c[ritical], e[rror], w[arning], i[nfo], d[ebug], t[race])")]
    [DefaultValue("info")]
    public required string LogLevel { get; set; }

    [CommandOption("-c|--console <BOOL>")]
    [Description("Enable console logging. Use `--console true` to enable. (might not work work properly when running cli interface)")]
    [DefaultValue("false")]
    public bool ConsoleOutput { get; set; } = false;
    #endregion
}

public class MonitorCommand() : AsyncCommand<MonitorSettings>
{

    protected override async Task<int> ExecuteAsync(
        CommandContext context,
        MonitorSettings settings,
        CancellationToken cancellationToken)
    {
        #region Logging Configuration
        var logLevel = ParseSerilogLevel(settings.LogLevel);
        var logFilePath = "logs/ZenMonitor.log";

        Directory.CreateDirectory("logs");
        File.WriteAllText(logFilePath, string.Empty);

        var loggerConfig = new LoggerConfiguration()
            .MinimumLevel.Is(logLevel)
            .Enrich.WithProperty("RunId", Guid.NewGuid());

        if (settings.ConsoleOutput)
        {
            loggerConfig.WriteTo.Console(
                outputTemplate:
                    "[{Timestamp:HH:mm:ss.fff} {Level:u3}] {Message:lj}{NewLine}{Exception}");
        }

        loggerConfig.WriteTo.File(
            logFilePath,
            outputTemplate:
                "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{RunId}] [{SourceContext}] {Message:lj}{NewLine}{Exception}");
        #endregion


        #region Dependency Injection
        Log.Logger = loggerConfig.CreateLogger();

        try
        {
            var services = new ServiceCollection();

            services.AddLogging(builder =>
            {
                builder.ClearProviders();
                builder.AddSerilog(dispose: true);
            });

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                services.AddSingleton<ICpuService, Core.Services.Linux.Cpu>();
                services.AddSingleton<IGpuService, Core.Services.Linux.Gpu>();
                services.AddSingleton<IMemoryService, Core.Services.Linux.Memory>();
                services.AddSingleton<INetworkService, Core.Services.Linux.Network>();
                services.AddSingleton<IStorageService, Core.Services.Linux.Storage>();
                services.AddSingleton<ISystemService, Core.Services.Linux.System>();
            }
            // TODO: Uncomment and adjust once the Linux implementation is finished.
            // else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            // {
            //     services.AddSingleton<ICpuService, Core.Services.Windows.Cpu>();
            //     Other services ...
            // }
            else
            {
                throw new PlatformNotSupportedException(
                    "ZenMonitor only supports Linux at the moment. Windows support will come in the future."
                );
            }

            services.AddTransient<MonitorEngine>();

            var serviceProvider = services.BuildServiceProvider();
            var engine = serviceProvider.GetRequiredService<MonitorEngine>();
            var _logger = serviceProvider.GetRequiredService<ILogger<MonitorCommand>>();

            _logger.LogWarning("ZenMonitor started.");

            await engine.Run();
            return 0;
        }
        finally
        {
            Log.CloseAndFlush();
        }
        #endregion
    }

    private static LogEventLevel ParseSerilogLevel(string level)
    {
        return level?.ToLowerInvariant() switch
        {
            "t" or "trace" => LogEventLevel.Verbose,
            "d" or "debug" => LogEventLevel.Debug,
            "i" or "info" => LogEventLevel.Information,
            "w" or "warning" => LogEventLevel.Warning,
            "e" or "error" => LogEventLevel.Error,
            "c" or "critical" => LogEventLevel.Fatal,
            _ => LogEventLevel.Information
        };
    }
}
