// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using System.Runtime.Versioning;

using Microsoft.Extensions.Logging;

using ZenMonitor.Core.Interfaces;
using ZenMonitor.Core.Models;

namespace ZenMonitor.Core.Services.Linux;

[SupportedOSPlatform("linux")]
public class GpuAmd(ILogger<GpuAmd> logger) : IGpu
{
    private readonly ILogger<GpuAmd> _logger = logger;
    private GpuInfoSnapshot _snapshot = new(
        "", 0, 0, 0.0, 0.0, 0, "", 0.0);

    public void Update() => _snapshot = FetchGpuInfo();

    public string GetGpuName() => _snapshot.GpuName;
    public int GetUsageGpu() => _snapshot.UsageGpu;
    public int GetUsageMemory() => _snapshot.UsageMemory;
    public double GetMemoryUsed() => _snapshot.MemoryUsed;
    public double GetMemoryTotal() => _snapshot.MemoryTotal;
    public int GetTemperatureGpu() => _snapshot.TemperatureGpu;
    public string GetPowerState() => _snapshot.PowerState;
    public double GetPowerDraw() => _snapshot.PowerDraw;

    // TODO: Once implementation is done, update unit tests!
    private GpuInfoSnapshot FetchGpuInfo()
    {
        _logger.LogTrace("Fetching all GpuAmd info...");
        _logger.LogWarning("Amd GPU's are currently not supported!");

        return new GpuInfoSnapshot(
            "", 0, 0, 0.0, 0.0, 0, "", 0.0);
    }
}
