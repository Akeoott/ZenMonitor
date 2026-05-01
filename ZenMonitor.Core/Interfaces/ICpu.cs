// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using ZenMonitor.Core.Models;

namespace ZenMonitor.Core.Interfaces;

public interface ICpu
{
    void Update();

    string GetCpuName();
    double GetCpuSpeed();
    int GetCpuUsage();
    int GetCpuTemp();
    double GetPowerDraw();
    CpuCoreSpeed[] GetCoreSpeeds();
    CpuCoreUsage[] GetCoreUsages();
    CpuCoreTemp[] GetCoreTemps();
}
