// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using Microsoft.Extensions.Logging;

using ZenMonitor.Core.Interfaces;
using ZenMonitor.Core.Models;

namespace ZenMonitor.Cli;

// TODO: Dont forget to add logging later!
internal class MonitorEngine(ILogger<MonitorEngine> logger, IHardwareService hardware)
{
    private readonly ILogger<MonitorEngine> _logger = logger;
    private readonly IHardwareService _hardware = hardware;

    public async Task Run()
    {
        await RunLiveDashboard(1000);
    }

    //! Debug stuff right now...
    private async Task RunLiveDashboard(int delay)
    {
        while (true)
        {
            Console.WriteLine(_hardware.GetCpuName());
            Thread.Sleep(1000);
        }
    }
}
