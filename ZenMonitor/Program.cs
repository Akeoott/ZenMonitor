// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using System.ComponentModel;
using System.Diagnostics;
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
    [DllImport("libc")]
    private static extern uint geteuid();

    private static bool IsRoot() => RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && geteuid() == 0;
    private static bool IsAdmin() => RuntimeInformation.IsOSPlatform(OSPlatform.Windows) &&
        new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);

    internal static async Task<int> Main(string[] args)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && !IsRoot())
        {
            Console.Error.WriteLine("ZenMonitor requires root privileges. Please run with sudo.");
            return 1;
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && !IsAdmin())
        {
            Console.Error.WriteLine("Elevating to administrator...");
            var psi = new ProcessStartInfo
            {
                FileName = Environment.ProcessPath,
                Arguments = string.Join(" ", args),
                UseShellExecute = true,
                Verb = "runas"
            };
            try
            {
                Process.Start(psi);
                return 0;
            }
            catch (Win32Exception) { /* canceled */ }
            return 1;
        }

        var app = new CommandApp<MonitorCommand>();
        return await app.RunAsync(args);
    }
}

public class MonitorSettings : CommandSettings
{
    #region Cli Options
    [CommandOption("-r|--run <value>")]
    [Description(
        "Available modes:\n" +
        "\tcli (Raw Values)\n" +
        "\ttui (Terminal User Interface)\n" +
        "\tgui (Graphical User Interface)\n")]
    public required string Mode { get; set; }

    [CommandOption("-d|--delay <VALUE>")]
    [Description("Change the delay before updating, min to max is 100ms to 10000ms")]
    [DefaultValue(1000)]
    public int LoopDelay { get; set; } = 1000;

    [CommandOption("-c|--cli-log <BOOL>")]
    [Description("Enable console logging. Use `--cli-log true` to enable. (Mode has to be set to cli)")]
    [DefaultValue("false")]
    public bool CliLogging { get; set; } = false;

    [CommandOption("-l|--log-level <LEVEL>")]
    [Description("Set logging verbosity: c|critical, r|error, w|warning, i|info, d|debug, t|trace.")]
    [DefaultValue("info")]
    public string LogLevel { get; set; } = "info";
    #endregion

    #region Cli Validation
    public override ValidationResult Validate()
    {
        string mode = Mode?.ToLowerInvariant() ?? "";

        if (mode != "cli" && mode != "gui" && mode != "tui")
        {
            return ValidationResult.Error(
                "Require mode arguments (`--run <value>`). Use `--help` for more information.");
        }

        if (CliLogging && mode != "cli")
        {
            return ValidationResult.Error(
                "When --log-cli is enabled, mode must be `cli`.");
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

            services.AddSingleton<System.IO.Abstractions.IFileSystem, System.IO.Abstractions.FileSystem>();
            services.AddSingleton<ITimeService, Core.Services.TimeService>();

            bool gpuNotSupported = false;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                services.AddSingleton<ICpu, Core.Services.Linux.Cpu>();

                if (Directory.Exists("/proc/driver/nvidia"))
                    services.AddSingleton<IGpu, Core.Services.Linux.GpuNvidia>();
                else if (Directory.Exists("/sys/class/drm/card0/device/hwmon"))
                    services.AddSingleton<IGpu, Core.Services.Linux.GpuAmd>();
                else
                {
                    services.AddSingleton<IGpu, Core.Services.Linux.GpuNull>();
                    gpuNotSupported = true;
                }

                services.AddSingleton<IMemory, Core.Services.Linux.Memory>();
                services.AddSingleton<INetwork, Core.Services.Linux.Network>();
                services.AddSingleton<IStorage, Core.Services.Linux.Storage>();
                services.AddSingleton<ISystem, Core.Services.Linux.System>();
            }
            else
            {
                throw new PlatformNotSupportedException(
                    "ZenMonitor only supports Linux at the moment. Windows support will come in the future."
                );
            }

            switch (settings.Mode)
            {
                case "cli":
                    services.AddTransient<Cli.Monitor>();
                    break;
                case "tui":
                    //services.AddTransient<Cli.Monitor>();
                    break;
                case "gui":
                    //services.AddTransient<Gui.Monitor>();
                    break;
            }

            var serviceProvider = services.BuildServiceProvider();
            var _logger = serviceProvider.GetRequiredService<ILogger<MonitorCommand>>();
            #endregion

            #region Init Application
            _logger.LogWarning("ZenMonitor initialized.");

            if (gpuNotSupported)
            {
                _logger.LogError("Unsupported GPU. Falling back to `GpuNull`, no graphics information will be returned.");
            }

            // In milliseconds
            if (settings.LoopDelay > 10000)
            {
                settings.LoopDelay = 10000;
                _logger.LogWarning("LoopDelay Exceeds 10 seconds. Setting back to a maximum of 10 seconds");
            }
            else if (settings.LoopDelay < 100)
            {
                settings.LoopDelay = 100;
                _logger.LogWarning("LoopDelay is below 0.1 seconds. Setting back to a minimum of 0.1 seconds");
            }

            using var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (sender, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
            };

            _logger.LogInformation("OutputMode: {OutputMode}", settings.Mode);
            switch (settings.Mode)
            {
                case "cli":
                    {
                        var engine = serviceProvider.GetRequiredService<Cli.Monitor>();
                        await engine.InitMonitor(settings.LoopDelay, cts.Token);
                        break;
                    }
                case "tui":
                    Console.WriteLine("tui is not implemented, come back later! (try cli)");
                    break;
                case "gui":
                    Console.WriteLine("gui is not implemented, come back later! (try cli)");
                    break;
                default:
                    throw new Exception($"Something really unexpected happened. Couldnt figure out which mode to use: {settings.Mode}");
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
