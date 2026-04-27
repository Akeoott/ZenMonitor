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

    public double GetMemTotal() => ParseMemInfoValue("MemTotal");
    public double GetMemFree() => ParseMemInfoValue("MemFree");
    public double GetMemAvailable() => ParseMemInfoValue("MemAvailable");
    public double GetMemUsed() => Math.Round(GetMemTotal() - GetMemAvailable(), 2);

    public double GetCached() => ParseMemInfoValue("Cached");
    public double GetSwapTotal() => ParseMemInfoValue("SwapTotal");
    public double GetSwapFree() => ParseMemInfoValue("SwapFree");

    private double ParseMemInfoValue(string key)
    {
        try
        {
            _logger.LogTrace("({key}) Getting memory value.", key);

            string line = File.ReadLines("/proc/meminfo")
            .FirstOrDefault(l => l.StartsWith(key)) ??
                throw new KeyNotFoundException($"Could not find '{key}' in /proc/meminfo");

            string valueStr = line.Split(':')[1].Trim().Split(' ')[0];

            if (double.TryParse(valueStr, out double value))
            {
                // Convert from KB to GiB
                value = Math.Round(value /= 1_048_576, 2);
                return value;
            }

            throw new FormatException($"Could not parse '{key}' (Parameter '{valueStr}')");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "({key}) Something unexpected happened.", key);
            _logger.LogWarning("({key}) Returning 0 due to error.", key);
            return 0;
        }
    }
}
