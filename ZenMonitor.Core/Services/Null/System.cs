// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using Microsoft.Extensions.Logging;

using ZenMonitor.Core.Interfaces;
using ZenMonitor.Core.Models;

namespace ZenMonitor.Core.Services.Null;

public class System(ILogger<System> logger) : ISystem
{
    private readonly ILogger<System> _logger = logger;
    private readonly SystemInfoSnapshot _snapshot = new("", "", 0, 0, 0);

    public void Update() => _logger.LogWarning("Overriding platform specific code. Returning empty snapshot...");

    public string GetKernelVersion() => _snapshot.KernelVersion;
    public string GetHostname() => _snapshot.Hostname;
    public double GetUptimeSeconds() => _snapshot.UptimeSeconds;
    public int GetRunningTasks() => _snapshot.RunningTasks;
    public int GetTotalTasks() => _snapshot.TotalTasks;
}
