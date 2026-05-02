// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

namespace ZenMonitor.Core.Models;

public record GpuInfoSnapshot(
    string GpuName,
    string UsageGpu,
    string UsageMemory,
    string MemoryUsed,
    string MemoryTotal,
    string TemperatureGpu,
    string PowerState,
    string PowerDraw
);

