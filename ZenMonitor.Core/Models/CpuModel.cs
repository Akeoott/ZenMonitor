// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

namespace ZenMonitor.Core.Models;

public record CpuCoreSpeed(int Index, double Speed);
public record CpuCoreUsage(int Index, double Usage);
public record CpuCoreTemp(int Index, int Temp);

public record CpuInfoSnapshot(
    string CpuName,
    double CpuSpeed,
    int CpuUsage,
    int CpuTemp,
    double PowerDraw,
    CpuCoreSpeed[] CoreSpeeds,
    CpuCoreUsage[] CoreUsages,
    CpuCoreTemp[] CoreTemps
);
