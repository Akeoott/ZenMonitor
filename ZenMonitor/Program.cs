// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.Principal;

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
    [CommandOption("-r|--run <cli|tui|gui>")]
    [Description(
        "Available modes:\n" +
        "\tcli (Command Line Interface Providing Raw Values)\n" +
        "\ttui (Terminal User Interface)\n" +
        "\tgui (Graphical User Interface)\n")]
    public required string Mode { get; set; }

    [CommandOption("-d|--delay <ms>")]
    [Description("Change the delay before updating, min to max is 100ms to 10000ms")]
    [DefaultValue(1000)]
    public int LoopDelay { get; set; } = 1000;

    [CommandOption("-n|--no-sudo <bool>")]
    [Description("Run ZenMonitor without sudo (some things might not work!)")]
    [DefaultValue("false")]
    public bool NoSudo { get; set; } = false;

    [CommandOption("-c|--cli-log <bool>")]
    [Description("Enable console logging. Use `--cli-log true` to enable. (Mode has to be set to cli)")]
    [DefaultValue("false")]
    public bool CliLogging { get; set; } = false;

    [CommandOption("-l|--log-level <level>")]
    [Description(
        "Set logging verbosity: `t|trace`, `d|debug`, `i|info`, `w|warning`, `e|error`, `c|critical`.\n" +
        "Logs are written to `logs/ZenMonitor.log` (cleared on each run)")]
    [DefaultValue("info")]
    public string LogLevel { get; set; } = "info";
    #endregion

    #region Cli Validation
    public override ValidationResult Validate()
    {
        Mode = Mode?.ToLowerInvariant() ?? string.Empty;

        if (Mode != "cli" && Mode != "gui" && Mode != "tui")
        {
            return ValidationResult.Error(
                "Require mode arguments (`--run <cli|tui|gui>`). Use `--help` for more information.");
        }

        if (CliLogging && Mode != "cli")
        {
            return ValidationResult.Error(
                "When --cli-log is enabled, mode must be `cli`.");
        }

        if (LoopDelay < 100 || LoopDelay > 10000)
        {
            return ValidationResult.Error(
                "--delay must be between 100 and 10000 milliseconds.");
        }

        return ValidationResult.Success();
    }
    #endregion
}

public class MonitorCommand : AsyncCommand<MonitorSettings>
{
    [DllImport("libc")]
    private static extern uint geteuid();

