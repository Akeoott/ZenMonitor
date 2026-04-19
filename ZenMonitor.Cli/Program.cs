// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using Microsoft.Extensions.DependencyInjection;

using ZenMonitor.Core.Interfaces;
using ZenMonitor.Core.Services.Linux;

namespace ZenMonitor.Cli;

internal class Program
{
    internal static async Task Main(string[] args)
    {
        var services = new ServiceCollection();

        services.AddSingleton<IHardwareService, Cpu>();
        services.AddTransient<MonitorEngine>();

        var serviceProvider = services.BuildServiceProvider();

        var engine = serviceProvider.GetRequiredService<MonitorEngine>();

        await engine.InitLoop();
    }
}
