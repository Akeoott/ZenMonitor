// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using Microsoft.Extensions.Logging;

using Spectre.Console;

using ZenMonitor.Core.Interfaces;
using ZenMonitor.Core.Models;

namespace ZenMonitor.Debug;

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

                Console.Clear();
                Console.WriteLine(_cpuInfo.GetCpuName());
                foreach (var speeds in _cpuInfo.GetCoreSpeeds())
                {
                    Console.Write($"{speeds}Mhz, ");
                }
                Console.WriteLine("");
                foreach (var speeds in _cpuInfo.GetCoreUsages())
                {
                    Console.Write($"C{speeds.Index} {speeds.Usage}%, ");
                }

                Console.WriteLine($"\n\n{_gpuInfo.GetGpuName()}\n");

                Console.WriteLine(
                    $"{_memoryInfo.GetMemTotal()}, {_memoryInfo.GetMemFree()}, " +
                    $"{_memoryInfo.GetMemAvailable()}, {_memoryInfo.GetMemUsed()}, " +
                    $"{_memoryInfo.GetCached()}, {_memoryInfo.GetSwapTotal()}, " +
                    $"{_memoryInfo.GetSwapFree()}"
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
