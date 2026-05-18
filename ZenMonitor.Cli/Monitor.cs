// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using Microsoft.Extensions.Logging;

using ZenMonitor.Core.Abstractions;
using ZenMonitor.Core.Models;

namespace ZenMonitor.Cli;

public class Monitor(
    ILogger<Monitor> logger,
    ICpu cpuInfo,
    IDrive driveInfo,
    IGpu gpuInfo,
    IMemory memoryInfo,
    INetwork networkInfo,
    ISystem systemInfo)
{
    private readonly ILogger<Monitor> _logger = logger;
    private readonly ICpu _cpuInfo = cpuInfo;
    private readonly IDrive _driveInfo = driveInfo;
    private readonly IGpu _gpuInfo = gpuInfo;
    private readonly IMemory _memoryInfo = memoryInfo;
    private readonly INetwork _networkInfo = networkInfo;
    private readonly ISystem _systemInfo = systemInfo;

    private readonly SemaphoreSlim _dataReadyEvent = new(0, int.MaxValue);

    public async Task InitMonitor(int loopDelay, CancellationToken cts)
    {
        await Task.WhenAll(RunBackend(loopDelay, cts), RunDashboard(cts));
    }

    private async Task RunDashboard(CancellationToken cts)
    {
        while (true)
        {
            try
            {
                await _dataReadyEvent.WaitAsync(cts);

                Console.Write("\n\n========================DEBUG========================\n\n\n");

                Console.WriteLine("CPU INFORMATION");
                Console.WriteLine($"  Name: {_cpuInfo.GetCpuName()}");
                Console.Write($"  Speed (MHz): C0 {_cpuInfo.GetCpuSpeed()}");
                CpuCoreSpeed[] cpuCoreSpeed = _cpuInfo.GetCoreSpeeds();
                for (int i = 0; i < cpuCoreSpeed.Length; i++)
                {
                    CpuCoreSpeed speed = cpuCoreSpeed[i];
                    Console.Write($", C{speed.Index + 1} {speed.Speed}");
                }
                Console.WriteLine();

                Console.Write($"  Usage (%): C0 {_cpuInfo.GetCpuUsage()}");
                CpuCoreUsage[] cpuCoreUsage = _cpuInfo.GetCoreUsages();
                for (int i = 0; i < cpuCoreUsage.Length; i++)
                {
                    CpuCoreUsage? usage = cpuCoreUsage[i];
                    Console.Write($", C{usage.Index + 1} {usage.Usage}");
                }
                Console.WriteLine();

                Console.Write($"  Temperature (°C): C0 {_cpuInfo.GetCpuTemp()}");
                CpuCoreTemp[] cpuCoreTemp = _cpuInfo.GetCoreTemps();
                for (int i = 0; i < cpuCoreTemp.Length; i++)
                {
                    CpuCoreTemp? temp = cpuCoreTemp[i];
                    Console.Write($", C{temp.Index + 1} {temp.Temp}");
                }
                Console.WriteLine();
                Console.WriteLine($"  Power Draw (W): {_cpuInfo.GetPowerDraw()}\n");

                Console.WriteLine("GPU INFORMATION");
                Console.WriteLine($"  Name: {_gpuInfo.GetGpuName()}");
                Console.WriteLine($"  GPU Usage (%): {_gpuInfo.GetUsageGpu()}");
                Console.WriteLine($"  Memory Usage (%): {_gpuInfo.GetUsageMemory()}");
                Console.WriteLine($"  Memory Used: {_gpuInfo.GetMemoryUsed()}");
                Console.WriteLine($"  Memory Total: {_gpuInfo.GetMemoryTotal()}");
                Console.WriteLine($"  Temperature (°C): {_gpuInfo.GetTemperatureGpu()}");
                Console.WriteLine($"  Power State: {_gpuInfo.GetPowerState()}");
                Console.WriteLine($"  Power Draw (W): {_gpuInfo.GetPowerDraw()}\n");

                Console.WriteLine("MEMORY INFORMATION");
                Console.WriteLine($"  Total: {_memoryInfo.GetMemTotal()}");
                Console.WriteLine($"  Free: {_memoryInfo.GetMemFree()}");
                Console.WriteLine($"  Available: {_memoryInfo.GetMemAvailable()}");
                Console.WriteLine($"  Used: {_memoryInfo.GetMemUsed()}");
                Console.WriteLine($"  Cached: {_memoryInfo.GetCached()}");
                Console.WriteLine($"  Swap Total: {_memoryInfo.GetSwapTotal()}");
                Console.WriteLine($"  Swap Free: {_memoryInfo.GetSwapFree()}\n");

                Console.WriteLine("SYSTEM INFORMATION");
                Console.WriteLine($"  Kernel: {_systemInfo.GetKernelVersion()}");
                Console.WriteLine($"  Hostname: {_systemInfo.GetHostname()}");
                Console.WriteLine($"  Uptime (s): {_systemInfo.GetUptimeSeconds()}");
                Console.WriteLine($"  Running Tasks: {_systemInfo.GetRunningTasks()}");
                Console.WriteLine($"  Total Tasks: {_systemInfo.GetTotalTasks()}");

                Console.WriteLine("DRIVE INFORMATION");
                var mountInfos = _driveInfo.GetMountInfos();
                foreach (var mount in mountInfos)
                {
                    double usagePercent = mount.TotalBytes > 0 ? (double)mount.UsedBytes / mount.TotalBytes * 100 : 0;
                    Console.WriteLine($"  {mount.MountPoint}: {mount.DeviceName} ({mount.FileSystem}) - {usagePercent:F1}% used ({mount.UsedBytes}/{mount.TotalBytes} bytes), IO: {mount.IOUsage:F1}%");
                }
                Console.WriteLine();
            }
            catch (TaskCanceledException)
            {
                break;
            }
        }
    }

    private Task RunBackend(int loopDelay, CancellationToken cts)
    {
        return Task.Run(async () =>
        {
            while (!cts.IsCancellationRequested)
            {
                _cpuInfo.Update();
                _driveInfo.Update();
                _gpuInfo.Update();
                _memoryInfo.Update();
                _systemInfo.Update();
                _logger.LogTrace("Done! Sending event to update interface.");
                _dataReadyEvent.Release();
                await Task.Delay(loopDelay, cts);
            }
        }, cts);
    }
}
