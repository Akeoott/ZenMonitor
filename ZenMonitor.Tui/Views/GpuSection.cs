// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

using ZenMonitor.Core.Abstractions;

namespace ZenMonitor.Tui.Views;

public sealed class GpuSection : View
{
    private readonly IGpu _gpu;
    private readonly FrameView _frame;
    private readonly Label _nameLabel;
    private readonly Label _gpuUsageLabel;
    private readonly ProgressBar _gpuUsageBar;
    private readonly Label _memUsageLabel;
    private readonly ProgressBar _memUsageBar;
    private readonly Label _extraLabel;

    public GpuSection(IGpu gpu)
    {
        _gpu = gpu;
        Width = Dim.Fill();

        _frame = new FrameView
        {
            Title = "GPU [2]",
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill()
        };

        _nameLabel = new Label
        {
            X = 1,
            Y = 0,
            Text = "GPU: ..."
        };

        _gpuUsageLabel = new Label
        {
            X = 1,
            Y = 1,
            Text = "GPU: 0%"
        };

        _gpuUsageBar = new ProgressBar
        {
            X = 10,
            Y = 1,
            Width = Dim.Fill() - 12,
            Height = 1,
            Fraction = 0
        };

        _memUsageLabel = new Label
        {
            X = 1,
            Y = 2,
            Text = "Mem: 0%"
        };

        _memUsageBar = new ProgressBar
        {
            X = 10,
            Y = 2,
            Width = Dim.Fill() - 12,
            Height = 1,
            Fraction = 0
        };

        _extraLabel = new Label
        {
            X = 1,
            Y = 3,
            Text = "Temp: --°C   Power: --W   State: --"
        };

        _frame.Add(_nameLabel, _gpuUsageLabel, _gpuUsageBar, _memUsageLabel, _memUsageBar, _extraLabel);
        Add(_frame);
    }

    public void Refresh()
    {
        string name = _gpu.GetGpuName();
        int gpuUsage = _gpu.GetUsageGpu();
        int memUsage = _gpu.GetUsageMemory();
        double memUsed = _gpu.GetMemoryUsed();
        double memTotal = _gpu.GetMemoryTotal();
        int temp = _gpu.GetTemperatureGpu();
        string powerState = _gpu.GetPowerState();
        double powerDraw = _gpu.GetPowerDraw();

        _nameLabel.Text = $"GPU: {name}";
        _gpuUsageLabel.Text = $"GPU: {gpuUsage}%";
        _gpuUsageBar.Fraction = gpuUsage / 100f;
        _memUsageLabel.Text = $"Mem: {memUsage}%  ({memUsed:F1}/{memTotal:F1} GiB)";
        _memUsageBar.Fraction = memUsage / 100f;
        _extraLabel.Text = $"Temp: {temp}°C   Power: {powerDraw:F1}W   State: {powerState}";
    }
}
