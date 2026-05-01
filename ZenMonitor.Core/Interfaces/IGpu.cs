// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using ZenMonitor.Core.Models;

namespace ZenMonitor.Core.Interfaces;

public interface IGpu
{
    void Update();

    string GetGpuName();
    string GetUsageGpu();
    string GetUsageMemory();
    string GetMemoryUsed();
    string GetMemoryTotal();
    string GetTemperatureGpu();
    string GetPowerState();
    string GetPowerDraw();
}
