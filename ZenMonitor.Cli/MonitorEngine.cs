// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using ZenMonitor.Core.Interfaces;
using ZenMonitor.Core.Models;

namespace ZenMonitor.Cli;

internal class MonitorEngine(IHardwareService hardware)
{
    private readonly IHardwareService _hardware = hardware;

    public void UpdateDashboard()
    {
        var usages = _hardware.GetCoreUsages();

        Console.WriteLine("Testing Raw output rn");

        foreach (var usage in usages)
        {
            var cpuUsage = new CpuUsage(usage.Index, usage.Usage);
            Console.WriteLine(cpuUsage);
        }
    }
}

