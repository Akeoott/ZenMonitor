// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using System.Runtime.Versioning;

using Microsoft.Extensions.Logging;

using ZenMonitor.Core.Interfaces;
using ZenMonitor.Core.Models;

namespace ZenMonitor.Core.Services.Linux;

[SupportedOSPlatform("linux")]
public class System(ILogger<System> logger) : ISystemService
{
    private readonly ILogger<System> _logger = logger;
    private SystemInfoSnapshot _snapshot = new(
        "Unknown", "Unknown", 0, 0, 0, 0, 0, 0, 0);

    public void Update()
    {
        _snapshot = FetchSystemInfo();
    }

    public string GetKernelVersion() => _snapshot.KernelVersion;
    public string GetHostname() => _snapshot.Hostname;
    public double GetUptimeSeconds() => _snapshot.UptimeSeconds;
    public double GetLoadAvg1Min() => _snapshot.LoadAvg1Min;
    public double GetLoadAvg5Min() => _snapshot.LoadAvg5Min;
    public double GetLoadAvg15Min() => _snapshot.LoadAvg15Min;
    public int GetRunningTasks() => _snapshot.RunningTasks;
    public int GetTotalTasks() => _snapshot.TotalTasks;
    public long GetBootTimeUnixSeconds() => _snapshot.BootTimeUnixSeconds;

    private SystemInfoSnapshot FetchSystemInfo()
    {
        try
        {
            _logger.LogTrace("Fetching all System info...");
            // Kernel version from /proc/sys/kernel/osrelease
            string kernel = File.ReadAllText("/proc/sys/kernel/osrelease").Trim();

            // Hostname from /proc/sys/kernel/hostname
            string hostname = File.ReadAllText("/proc/sys/kernel/hostname").Trim();

            // Uptime (total uptime, idle time)
            var uptimeParts = File.ReadAllText("/proc/uptime").Trim().Split(' ');
            double uptime = double.Parse(uptimeParts[0]);

            // Load average
            var loadParts = File.ReadAllText("/proc/loadavg").Trim().Split(' ');
            double load1 = double.Parse(loadParts[0]);
            double load5 = double.Parse(loadParts[1]);
            double load15 = double.Parse(loadParts[2]);

            var tasks = loadParts[3].Split('/');
            int running = int.Parse(tasks[0]);
            int total = int.Parse(tasks[1]);

            // Boot time from /proc/stat (btime)
            long bootTime = 0;
            foreach (var line in File.ReadLines("/proc/stat"))
            {
                if (line.StartsWith("btime "))
                {
                    bootTime = long.Parse(line.AsSpan(6).Trim());
                    break;
                }
            }

            return new SystemInfoSnapshot(
                kernel, hostname, uptime, load1, load5, load15,
                running, total, bootTime);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch system info");
            return new SystemInfoSnapshot("Error", "Error", 0, 0, 0, 0, 0, 0, 0);
        }
    }
}
