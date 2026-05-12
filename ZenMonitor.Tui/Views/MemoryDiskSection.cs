// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using System.Text;

using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

using ZenMonitor.Core.Interfaces;
using ZenMonitor.Core.Models;

namespace ZenMonitor.Tui.Views;

/// <summary>
/// Unified Memory + Disk section displayed side-by-side with a vertical divider.
/// Outer FrameView titled "Mem+Disk [3]", inner FrameViews titled "Memory" and "Disks".
/// </summary>
public sealed class MemoryDiskSection : View
{
    private readonly IMemory _memory;
    private readonly IDrive _drive;

    private readonly FrameView _memoryFrame;
    private readonly Label _infoLabel;
    private readonly Label _usageLabel;
    private readonly ProgressBar _usageBar;
    private readonly Label _swapLabel;

    private readonly FrameView _diskFrame;
    private readonly TextView _driveInfo;

    public MemoryDiskSection(IMemory memory, IDrive drive)
    {
        _memory = memory;
        _drive = drive;

        Width = Dim.Fill();
        Height = Dim.Fill();

        var outerFrame = new FrameView
        {
            Title = "Mem+Disk [3]",
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill()
        };

        _memoryFrame = new FrameView
        {
            Title = "Memory",
            X = 0,
            Y = 0,
            Width = Dim.Percent(50),
            Height = Dim.Fill()
        };

        _infoLabel = new Label
        {
            X = 1,
            Y = 0,
            Text = "Total: -- GiB   Used: -- GiB"
        };

        _usageLabel = new Label
        {
            X = 1,
            Y = 1,
            Text = "--%"
        };

        _usageBar = new ProgressBar
        {
            X = 8,
            Y = 1,
            Width = Dim.Fill() - 10,
            Height = 1,
            Fraction = 0
        };

        _swapLabel = new Label
        {
            X = 1,
            Y = 2,
            Text = "Swap: -- / -- GiB"
        };

        _memoryFrame.Add(_infoLabel, _usageLabel, _usageBar, _swapLabel);

        _diskFrame = new FrameView
        {
            Title = "Disks",
            X = Pos.Percent(50),
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill()
        };

        _driveInfo = new TextView
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            ReadOnly = true,
            CanFocus = false
        };

        _diskFrame.Add(_driveInfo);

        outerFrame.Add(_memoryFrame, _diskFrame);
        Add(outerFrame);
    }

    public void Refresh()
    {
        double total = _memory.GetMemTotal();
        double used = _memory.GetMemUsed();
        double free = _memory.GetMemFree();
        double avail = _memory.GetMemAvailable();
        double cached = _memory.GetCached();
        double swapTotal = _memory.GetSwapTotal();
        double swapFree = _memory.GetSwapFree();
        double usagePercent = total > 0 ? used / total * 100.0 : 0;

        _infoLabel.Text = $"Total: {total,5:F1} GiB   Used: {used,5:F1} GiB";
        _usageLabel.Text = $"{usagePercent,5:F1}%";
        _usageBar.Fraction = (float)(usagePercent / 100.0);
        _swapLabel.Text = $"Swap: {swapFree,5:F1} / {swapTotal,5:F1} GiB";

        var mountInfos = _drive.GetMountInfos();
        var sb = new StringBuilder();

        foreach (var mount in mountInfos)
        {
            double usagePercentD = mount.TotalBytes > 0
                ? (double)mount.UsedBytes / mount.TotalBytes * 100.0
                : 0;

            int barLen = (int)(usagePercentD / 100.0 * 16);
            string bar = new string('█', Math.Min(barLen, 16)).PadRight(16, '░');

            string usedStr = FormatBytes(mount.UsedBytes);
            string totalStr = FormatBytes(mount.TotalBytes);

            sb.AppendLine(
                $"{mount.MountPoint,-12}{bar} {usagePercentD,4:F1}%  " +
                $"{usedStr,5}/{totalStr}");
        }

        if (mountInfos.Length == 0)
        {
            sb.AppendLine("No mount info");
        }

        _driveInfo.Text = sb.ToString();
    }

    private static string FormatBytes(long bytes)
    {
        const double GiB = 1024.0 * 1024 * 1024;
        return bytes >= GiB
            ? $"{bytes / GiB,4:F1} GiB"
            : $"{bytes / (1024.0 * 1024),4:F1} MiB";
    }
}
