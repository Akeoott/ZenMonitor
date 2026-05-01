// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using Microsoft.Extensions.Logging;

using ZenMonitor.Core.Interfaces;
using ZenMonitor.Core.Models;

namespace ZenMonitor.Cli;

public class Monitor(
    ILogger<Monitor> logger,
    ICpuService cpuInfo,
    IGpuService gpuInfo,
    IMemoryService memoryInfo,
    INetworkService networkInfo,
    ISystemService systemInfo)
{
    private readonly ILogger<Monitor> _logger = logger;
    private readonly ICpuService _cpuInfo = cpuInfo;
    private readonly IGpuService _gpuInfo = gpuInfo;
    private readonly IMemoryService _memoryInfo = memoryInfo;
    private readonly INetworkService _networkInfo = networkInfo;
    private readonly ISystemService _systemInfo = systemInfo;

    private readonly SemaphoreSlim _dataReadyEvent = new(0, int.MaxValue);

    public async Task InitMonitor(int loopDelay, CancellationToken cts)
    {
        await RunBackend(loopDelay, cts);
        await RunDashboard(cts);
    }

    private async Task RunDashboard(CancellationToken cts)
    {
        while (true)
        {
            try
            {
                await _dataReadyEvent.WaitAsync(cts);

                Console.WriteLine("\n--------------------------------------\n");

                Console.WriteLine(_cpuInfo.GetCpuName());
                Console.Write($"C0 {_cpuInfo.GetCpuSpeed()}GHz, ");
                CpuCoreSpeed[] cpuCoreSpeed = _cpuInfo.GetCoreSpeeds();
                for (int i = 0; i < cpuCoreSpeed.Length; i++)
                {
                    CpuCoreSpeed speed = cpuCoreSpeed[i];
                    Console.Write($"C{speed.Index + 1} {speed.Speed}Mhz, ");
                }
                Console.WriteLine("");

                Console.Write($"C0 {_cpuInfo.GetCpuUsage()}%, ");
                CpuCoreUsage[] cpuCoreUsage = _cpuInfo.GetCoreUsages();
                for (int i = 0; i < cpuCoreUsage.Length; i++)
                {
                    CpuCoreUsage? usage = cpuCoreUsage[i];
                    Console.Write($"C{usage.Index + 1} {usage.Usage}%, ");
                }
                Console.WriteLine("");

                Console.Write($"C0 {_cpuInfo.GetCpuTemp()}°C, ");
                CpuCoreTemp[] cpuCoreTemp = _cpuInfo.GetCoreTemps();
                for (int i = 0; i < cpuCoreTemp.Length; i++)
                {
                    CpuCoreTemp? temp = cpuCoreTemp[i];
                    Console.Write($"C{temp.Index + 1} {temp.Temp}°C, ");
                }
                Console.Write($"{_cpuInfo.GetPowerDraw()} Watts, ");

                Console.WriteLine(
                    $"\n\n{_gpuInfo.GetGpuName()},\n{_gpuInfo.GetUsageGpu()}, " +
                    $"{_gpuInfo.GetUsageMemory()}, {_gpuInfo.GetMemoryUsed()}, " +
                    $"{_gpuInfo.GetMemoryTotal()}, {_gpuInfo.GetTemperatureGpu()}, " +
                    $"{_gpuInfo.GetPowerState()}, {_gpuInfo.GetPowerDraw()}\n"
                );

                Console.WriteLine(
                    $"{_memoryInfo.GetMemTotal()}, {_memoryInfo.GetMemFree()}, " +
                    $"{_memoryInfo.GetMemAvailable()}, {_memoryInfo.GetMemUsed()}, " +
                    $"{_memoryInfo.GetCached()}, {_memoryInfo.GetSwapTotal()}, " +
                    $"{_memoryInfo.GetSwapFree()}\n"
                );
                Console.WriteLine(
                    $"{_systemInfo.GetKernelVersion()}, {_systemInfo.GetHostname()}, " +
                    $"{_systemInfo.GetUptimeSeconds()}, {_systemInfo.GetLoadAvg1Min()}, " +
                    $"{_systemInfo.GetLoadAvg5Min()}, {_systemInfo.GetLoadAvg15Min()}, " +
                    $"{_systemInfo.GetRunningTasks()}, {_systemInfo.GetTotalTasks()}, " +
                    $"{_systemInfo.GetBootTimeUnixSeconds()}\n"
                );
            }
            catch (TaskCanceledException)
            {
                break;
            }
        }
    }

    private Task RunBackend(int loopDelay, CancellationToken cts)
    {

        var backendThread = new Thread(() =>
        {
            while (!cts.IsCancellationRequested)
            {
                _cpuInfo.Update();
                _gpuInfo.Update();
                _memoryInfo.Update();
                _systemInfo.Update();
                _dataReadyEvent.Release();
                Thread.Sleep(loopDelay);
            }
        })
        { IsBackground = true };
        backendThread.Start();

        return Task.CompletedTask;
    }
}
