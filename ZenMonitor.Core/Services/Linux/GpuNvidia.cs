// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using System.Diagnostics;
using System.Runtime.Versioning;

using Microsoft.Extensions.Logging;

using ZenMonitor.Core.Interfaces;
using ZenMonitor.Core.Models;

namespace ZenMonitor.Core.Services.Linux;

[SupportedOSPlatform("linux")]
public class GpuNvidia(ILogger<GpuNvidia> logger, IHelper helper) : IGpu
{
    private readonly ILogger<GpuNvidia> _logger = logger;
    private readonly IHelper _helper = helper;
    private GpuInfoSnapshot _snapshot = new(
        "", "", "", "", "", "", "", "");

    public void Update() => _snapshot = FetchGpuInfo();

    public string GetGpuName() => _snapshot.GpuName;
    public string GetUsageGpu() => _snapshot.UsageGpu;
    public string GetUsageMemory() => _snapshot.UsageMemory;
    public string GetMemoryUsed() => _snapshot.MemoryUsed;
    public string GetMemoryTotal() => _snapshot.MemoryTotal;
    public string GetTemperatureGpu() => _snapshot.TemperatureGpu;
    public string GetPowerState() => _snapshot.PowerState;
    public string GetPowerDraw() => _snapshot.PowerDraw;

    private GpuInfoSnapshot FetchGpuInfo()
    {
        _logger.LogTrace("Fetching all GpuNvidia info...");

        string csv = RunNvidiaSmi(
            "--query-gpu=name,utilization.gpu,utilization.memory,memory.used,memory.total,temperature.gpu,pstate,power.draw --format=csv,noheader,nounits");

        string[] part = [.. csv.Split(',').Select(p => p.Trim())];

        return new GpuInfoSnapshot(
            part[0], part[1], part[2], part[3],
            part[4], part[5], part[6], part[7]);
    }

    private string RunNvidiaSmi(string arguments)
    {
        ProcessResult result = _helper.RunProcess("nvidia-smi", arguments);

        if (result.ExitCode != 0)
        {
            _logger.LogError("Running nvidia-smi failed with exit code {ExitCode}: {Error}", result.ExitCode, result.StandardError);
            throw new InvalidOperationException($"nvidia-smi error: {result.StandardError}");
        }

        return result.StandardOutput.Trim();
    }
}
