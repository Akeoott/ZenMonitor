// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using ZenMonitor.Core.Models;

namespace ZenMonitor.Core.Interfaces;

// TODO: add implementation
public interface ISystemService
{
    void Update();

    string GetKernelVersion();
    string GetHostname();
    double GetUptimeSeconds();
    double GetLoadAvg1Min();
    double GetLoadAvg5Min();
    double GetLoadAvg15Min();
    int GetRunningTasks();
    int GetTotalTasks();
    long GetBootTimeUnixSeconds();
}
