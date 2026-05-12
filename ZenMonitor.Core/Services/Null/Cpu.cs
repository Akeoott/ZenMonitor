// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using Microsoft.Extensions.Logging;

using ZenMonitor.Core.Interfaces;
using ZenMonitor.Core.Models;

namespace ZenMonitor.Core.Services.Null;

public class Cpu(ILogger<Cpu> logger) : ICpu
{
    private readonly ILogger<Cpu> _logger = logger;
    private readonly CpuInfoSnapshot _snapshot = new("", 0, 0, 0, 0, [], [], []);

    public void Update() => _logger.LogWarning("Overriding platform specific code. Returning empty snapshot...");

    public string GetCpuName() => _snapshot.CpuName;
    public double GetCpuSpeed() => _snapshot.CpuSpeed;
    public int GetCpuUsage() => _snapshot.CpuUsage;
    public int GetCpuTemp() => _snapshot.CpuTemp;
    public double GetPowerDraw() => _snapshot.PowerDraw;
    public CpuCoreSpeed[] GetCoreSpeeds() => _snapshot.CoreSpeeds;
    public CpuCoreUsage[] GetCoreUsages() => _snapshot.CoreUsages;
    public CpuCoreTemp[] GetCoreTemps() => _snapshot.CoreTemps;
}
