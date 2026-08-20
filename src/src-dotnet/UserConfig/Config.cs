// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

namespace ZenMonitor.UserConfig;

public record Config
{
    public string Theme
    {
        get;
        init => field = value is "dark" or "light"
            ? value : "dark";
    }

    public string LogLevel
    {
        get;
        init => field = value
            is "verbose" or "debug"
            or "info" or "information"
            or "warn" or "warning"
            or "error" or "fatal"
            ? value : "info";
    }

    public int Delay
    {
        get;
        init => field = value is >= 500 and <= 10000
            ? value : 1000;
    }

    public Config() : this("dark", "info", 1000) { }

    private Config(string theme, string logLevel, int delay)
    {
        Theme = theme;
        LogLevel = logLevel;
        Delay = delay;
    }
}
