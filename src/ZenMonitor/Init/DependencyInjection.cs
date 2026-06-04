// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Serilog;

using ZenMonitor.Core.Hosting;

namespace ZenMonitor.Init;

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

        services.AddZenMonitor(out gpuNotSupported);
        return services.BuildServiceProvider();
    }
}
