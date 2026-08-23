// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using Microsoft.AspNetCore.SignalR;
using ZenMonitor.Core.Abstractions;
using ZenMonitor.Core.Models.Telemetry;

namespace ZenMonitor.Hubs;

public class DataHub(ISystemTelemetry monitor) : Hub
{
    public void UpdateAll() => monitor.UpdateAll();

    public AllInfoSnapshot GetAllTelemetry() => new(
        monitor.CpuTel.GetSnapshot(),
        monitor.DriveTel.GetSnapshot(),
        monitor.GpuTel.GetSnapshot(),
        monitor.MemoryTel.GetSnapshot(),
        monitor.NetworkTel.GetSnapshot(),
        monitor.ProcessTel.GetSnapshot(),
        monitor.SystemTel.GetSnapshot()
    );

    public CpuInfoSnapshot GetCpuTelemetry() => monitor.CpuTel.GetSnapshot();
    public DriveInfoSnapshot GetDriveTelemetry() => monitor.DriveTel.GetSnapshot();
    public GpuInfoSnapshot GetGpuTelemetry() => monitor.GpuTel.GetSnapshot();
    public MemoryInfoSnapshot GetMemoryTelemetry() => monitor.MemoryTel.GetSnapshot();
    public NetworkInfoSnapshot GetNetworkTelemetry() => monitor.NetworkTel.GetSnapshot();
    public ProcessInfoSnapshot GetProcessTelemetry() => monitor.ProcessTel.GetSnapshot();
    public SystemInfoSnapshot GetSystemTelemetry() => monitor.SystemTel.GetSnapshot();
}
