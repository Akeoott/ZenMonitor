// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using ZenMonitor.Core.Models.Telemetry;

namespace ZenMonitor.Hubs;

public record AllInfoSnapshot(
    CpuInfoSnapshot CpuInfo,
    DriveInfoSnapshot DriveInfo,
    GpuInfoSnapshot GpuInfo,
    MemoryInfoSnapshot MemoryInfo,
    NetworkInfoSnapshot NetworkInfo,
    ProcessInfoSnapshot ProcessInfo,
    SystemInfoSnapshot SystemInfo
);
