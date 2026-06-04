// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using Serilog;

using ZenMonitor.Core.Hosting;

namespace ZenMonitor.Setup;

internal class DependencyInjection
{
    internal static ServiceProvider BuildServiceProvider(out bool gpuNotSupported)
    {
        var services = new ServiceCollection();

        services.AddLogging(builder =>
        {
            builder.ClearProviders();
            builder.AddSerilog(dispose: true);
        });

        // All ZenMonitor platform services (Linux/Win detection, GPU auto-detect)
        services.AddZenMonitor(out gpuNotSupported);
        services.AddTransient<Monitor>();
        return services.BuildServiceProvider();
    }
}
