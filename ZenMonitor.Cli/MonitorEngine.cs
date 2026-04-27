// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using Microsoft.Extensions.Logging;

using Spectre.Console;
using Spectre.Console.Cli;

using ZenMonitor.Core.Interfaces;
using ZenMonitor.Core.Models;

namespace ZenMonitor.Cli;

internal class MonitorEngine(
    ILogger<MonitorEngine> logger,
    ICpuService cpuInfo,
    IGpuService gpuInfo,
    IMemoryService memoryInfo,
    INetworkService networkInfo,
    ISystemService systemInfo)
{
    private readonly ILogger<MonitorEngine> _logger = logger;
    private readonly ICpuService _cpuInfo = cpuInfo;
    private readonly IGpuService _gpuInfo = gpuInfo;
    private readonly IMemoryService _memoryInfo = memoryInfo;
    private readonly INetworkService _networkInfo = networkInfo;
    private readonly ISystemService _systemInfo = systemInfo;

    public async Task Run()
    {
        await RunLiveDashboard();
    }

    //! Debug stuff right now...
    private async Task RunLiveDashboard()
    {
        while (true)
        {
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

            Console.WriteLine("");

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
                $"{_systemInfo.GetBootTimeUnixSeconds()}"
            );
            Console.WriteLine("");

            _cpuInfo.Update();
            _memoryInfo.Update();
            _systemInfo.Update();
            await Task.Delay(1000);
        }
        // var coreUsages = _cpuInfo.GetCoreUsages();
        // AnsiConsole.Progress()
        //     .Columns(
        //         new TaskDescriptionColumn(),
        //         new ProgressBarColumn()
        //         {
        //             CompletedStyle = new Style(Color.Green),
        //             FinishedStyle = new Style(Color.Green),
        //             RemainingStyle = new Style(Color.Grey)
        //         },
        //         new PercentageColumn())
        //     .Start(ctx =>
        //     {
        //         var coreUsages = _cpuInfo.GetCoreUsages();
        //         var tasks = new Dictionary<int, ProgressTask>();

        //         foreach (var core in coreUsages)
        //         {
        //             tasks[core.Index] = ctx.AddTask($"C{core.Index}");
        //         }

        //         while (true)
        //         {
        //             coreUsages = _cpuInfo.GetCoreUsages();
        //             foreach (var core in coreUsages)
        //             {
        //                 if (tasks.TryGetValue(core.Index, out var task))
        //                 {
        //                     task.Value = core.Usage;
        //                 }
        //             }
        //             Thread.Sleep(1000);
        //         }
        //     });
    }
}
