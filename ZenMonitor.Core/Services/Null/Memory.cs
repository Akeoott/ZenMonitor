// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using Microsoft.Extensions.Logging;

using ZenMonitor.Core.Interfaces;
using ZenMonitor.Core.Models;

namespace ZenMonitor.Core.Services.Null;

public class Memory(ILogger<Memory> logger) : IMemory
{
    private readonly ILogger<Memory> _logger = logger;
    private readonly MemoryInfoSnapshot _snapshot = new(0, 0, 0, 0, 0, 0, 0);

    public void Update() => _logger.LogWarning("Overriding platform specific code. Returning empty snapshot...");

    public double GetMemTotal() => _snapshot.MemTotal;
    public double GetMemFree() => _snapshot.MemFree;
    public double GetMemAvailable() => _snapshot.MemAvailable;
    public double GetMemUsed() => _snapshot.MemUsed;
    public double GetCached() => _snapshot.Cached;
    public double GetSwapTotal() => _snapshot.SwapTotal;
    public double GetSwapFree() => _snapshot.SwapFree;
}
