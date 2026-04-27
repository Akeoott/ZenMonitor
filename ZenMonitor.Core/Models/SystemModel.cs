// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

namespace ZenMonitor.Core.Models;

public record SystemInfoSnapshot(
    string KernelVersion,
    string Hostname,
    double UptimeSeconds,
    double LoadAvg1Min,
    double LoadAvg5Min,
    double LoadAvg15Min,
    int RunningTasks,
    int TotalTasks,
    long BootTimeUnixSeconds
);
