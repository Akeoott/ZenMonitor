// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using System.Runtime.Versioning;

using Microsoft.Extensions.Logging;

using ZenMonitor.Core.Interfaces;
using ZenMonitor.Core.Models;

namespace ZenMonitor.Core.Services.Linux;

[SupportedOSPlatform("linux")]
public class Memory(ILogger<Memory> logger) : IMemoryService
{
    private readonly ILogger<Memory> _logger = logger;
    private MemoryInfoSnapshot _snapshot = new(0, 0, 0, 0, 0, 0, 0);

    public void Update()
    {
        _snapshot = FetchMemoryInfo();
    }

    public double GetMemTotal() => _snapshot.MemTotal;
    public double GetMemFree() => _snapshot.MemFree;
    public double GetMemAvailable() => _snapshot.MemAvailable;
    public double GetMemUsed() => _snapshot.MemUsed;
    public double GetCached() => _snapshot.Cached;
    public double GetSwapTotal() => _snapshot.SwapTotal;
    public double GetSwapFree() => _snapshot.SwapFree;

    private MemoryInfoSnapshot FetchMemoryInfo()
    {
        try
        {
            _logger.LogTrace("Fetching all Memory info...");

            // Read all lines and collect only the keys we need
            var values = new Dictionary<string, double>(StringComparer.Ordinal);
            const double KB_TO_GIB = 1.0 / 1_048_576;

            foreach (var line in File.ReadLines("/proc/meminfo"))
            {
                int colon = line.IndexOf(':');
                if (colon < 0) continue;

                string key = line[..colon].Trim();
                if (key != "MemTotal" && key != "MemFree" && key != "MemAvailable" &&
                    key != "Cached" && key != "SwapTotal" && key != "SwapFree")
                {
                    continue;
                }

                string valuePart = line[(colon + 1)..].Trim();
                int space = valuePart.IndexOf(' ');
                string numberStr = space >= 0 ? valuePart[..space] : valuePart;

                if (double.TryParse(numberStr, out double kb))
                {
                    values[key] = Math.Round(kb * KB_TO_GIB, 2);
                }
                else
                {
                    throw new FormatException($"Could not parse '{key}' value '{numberStr}'");
                }
            }

            // Ensure we got all keys
            string[] required = ["MemTotal", "MemFree", "MemAvailable", "Cached", "SwapTotal", "SwapFree"];

            foreach (var key in required)
            {
                if (!values.ContainsKey(key))
                    throw new KeyNotFoundException($"Could not find '{key}' in /proc/meminfo");
            }

            return new MemoryInfoSnapshot(
                values["MemTotal"], values["MemFree"], values["MemAvailable"],
                Math.Round(values["MemTotal"] - values["MemAvailable"], 2),
                values["Cached"], values["SwapTotal"], values["SwapFree"]);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch memory info");
            return new MemoryInfoSnapshot(0, 0, 0, 0, 0, 0, 0);
        }
    }
}
