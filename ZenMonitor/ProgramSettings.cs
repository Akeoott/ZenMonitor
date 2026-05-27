// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.Principal;

using Spectre.Console;
using Spectre.Console.Cli;

namespace ZenMonitor;

public class ProgramSettings : CommandSettings
{
    #region Cli Options
    [CommandOption("-r|--run <tui|gui>")]
    [Description(
        "Available modes:\n" +
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
    #endregion

    #region Cli Validation
    [DllImport("libc")]
    private static extern uint geteuid();

    private static bool IsRoot() => RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && geteuid() == 0;
    private static bool IsAdmin() => RuntimeInformation.IsOSPlatform(OSPlatform.Windows) &&
        new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);

    public override ValidationResult Validate()
    {
        Mode = Mode?.ToLowerInvariant() ?? string.Empty;

        if (Mode != "gui" && Mode != "tui")
        {
            return ValidationResult.Error(
                "\tRequire mode arguments (`--run <tui|gui>`).\n\tUse `--help` for more information.");
        }

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
            return ValidationResult.Error("ZenMonitor requires root privileges. Please run with [yellow]sudo[/].");
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && !IsAdmin())
        {
            return ValidationResult.Error("ZenMonitor requires admin privileges. Please run as [yellow]Administrator[/].");
        }
        return ValidationResult.Success();
    }
    #endregion
}
