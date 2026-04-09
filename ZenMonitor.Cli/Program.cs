// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using Microsoft.Extensions.DependencyInjection;

using ZenMonitor.Core.Interfaces;
using ZenMonitor.Core.Services;

namespace ZenMonitor.Cli;

internal class Program
{
    internal static void Main(string[] args)
    {
        var services = new ServiceCollection();

        services.AddSingleton<IHardwareService, LinuxHardwareService>();
        services.AddTransient<MonitorEngine>();

        var serviceProvider = services.BuildServiceProvider();

        var monitor = serviceProvider.GetRequiredService<IHardwareService>();
        var engine = serviceProvider.GetRequiredService<MonitorEngine>();

        // TODO: Use CancellationToken
        while (true)
        {
            Thread.Sleep(1000);
            engine.UpdateDashboard();
        }
    }
}
