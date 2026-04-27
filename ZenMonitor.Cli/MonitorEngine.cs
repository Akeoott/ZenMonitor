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
    IGeneralService generalInfo,
    IGpuService gpuInfo,
    IMemoryService memoryInfo,
    INetworkService networkInfo)
{
    private readonly ILogger<MonitorEngine> _logger = logger;
    private readonly ICpuService _cpuInfo = cpuInfo;
    private readonly IGeneralService _generalInfo = generalInfo;
    private readonly IGpuService _gpuInfo = gpuInfo;
    private readonly IMemoryService _memoryInfo = memoryInfo;
    private readonly INetworkService _networkInfo = networkInfo;

    public async Task Run()
    {
        await RunLiveDashboard();
    }

    //! Debug stuff right now...
    private async Task RunLiveDashboard()
    {
        while (true)
        {
            Console.WriteLine(_memoryInfo.GetMemTotal());
            Console.WriteLine(_memoryInfo.GetMemFree());
            Console.WriteLine(_memoryInfo.GetMemAvailable());
            Console.WriteLine(_memoryInfo.GetMemUsed());
            Console.WriteLine(_memoryInfo.GetCached());
            Console.WriteLine(_memoryInfo.GetSwapTotal());
            Console.WriteLine(_memoryInfo.GetSwapFree());
            System.Console.WriteLine(

            );

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
