// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

namespace ZenMonitor.Core.Interfaces;

public interface IGpu
{
    void Update();

    string GetGpuName();
    int GetUsageGpu();
    int GetUsageMemory();
    double GetMemoryUsed();
    double GetMemoryTotal();
    int GetTemperatureGpu();
    string GetPowerState();
    double GetPowerDraw();
}
