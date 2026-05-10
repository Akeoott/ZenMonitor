// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using System.IO.Abstractions;
using System.Runtime.Versioning;

using Microsoft.Extensions.Logging;

using ZenMonitor.Core.Interfaces;
using ZenMonitor.Core.Models;

namespace ZenMonitor.Core.Services.Linux;

[SupportedOSPlatform("linux")]
public class System(ILogger<System> logger, IFileSystem fileSystem) : ISystem
{
    private readonly ILogger<System> _logger = logger;
    private readonly IFileSystem _fileSystem = fileSystem;
    private SystemInfoSnapshot _snapshot = new(
        "Unknown", "Unknown", 0, 0, 0);

    public void Update() => _snapshot = FetchSystemInfo();

    public string GetKernelVersion() => _snapshot.KernelVersion;
    public string GetHostname() => _snapshot.Hostname;
    public double GetUptimeSeconds() => _snapshot.UptimeSeconds;
    public int GetRunningTasks() => _snapshot.RunningTasks;
    public int GetTotalTasks() => _snapshot.TotalTasks;

    private SystemInfoSnapshot FetchSystemInfo()
    {
        try
        {
            _logger.LogTrace("Fetching all System info...");
            // Kernel version from /proc/sys/kernel/osrelease
            string kernel = _fileSystem.File.ReadAllText("/proc/sys/kernel/osrelease").Trim();

            // Hostname from /proc/sys/kernel/hostname
            string hostname = _fileSystem.File.ReadAllText("/proc/sys/kernel/hostname").Trim();

            // Uptime (total uptime, idle time)
            var uptimeParts = _fileSystem.File.ReadAllText("/proc/uptime").Trim().Split(' ');
            double uptime = double.Parse(uptimeParts[0]);

            var loadParts = _fileSystem.File.ReadAllText("/proc/loadavg").Trim().Split(' ');

            var tasks = loadParts[3].Split('/');
            int running = int.Parse(tasks[0]);
            int total = int.Parse(tasks[1]);

            return new SystemInfoSnapshot(
                kernel, hostname, uptime,
                running, total);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch system info");
            return new SystemInfoSnapshot("Error", "Error", 0, 0, 0);
        }
    }
}
