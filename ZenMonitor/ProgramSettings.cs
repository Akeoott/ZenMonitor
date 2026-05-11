// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using System.ComponentModel;

using Spectre.Console;
using Spectre.Console.Cli;

namespace ZenMonitor;

public class ProgramSettings : CommandSettings
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

    [CommandOption("-f|--force-run <bool>")]
    [Description(
        "Run ZenMonitor regardless of what OS you're on.\n" +
        "(no data will be returned if your OS is not supported! some things might break!)")]
    [DefaultValue("false")]
    public bool ForceRun { get; set; } = false;

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
