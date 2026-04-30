// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using System.Runtime.Versioning;

using Microsoft.Extensions.Logging;

using ZenMonitor.Core.Interfaces;
using ZenMonitor.Core.Models;

namespace ZenMonitor.Core.Services.Linux;

[SupportedOSPlatform("linux")]
public class GpuNull(ILogger<GpuNull> logger) : IGpuService
{
    private readonly ILogger<GpuNull> _logger = logger;
    private readonly GpuInfoSnapshot _snapshot = new(
        "", "", "", "", "", "", "", "");

    public void Update() => _logger.LogTrace("Gpu not supported, using GpuNull...");

    public string GetGpuName() => _snapshot.GpuName;
    public string GetUsageGpu() => _snapshot.UsageGpu;
    public string GetUsageMemory() => _snapshot.UsageMemory;
    public string GetMemoryUsed() => _snapshot.MemoryUsed;
    public string GetMemoryTotal() => _snapshot.MemoryTotal;
    public string GetTemperatureGpu() => _snapshot.TemperatureGpu;
    public string GetPowerState() => _snapshot.PowerState;
    public string GetPowerDraw() => _snapshot.PowerDraw;
}
