// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

using ZenMonitor.Core.Abstractions;
using ZenMonitor.Core.Models.Telemetry;
using ZenMonitor.Models;
using ZenMonitor.UserConfig;

namespace ZenMonitor.Hubs;

public class ApiHub(
    ILogger<ApiHub> logger,
    IConfigService configService,
    ISystemTelemetry telemetry) : Hub
{
    #region Config

    public ValueTask<ConfigModel> GetConfig(CancellationToken ct = default)
    {
        try
        {
            return ValueTask.FromResult(configService.Current);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "GetConfig failed.");
            throw new HubException("Unable to retrieve configuration.");
        }
    }

    public void UpdateConfig(ConfigModel newConfig, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(newConfig);

        try
        {
            configService.UpdateConfig(newConfig);
            logger.LogInformation("Config updated in memory.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "UpdateConfig failed.");
            throw new HubException("Failed to update configuration.");
        }
    }

    public async Task LoadConfig(CancellationToken ct = default)
    {
        try
        {
            await configService.LoadAsync(ct);
            logger.LogInformation("Config loaded from disk.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "LoadConfig failed.");
            throw new HubException("Failed to load configuration from disk.");
        }
    }

    public async Task SaveConfig(CancellationToken ct = default)
    {
        try
        {
            await configService.SaveAsync(ct);
            logger.LogInformation("Config saved to disk.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "SaveConfig failed.");
            throw new HubException("Failed to save configuration to disk.");
        }
    }

    #endregion

    #region Telemetry

    public void UpdateAll()
    {
        try
        {
            telemetry.UpdateAll();
            logger.LogDebug("Telemetry updated.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "UpdateAll failed.");
            throw new HubException("Failed to refresh telemetry.");
        }
    }

    public ValueTask<AllInfoSnapshot> GetAllTelemetry(CancellationToken ct = default)
    {
        try
        {
            return ValueTask.FromResult(new AllInfoSnapshot(
                telemetry.CpuTel.GetSnapshot(),
                telemetry.DriveTel.GetSnapshot(),
                telemetry.GpuTel.GetSnapshot(),
                telemetry.MemoryTel.GetSnapshot(),
                telemetry.NetworkTel.GetSnapshot(),
                telemetry.ProcessTel.GetSnapshot(),
                telemetry.SystemTel.GetSnapshot()
            ));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "GetAllTelemetry failed.");
            throw new HubException("Unable to fetch telemetry snapshot.");
        }
    }

    public ValueTask<CpuInfoSnapshot> GetCpuTelemetry(CancellationToken ct = default)
        => ValueTask.FromResult(telemetry.CpuTel.GetSnapshot());

    public ValueTask<DriveInfoSnapshot> GetDriveTelemetry(CancellationToken ct = default)
        => ValueTask.FromResult(telemetry.DriveTel.GetSnapshot());

    public ValueTask<GpuInfoSnapshot> GetGpuTelemetry(CancellationToken ct = default)
        => ValueTask.FromResult(telemetry.GpuTel.GetSnapshot());

    public ValueTask<MemoryInfoSnapshot> GetMemoryTelemetry(CancellationToken ct = default)
        => ValueTask.FromResult(telemetry.MemoryTel.GetSnapshot());

    public ValueTask<NetworkInfoSnapshot> GetNetworkTelemetry(CancellationToken ct = default)
        => ValueTask.FromResult(telemetry.NetworkTel.GetSnapshot());

    public ValueTask<ProcessInfoSnapshot> GetProcessTelemetry(CancellationToken ct = default)
        => ValueTask.FromResult(telemetry.ProcessTel.GetSnapshot());

    public ValueTask<SystemInfoSnapshot> GetSystemTelemetry(CancellationToken ct = default)
        => ValueTask.FromResult(telemetry.SystemTel.GetSnapshot());

    #endregion
}