    private static bool IsRoot() => RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && geteuid() == 0;
    private static bool IsAdmin() => RuntimeInformation.IsOSPlatform(OSPlatform.Windows) &&
        new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);

    protected override async Task<int> ExecuteAsync(
        CommandContext context,
        MonitorSettings settings,
        CancellationToken cancellationToken)
    {
        if (!settings.NoSudo)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && !IsRoot())
            {
                Console.Error.WriteLine("ZenMonitor requires root privileges. Please run with sudo.");
                return 1;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && !IsAdmin())
            {
                Console.Error.WriteLine("ZenMonitor requires admin privileges. Please run as admin.");
                return 1;
            }
        }

        try
        {
            var logLevel = ParseSerilogLevel(settings.LogLevel);
            const string logFilePath = "logs/ZenMonitor.log";

            ConfigureLogging(logLevel, logFilePath, settings.CliLogging);

            using var serviceProvider = BuildServiceProvider(settings, out var gpuNotSupported);
            var logger = serviceProvider.GetRequiredService<ILogger<MonitorCommand>>();

            ApplyRuntimeSafetyChecks(settings, logger, gpuNotSupported);

            await RunApplicationAsync(serviceProvider, settings, cancellationToken);
            logger.LogInformation("Application finished, bye bye!");

            return 0;
        }
        catch (PlatformNotSupportedException ex)
        {
            Console.Error.WriteLine(ex.Message);
            Console.Write("Press any key to exit... ");
            Console.ReadKey();
            return 1;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    #region Logging Config
    private static void ConfigureLogging(LogEventLevel logLevel, string logFilePath, bool cliLogging)
    {
        Directory.CreateDirectory("logs");
        File.WriteAllText(logFilePath, string.Empty);

        var loggerConfig = new LoggerConfiguration()
            .MinimumLevel.Is(logLevel)
            .Enrich.WithProperty("RunId", Guid.NewGuid());

        if (cliLogging)
        {
            loggerConfig.WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss.fff} {Level:u3}] {Message:lj}{NewLine}{Exception}");
        }

        loggerConfig.WriteTo.File(
            logFilePath,
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{RunId}] [{SourceContext}] {Message:lj}{NewLine}{Exception}");

        Log.Logger = loggerConfig.CreateLogger();
    }
    #endregion

    #region Dependency Injection
    private static ServiceProvider BuildServiceProvider(MonitorSettings settings, out bool gpuNotSupported)
    {
        var services = new ServiceCollection();

        services.AddLogging(builder =>
        {
            builder.ClearProviders();
            builder.AddSerilog(dispose: true);
        });

        services.AddSingleton<System.IO.Abstractions.IFileSystem, System.IO.Abstractions.FileSystem>();

        gpuNotSupported = false;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            services.AddSingleton<IHelper, Core.Services.Linux.Helper>();
            services.AddSingleton<ICpu, Core.Services.Linux.Cpu>();

            if (Directory.Exists("/proc/driver/nvidia"))
            {
                services.AddSingleton<IGpu, Core.Services.Linux.GpuNvidia>();
            }
            else if (Directory.Exists("/sys/class/drm/card0/device/hwmon"))
            {
                services.AddSingleton<IGpu, Core.Services.Linux.GpuAmd>();
            }
            else
            {
                services.AddSingleton<IGpu, Core.Services.Linux.GpuNull>();
                gpuNotSupported = true;
            }

            services.AddSingleton<IMemory, Core.Services.Linux.Memory>();
            services.AddSingleton<INetwork, Core.Services.Linux.Network>();
            services.AddSingleton<IDrive, Core.Services.Linux.Drive>();
            services.AddSingleton<ISystem, Core.Services.Linux.System>();
        }
        else
        {
            throw new PlatformNotSupportedException(
                "ZenMonitor only supports Linux at the moment. Windows support will come in the future.");
        }

        switch (settings.Mode)
        {
            case "cli":
                services.AddTransient<Cli.Monitor>();
                break;
            case "tui":
                //services.AddTransient<Tui.Monitor>();
                break;
            case "gui":
                //services.AddTransient<Gui.Monitor>();
                break;
        }

        return services.BuildServiceProvider();
    }
    #endregion

    #region Log + Safety Checks
    private static void ApplyRuntimeSafetyChecks(MonitorSettings settings, Microsoft.Extensions.Logging.ILogger logger, bool gpuNotSupported)
    {
        logger.LogWarning("ZenMonitor initialized.");

        if (settings.NoSudo)
        {
            logger.LogWarning("Bypassing sudo/admin requirements!");
        }

        if (gpuNotSupported)
        {
            logger.LogError("Unsupported GPU. Falling back to `GpuNull`, no graphics information will be returned.");
        }

        logger.LogInformation("OutputMode: {OutputMode}", settings.Mode);

        if (settings.LoopDelay > 10000)
        {
            settings.LoopDelay = 10000;
            logger.LogWarning("LoopDelay exceeds 10 seconds. Setting back to a maximum of 10 seconds.");
        }
        else if (settings.LoopDelay < 100)
        {
            settings.LoopDelay = 100;
            logger.LogWarning("LoopDelay is below 0.1 seconds. Setting back to a minimum of 0.1 seconds.");
        }
    }
    #endregion

    #region Init Application
    private static async Task RunApplicationAsync(ServiceProvider serviceProvider, MonitorSettings settings, CancellationToken cancellationToken)
    {
        switch (settings.Mode)
        {
            case "cli":
                {
                    var engine = serviceProvider.GetRequiredService<Cli.Monitor>();
                    await engine.InitMonitor(settings.LoopDelay, cancellationToken);
                    break;
                }
            case "tui":
                Console.WriteLine("tui is not implemented, come back later! (try cli)");
                break;
            case "gui":
                Console.WriteLine("gui is not implemented, come back later! (try cli)");
                break;
            default:
                throw new InvalidOperationException($"Unsupported mode: {settings.Mode}");
        }
    }
    #endregion

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
