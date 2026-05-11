// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using Terminal.Gui.Drawing;
using Terminal.Gui.Input;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

using ZenMonitor.Core.Interfaces;

namespace ZenMonitor.Tui.Views;

public sealed class Window : Runnable<bool>
{
    private readonly CpuTab _cpuTab;
    //private readonly GpuTab _gpuTab;
    //private readonly MemoryTab _memoryTab;
    //private readonly DrivesTab _drivesTab;
    //private readonly NetworkTab _networkTab;
    //private readonly SystemTab _systemTab;

    public Window(
        ICpu cpu,
        IGpu gpu,
        IMemory memory,
        IDrive drive,
        INetwork network,
        ISystem system)
    {
        Title = "ZenMonitor";
        BorderStyle = LineStyle.Rounded;

        _cpuTab = new CpuTab(cpu);
        //_gpuTab = new GpuTab(gpu);
        //_memoryTab = new MemoryTab(memory);
        //_drivesTab = new DrivesTab(drive);
        //_networkTab = new NetworkTab(network);
        //_systemTab = new SystemTab(system);

        var tabView = new TabView
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill()
        };

        var cpuTabObj = new Tab { View = _cpuTab, Text = "CPU" };
        //var gpuTabObj = new Tab { View = _gpuTab, Text = "GPU" };
        //var memTabObj = new Tab { View = _memoryTab, Text = "Memory" };
        //var drvTabObj = new Tab { View = _drivesTab, Text = "Drives" };
        //var netTabObj = new Tab { View = _networkTab, Text = "Network" };
        //var sysTabObj = new Tab { View = _systemTab, Text = "System" };

        tabView.AddTab(cpuTabObj, true);
        //tabView.AddTab(gpuTabObj, false);
        //tabView.AddTab(memTabObj, false);
        //tabView.AddTab(drvTabObj, false);
        //tabView.AddTab(netTabObj, false);
        //tabView.AddTab(sysTabObj, false);

        Add(tabView);

        AddCommand(Command.Quit, () =>
        {
            App?.RequestStop();
            return true;
        });
        KeyBindings.Add(Key.Esc, Command.Quit);
    }

    public void RefreshAll()
    {
        _cpuTab.Refresh();
        //_gpuTab.Refresh();
        //_memoryTab.Refresh();
        //_drivesTab.Refresh();
        //_networkTab.Refresh();
        //_systemTab.Refresh();
    }
}
