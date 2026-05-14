// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using Terminal.Gui.ViewBase;

using ZenMonitor.Core.Abstractions;

namespace ZenMonitor.Tui.Views;

/// <summary>
/// A compact 1-line status bar showing hostname, kernel version, uptime, and task counts.
/// Pinned at the top of the window (Y=0, Height=1).
/// </summary>
public sealed class HeaderView : View
{
    private readonly ISystem _systemInfo;

    public HeaderView(ISystem systemInfo)
    {
        _systemInfo = systemInfo;

        X = 0;
        Y = 0;
        Width = Dim.Fill();
        Height = 1;

        CanFocus = false;

        UpdateText();
    }

    public void Refresh()
    {
        UpdateText();
    }

    private void UpdateText()
    {
        string hostname = Truncate(_systemInfo.GetHostname(), 24);
        string kernel = Truncate(_systemInfo.GetKernelVersion(), 28);
        double uptimeSec = _systemInfo.GetUptimeSeconds();
        int runningTasks = _systemInfo.GetRunningTasks();
        int totalTasks = _systemInfo.GetTotalTasks();

        string uptime = FormatUptime(uptimeSec);

        Text = $" {hostname} | Kernel: {kernel} | Up: {uptime} | Tasks: {runningTasks}/{totalTasks} ";
    }

    private static string FormatUptime(double seconds)
    {
        var ts = TimeSpan.FromSeconds(seconds);
        if (ts.TotalDays >= 1)
            return $"{(int)ts.TotalDays}d {ts.Hours}h {ts.Minutes}m";
        if (ts.TotalHours >= 1)
            return $"{(int)ts.TotalHours}h {ts.Minutes}m {ts.Seconds}s";
        return $"{(int)ts.TotalMinutes}m {ts.Seconds}s";
    }

    private static string Truncate(string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value)) return "N/A";
        return value.Length <= maxLength ? value : value[..(maxLength - 3)] + "...";
    }
}
