// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using Spectre.Console;

using ZenMonitor.Core.Interfaces;
using ZenMonitor.Core.Models;

namespace ZenMonitor.Cli;

internal class MonitorEngine(IHardwareService hardware)
{
    private readonly IHardwareService _hardware = hardware;

    public async Task InitLoop()
    {
        await RunLiveDashboard(1000);
    }

    //! NOTE: This is just a debug thingy for now.
    //        I intend to completely overhaul all of this later.
    private async Task RunLiveDashboard(int delay)
    {
        var table = new Table().Border(TableBorder.Rounded).Expand();
        table.AddColumn("Core");
        table.AddColumn("Clock Speed");
        table.AddColumn("Usage Graph");
        table.AddColumn("Percentage");

        var initialUsages = _hardware.GetCoreUsages();
        for (int i = 0; i < initialUsages.Length; i++)
        {
            table.AddRow("", "", "", "");
        }

        await AnsiConsole.Live(table)
            .StartAsync(async ctx =>
            {
                while (true)
                {
                    var usages = _hardware.GetCoreUsages();

                    for (int i = 0; i < usages.Length; i++)
                    {
                        var u = usages[i].Usage;
                        string color = u > 80 ? "red" : u > 40 ? "yellow" : "green";

                        string bar = new('█', (int)(u / 5));

                        table.UpdateCell(i, 0, i == 0 ? "[bold]Total[/]" : $"Core {i - 1:D2}");
                        table.UpdateCell(i, 1, $"[{color}]{bar}[/]");
                        table.UpdateCell(i, 2, $"[{color}]{u:F1}%[/]");
                    }

                    ctx.Refresh();
                    await Task.Delay(delay);
                }
            });
    }
}
