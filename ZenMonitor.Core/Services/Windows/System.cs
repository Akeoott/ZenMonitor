// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using System.IO.Abstractions;
using System.Runtime.Versioning;

using Microsoft.Extensions.Logging;

using ZenMonitor.Core.Interfaces;
using ZenMonitor.Core.Models;

namespace ZenMonitor.Core.Services.Windows;

[SupportedOSPlatform("windows")]
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
        return new SystemInfoSnapshot("Error", "Error", 0, 0, 0);
    }
}
