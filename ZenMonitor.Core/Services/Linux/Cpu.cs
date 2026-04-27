// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using System.Runtime.Versioning;

using Microsoft.Extensions.Logging;

using ZenMonitor.Core.Interfaces;
using ZenMonitor.Core.Models;

namespace ZenMonitor.Core.Services.Linux;

[SupportedOSPlatform("linux")]
public class Cpu(ILogger<Cpu> logger) : ICpuService
{
    private readonly ILogger<Cpu> _logger = logger;
    private CpuInfoSnapshot _snapshot = new("Unknown CPU", [], []);

    // Tick buffers for /proc/stat diffs
    private long[][] _currentTicks = [];
    private long[][] _previousTicks = [];

    public void Update()
    {
        _snapshot = FetchCpuInfo();
    }

    public string GetCpuName() => _snapshot.CpuName;
    public double[] GetCoreSpeeds() => _snapshot.CoreSpeedsMHz;
    public CpuUsage[] GetCoreUsages() => _snapshot.CoreUsages;

    private CpuInfoSnapshot FetchCpuInfo()
    {
        try
        {
            _logger.LogTrace("Fetching all CPU info...");

            // Read /proc/cpuinfo once for name and speeds
            string cpuName = "Unknown CPU";
            var speeds = new List<double>();
            foreach (var line in File.ReadLines("/proc/cpuinfo"))
            {
                if (line.StartsWith("model name") && cpuName == "Unknown CPU")
                {
                    var parts = line.Split(':', 2);
                    if (parts.Length == 2)
                        cpuName = parts[1].Trim();
                }
                else if (line.StartsWith("cpu MHz"))
                {
                    var parts = line.Split(':', 2);
                    if (parts.Length == 2 && double.TryParse(parts[1].Trim(), out double mhz))
                        speeds.Add(mhz);
                }
            }

            // Read /proc/stat and compute core usages
            ReadCurrentTicks(); // fills _currentTicks

            CpuUsage[] usages;
            if (_previousTicks.Length == 0)
            {
                // First call baseline only, usage is 0%
                usages = new CpuUsage[_currentTicks.Length];
                for (int i = 0; i < _currentTicks.Length; i++)
                    usages[i] = new CpuUsage(i, 0);

                // Make previous equal to current
                _previousTicks = new long[_currentTicks.Length][];
                for (int i = 0; i < _currentTicks.Length; i++)
                {
                    _previousTicks[i] = new long[_currentTicks[i].Length];
                    Array.Copy(_currentTicks[i], _previousTicks[i], _currentTicks[i].Length);
                }
            }
            else
            {
                usages = ComputeUsages();
            }

            return new CpuInfoSnapshot(cpuName, speeds.ToArray(), usages);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch CPU info");
            return new CpuInfoSnapshot("Error", [], []);
        }
    }

    private void ReadCurrentTicks()
    {
        var lines = File.ReadLines("/proc/stat").Where(l => l.StartsWith("cpu"));
        var tickLists = new List<long[]>();

        foreach (var line in lines)
        {
            var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            int fieldCount = parts.Length - 1;
            var ticks = new long[fieldCount];
            for (int j = 0; j < fieldCount; j++)
                ticks[j] = long.Parse(parts[j + 1]);
            tickLists.Add(ticks);
        }
        _currentTicks = [.. tickLists];
    }

    private CpuUsage[] ComputeUsages()
    {
        int len = Math.Min(_currentTicks.Length, _previousTicks.Length);
        var usages = new CpuUsage[len];

        for (int i = 0; i < len; i++)
        {
            var curr = _currentTicks[i];
            var prev = _previousTicks[i];
            int fields = Math.Min(curr.Length, prev.Length);

            long totalDiff = 0, idleDiff = 0;
            long totalCurr = 0, totalPrev = 0;

            for (int j = 0; j < fields; j++)
            {
                totalCurr += curr[j];
                totalPrev += prev[j];
            }
            totalDiff = totalCurr - totalPrev;

            // Idle fields: index 3 (idle) + index 4 (iowait)
            long idleCurr = curr.Length > 4 ? curr[3] + curr[4] : curr.Length > 3 ? curr[3] : 0;
            long idlePrev = prev.Length > 4 ? prev[3] + prev[4] : prev.Length > 3 ? prev[3] : 0;
            idleDiff = idleCurr - idlePrev;

            double usage = 0;
            if (totalDiff > 0)
                usage = (double)(totalDiff - idleDiff) / totalDiff * 100.0;

            usages[i] = new CpuUsage(i, Math.Round(usage));
        }

        _previousTicks = _currentTicks;
        _currentTicks = [];

        return usages;
    }
}
