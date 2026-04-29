// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using System.ComponentModel;
using System.Runtime.InteropServices;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Serilog;
using Serilog.Events;

using Spectre.Console;
using Spectre.Console.Cli;

using ZenMonitor.Core.Interfaces;

namespace ZenMonitor;

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
    [CommandArgument(0, "<MODE>")]
    [Description("Output mode: cli, gui, or debug.")]
    public required string OutputMode { get; set; }

    [CommandOption("-d|--delay <VALUE>")]
    [Description("Change the delay before updating, max is 10. (value is in seconds)")]
    [DefaultValue(1)]
    public int LoopDelay { get; set; } = 1;

    [CommandOption("-l|--log-level <LEVEL>")]
    [Description("Set logging verbosity: c|critical, r|error, w|warning, i|info, d|debug, t|trace)")]
    [DefaultValue("info")]
    public required string LogLevel { get; set; } = "info";

    [CommandOption("-L|--log-cli <BOOL>")]
    [Description("Enable console logging. Use `--log-cli true` to enable. (Output has to be set to debug)")]
    [DefaultValue("false")]
    public bool CliLogging { get; set; } = false;
    #endregion

    #region Cli Validation
    public override ValidationResult Validate()
    {
        string mode = OutputMode?.ToLowerInvariant() ?? "";

        if (mode != "cli" && mode != "gui" && mode != "debug")
        {
            return ValidationResult.Error(
                "Invalid output mode. Please use 'cli', 'gui', or 'debug'. " +
                "Use '--help' for more information.");
        }

        if (CliLogging && mode != "debug")
        {
            return ValidationResult.Error(
                "When --log-cli is enabled, output mode must be 'debug'.");
        }

        return ValidationResult.Success();
    }
    #endregion
}

public class MonitorCommand() : AsyncCommand<MonitorSettings>
{
    protected override async Task<int> ExecuteAsync(
        CommandContext context,
        MonitorSettings settings,
        CancellationToken cancellationToken)
    {
        #region Logging Config
        var logLevel = ParseSerilogLevel(settings.LogLevel);
        var logFilePath = "logs/ZenMonitor.log";

        Directory.CreateDirectory("logs");
        File.WriteAllText(logFilePath, string.Empty);

        var loggerConfig = new LoggerConfiguration()
            .MinimumLevel.Is(logLevel)
            .Enrich.WithProperty("RunId", Guid.NewGuid());

        if (settings.CliLogging)
        {
            loggerConfig.WriteTo.Console(
                outputTemplate:
                    "[{Timestamp:HH:mm:ss.fff} {Level:u3}] {Message:lj}{NewLine}{Exception}");
        }

        loggerConfig.WriteTo.File(
            logFilePath,
            outputTemplate:
                "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{RunId}] [{SourceContext}] {Message:lj}{NewLine}{Exception}");
        Log.Logger = loggerConfig.CreateLogger();
        #endregion

        #region Dependency Injection
        try
        {
            var services = new ServiceCollection();

            services.AddLogging(builder =>
            {
                builder.ClearProviders();
                builder.AddSerilog(dispose: true);
            });

            bool gpuNotSupported = false;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                services.AddSingleton<ICpuService, Core.Services.Linux.Cpu>();

                if (Directory.Exists("/proc/driver/nvidia"))
                    services.AddSingleton<IGpuService, Core.Services.Linux.GpuNvidia>();
                else if (Directory.Exists("/sys/class/drm/card0/device/hwmon"))
                    services.AddSingleton<IGpuService, Core.Services.Linux.GpuAmd>();
                else
                {
                    services.AddSingleton<IGpuService, Core.Services.Linux.GpuNull>();
                    gpuNotSupported = true;
                }

                services.AddSingleton<IMemoryService, Core.Services.Linux.Memory>();
                services.AddSingleton<INetworkService, Core.Services.Linux.Network>();
                services.AddSingleton<IStorageService, Core.Services.Linux.Storage>();
                services.AddSingleton<ISystemService, Core.Services.Linux.System>();
            }
            else
            {
                throw new PlatformNotSupportedException(
                    "ZenMonitor only supports Linux at the moment. Windows support will come in the future."
                );
            }

            services.AddTransient<MonitorEngine>();

            var serviceProvider = services.BuildServiceProvider();
            var _logger = serviceProvider.GetRequiredService<ILogger<MonitorCommand>>();
            #endregion

            #region Init Application
            _logger.LogWarning("ZenMonitor initialized.");

            if (gpuNotSupported)
            {
                _logger.LogError("Unsupported GPU. Falling back to `GpuNull`, no graphics information will be returned.");
            }

            if (settings.LoopDelay > 10)
            {
                settings.LoopDelay = 10;
                _logger.LogWarning("LoopDelay Exceeds 10 seconds. Setting back to a maximum of 10");
            }
            settings.LoopDelay *= 1000; // Used for Thread.Sleep()

            using var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (sender, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
            };

            // TODO: Re-Add services on next commit...
            _logger.LogInformation("OutputMode: {OutputMode}", settings.OutputMode);
            if (settings.OutputMode == "cli")
            {
                //var engine = serviceProvider.GetRequiredService<>();
            }
            else if (settings.OutputMode == "gui")
            {

            }
            else if (settings.OutputMode == "debug")
            {

            }
            else
            {
                _logger.LogCritical("Something really unexpected happened. Couldnt figure out what user interface to use: {OutputMode}", settings.OutputMode);
            }

            _logger.LogInformation("Application Finished");

            return 0;
            #endregion
        }
        finally
        {
            Log.CloseAndFlush();
        }
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
