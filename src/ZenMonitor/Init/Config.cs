// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.Principal;

using Serilog;
using Serilog.Events;

using Spectre.Console;
using Spectre.Console.Cli;

namespace ZenMonitor.Init;

public class Config : CommandSettings
{
    [CommandOption("-d|--delay <ms>")]
    [Description("Change the delay before updating, min to max is 100ms to 10000ms")]
    [DefaultValue(1000)]
    public int LoopDelay { get; set; } = 1000;

    [CommandOption("-n|--no-sudo <bool>")]
    [Description("Run ZenMonitor without sudo (some things might not work!)")]
    [DefaultValue("false")]
    public bool NoSudo { get; set; } = false;

    [CommandOption("-f|--force-run <bool>")]
    [Description(
        "Run ZenMonitor regardless of what OS you're on.\n" +
        "(no data will be returned if your OS is not supported! some things might break!)")]
    [DefaultValue("false")]
    public bool ForceRun { get; set; } = false;

    [CommandOption("-l|--log-level <level>")]
    [Description(
        "Set logging verbosity: `t|trace`, `d|debug`, `i|info`, `w|warning`, `e|error`, `c|critical`.\n" +
        "Logs are written to `logs/ZenMonitor.log` (cleared on each run)")]
    [DefaultValue("info")]
    public string LogLevel { get; set; } = "info";

    [DllImport("libc")]
    private static extern uint geteuid();

    private static bool IsRoot() => RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && geteuid() == 0;
    private static bool IsAdmin() => RuntimeInformation.IsOSPlatform(OSPlatform.Windows) &&
        new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);

    public override ValidationResult Validate()
    {
        if (LoopDelay < 100 || LoopDelay > 10000)
        {
            return ValidationResult.Error("Loop delay must be between 100 and 10000 milliseconds.");
        }

        if (NoSudo)
        {
            return ValidationResult.Success();
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && !IsRoot())
        {
            return ValidationResult.Error("ZenMonitor requires root privileges. Please run with sudo.");
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && !IsAdmin())
        {
            return ValidationResult.Error("ZenMonitor requires admin privileges. Please run as Administrator.");
        }
        return ValidationResult.Success();
    }

    internal static void ConfigureLogging(LogEventLevel logLevel, string logFilePath)
    {
        Directory.CreateDirectory("logs");
        File.WriteAllText(logFilePath, string.Empty);

        var loggerConfig = new LoggerConfiguration()
            .MinimumLevel.Is(logLevel)
            .Enrich.WithProperty("RunId", Guid.NewGuid());

        loggerConfig.WriteTo.File(
            logFilePath,
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{RunId}] [{SourceContext}] {Message:lj}{NewLine}{Exception}");

        Log.Logger = loggerConfig.CreateLogger();
    }

    internal static LogEventLevel ParseSerilogLevel(string level)
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
