// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

namespace ZenMonitor.Core.Models;

public record MemoryInfoSnapshot(
    double MemTotal,
    double MemFree,
    double MemAvailable,
    double MemUsed,
    double Cached,
    double SwapTotal,
    double SwapFree
);
