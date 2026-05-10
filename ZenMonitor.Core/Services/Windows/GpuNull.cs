// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using System.Runtime.Versioning;

using Microsoft.Extensions.Logging;

using ZenMonitor.Core.Interfaces;
using ZenMonitor.Core.Models;

namespace ZenMonitor.Core.Services.Windows;

[SupportedOSPlatform("windows")]
public class GpuNull(ILogger<GpuNull> logger) : IGpu
{
    private readonly ILogger<GpuNull> _logger = logger;
    private readonly GpuInfoSnapshot _snapshot = new(
        "", 0, 0, 0.0, 0.0, 0, "", 0.0);

    public void Update() => _logger.LogTrace("Gpu not supported, using GpuNull...");

    public string GetGpuName() => _snapshot.GpuName;
    public int GetUsageGpu() => _snapshot.UsageGpu;
    public int GetUsageMemory() => _snapshot.UsageMemory;
    public double GetMemoryUsed() => _snapshot.MemoryUsed;
    public double GetMemoryTotal() => _snapshot.MemoryTotal;
    public int GetTemperatureGpu() => _snapshot.TemperatureGpu;
    public string GetPowerState() => _snapshot.PowerState;
    public double GetPowerDraw() => _snapshot.PowerDraw;
}
