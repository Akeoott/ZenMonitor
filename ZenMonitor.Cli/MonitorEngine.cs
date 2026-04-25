// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using Microsoft.Extensions.Logging;

using ZenMonitor.Core.Interfaces;
using ZenMonitor.Core.Models;

namespace ZenMonitor.Cli;

internal class MonitorEngine(
    ILogger<MonitorEngine> logger,
    ICpuService cpuInfo,
    IGpuService gpuInfo)
{
    private readonly ILogger<MonitorEngine> _logger = logger;
    private readonly ICpuService _cpuInfo = cpuInfo;
    private readonly IGpuService _gpuInfo = gpuInfo;

    public async Task Run()
    {
        await RunLiveDashboard(1000);
    }

    //! Debug stuff right now...
    private async Task RunLiveDashboard(int delay)
    {
        while (true)
        {
            Console.WriteLine(_cpuInfo.GetCpuName());
            Thread.Sleep(1000);
        }
    }
}
