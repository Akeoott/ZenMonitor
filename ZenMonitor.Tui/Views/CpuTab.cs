// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using System.Text;

using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

using ZenMonitor.Core.Interfaces;

namespace ZenMonitor.Tui.Views;

public class CpuTab : View
{
    private readonly ICpu _cpu;
    private readonly Label _cpuNameLabel;
    private readonly Label _usageLabel;
    private readonly ProgressBar _usageBar;
    private readonly TextView _coreInfo;

    public CpuTab(ICpu cpu)
    {
        _cpu = cpu;
        Width = Dim.Fill();
        Height = Dim.Fill();

        var nameFrame = new FrameView
        {
            Title = "CPU Info",
            X = 0,
            Y = 0,
            Width = Dim.Percent(50),
            Height = 3
        };
        _cpuNameLabel = new Label { X = 1, Y = 0, Text = "CPU: ..." };
        nameFrame.Add(_cpuNameLabel);
        Add(nameFrame);

        var usageFrame = new FrameView
        {
            Title = "Usage",
            X = Pos.Right(nameFrame),
            Y = 0,
            Width = Dim.Fill(),
            Height = 3
        };
        _usageLabel = new Label { X = 1, Y = 0, Text = "0%" };
        _usageBar = new ProgressBar
        {
            X = 1,
            Y = 1,
            Width = Dim.Fill() - 2,
            Height = 1,
            Fraction = 0
        };
        usageFrame.Add(_usageLabel, _usageBar);
        Add(usageFrame);

        var coresFrame = new FrameView
        {
            Title = "Cores",
            X = 0,
            Y = Pos.Bottom(nameFrame) + 1,
            Width = Dim.Fill(),
            Height = Dim.Fill()
        };
        _coreInfo = new TextView
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            ReadOnly = true
        };
        coresFrame.Add(_coreInfo);
        Add(coresFrame);
    }

    public void Refresh()
    {
        var name = _cpu.GetCpuName();
        var speed = _cpu.GetCpuSpeed();
        var usage = _cpu.GetCpuUsage();
        _cpuNameLabel.Text = $"CPU: {name} @ {speed} MHz";
        _usageLabel.Text = $"{usage}%";
        _usageBar.Fraction = usage / 100f;

        var sb = new StringBuilder();
        var speeds = _cpu.GetCoreSpeeds();
        var usages = _cpu.GetCoreUsages();
        var temps = _cpu.GetCoreTemps();

        for (int i = 0; i < speeds.Length; i++)
        {
            sb.AppendLine(
                $"Core {i,2}: {usages[i].Usage,5:F1}%  {temps[i].Temp,3}°C  {speeds[i].Speed,6:F1} MHz");
        }
        _coreInfo.Text = sb.ToString();
    }
}
