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

public abstract partial class Config : CommandSettings
{
    [CommandOption("-d|--delay <ms>")]
    [Description("Change the delay before updating, min to max is 100ms to 10000ms")]
    [DefaultValue(1000)]
    private int Delay { get; } = 1000;

    [CommandOption("-f|--force <bool>")]
    [Description("Run ZenMonitor without sudo/admin privileges (some things might not work!)")]
    [DefaultValue("false")]
    public bool Force { get; set; } = false;

    [CommandOption("-v|--verbosity <level>")]
    [Description(
        "Set logging verbosity: `t|trace`, `d|debug`, `i|info`, `w|warning`, `e|error`, `c|critical`.\n" +
        "Logs are written to `logs/ZenMonitor.log` (cleared on each run)")]
    [DefaultValue("info")]
    public string Verbosity { get; set; } = "info";

    [CommandOption("-q|--quiet <bool>")]
    [Description("Suppress console log output.")]
    [DefaultValue("false")]
    public bool Quiet { get; set; } = false;

    [LibraryImport("libc")]
    private static partial uint geteuid();

    private static bool IsRoot()
    {
        return RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && geteuid() == 0;
    }

    private static bool IsAdmin()
    {
        return RuntimeInformation.IsOSPlatform(OSPlatform.Windows) &&
               new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
    }

    public override ValidationResult Validate()
    {
        if (Delay is < 100 or > 10000) return ErrorResult("Loop delay must be between 100 and 10000 milliseconds.");

        if (Force) return ValidationResult.Success();

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && !IsRoot())
            return ErrorResult("ZenMonitor requires root privileges. Please run with sudo.");

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && !IsAdmin())
            return ErrorResult("ZenMonitor requires admin privileges. Please run as Administrator.");
        return ValidationResult.Success();

        static ValidationResult ErrorResult(string message)
        {
            AnsiConsole.MarkupLine("[Yellow3_1]Use `--help` for more options.[/]");
            return ValidationResult.Error(message);
        }
    }

    internal static void ConfigureLogging(bool isQuiet, LogEventLevel logLevel, string logFilePath)
    {
        Directory.CreateDirectory("logs");
        File.WriteAllText(logFilePath, string.Empty);

        var loggerConfig = new LoggerConfiguration()
            .MinimumLevel.Is(logLevel)
            .Enrich.WithProperty("RunId", Guid.NewGuid());

        const string outputTemplate =
            "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{RunId}] [{SourceContext}] {Message:lj}{NewLine}{Exception}";

        loggerConfig.WriteTo.File(
            logFilePath,
            outputTemplate: outputTemplate);

        if (!isQuiet)
            loggerConfig.WriteTo.Console(
                outputTemplate: outputTemplate);

        Log.Logger = loggerConfig.CreateLogger();
    }

    internal static LogEventLevel ParseSerilogLevel(string? level)
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
