// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using System.Text;

using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

using ZenMonitor.Core.Abstractions;

namespace ZenMonitor.Tui.Views;

public sealed class CpuSection : View
{
    private readonly ICpu _cpu;
    private readonly FrameView _frame;
    private readonly Label _nameLabel;
    private readonly Label _usageLabel;
    private readonly ProgressBar _usageBar;
    private readonly TextView _coreInfo;

    public CpuSection(ICpu cpu)
    {
        _cpu = cpu;
        Width = Dim.Fill();

        _frame = new FrameView
        {
            Title = "CPU [1]",
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill()
        };

        _nameLabel = new Label
        {
            X = 1,
            Y = 0,
            Text = "CPU: ... @ ... MHz"
        };

        _usageLabel = new Label
        {
            X = 1,
            Y = 1,
            Text = "0%"
        };

        _usageBar = new ProgressBar
        {
            X = 8,
            Y = 1,
            Width = Dim.Fill() - 10,
            Height = 1,
            Fraction = 0
        };

        _coreInfo = new TextView
        {
            X = 0,
            Y = 2,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            ReadOnly = true,
            CanFocus = false
        };

        _frame.Add(_nameLabel, _usageLabel, _usageBar, _coreInfo);
        Add(_frame);
    }

    public void Refresh()
    {
        string name = _cpu.GetCpuName();
        double speed = _cpu.GetCpuSpeed();
        int usage = _cpu.GetCpuUsage();
        int temp = _cpu.GetCpuTemp();
        double power = _cpu.GetPowerDraw();

        _nameLabel.Text = $"CPU: {name} @ {speed:F0} MHz  {temp}°C  {power:F1}W";
        _usageLabel.Text = $"{usage}%";
        _usageBar.Fraction = usage / 100f;

        var sb = new StringBuilder();
        var speeds = _cpu.GetCoreSpeeds();
        var usages = _cpu.GetCoreUsages();
        var temps = _cpu.GetCoreTemps();

        int coreCount = Math.Min(Math.Min(speeds.Length, usages.Length), temps.Length);
        for (int i = 0; i < coreCount; i++)
        {
            double coreUsage = usages[i].Usage;
            int barLen = (int)(coreUsage / 100.0 * 20);
            string bar = new string('█', Math.Min(barLen, 20)).PadRight(20, '░');
            sb.AppendLine(
                $"Core {i,2}: {bar} {coreUsage,5:F1}%  {temps[i].Temp,3}°C  {speeds[i].Speed,6:F0} MHz");
        }

        _coreInfo.Text = sb.ToString();
    }
}
