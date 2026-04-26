// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using System.ComponentModel;
using System.Runtime.InteropServices;

using Spectre.Console.Cli;

using ZenMonitor.Core.Interfaces;

namespace ZenMonitor.Cli;

internal class Program
{
    internal static async Task<int> Main(string[] args)
    {
        var app = new CommandApp<MonitorCommand>();
        return await app.RunAsync(args);
    }
}

public class MonitorSettings : CommandSettings
{
    [CommandOption("-l|--log <level>")]
    [Description("Set minimum log level (critical/error/warning/info/debug/trace)")]
    [DefaultValue("warning")]
    public required string LogLevel { get; set; }
}

public class MonitorCommand : AsyncCommand<MonitorSettings>
{
    protected override async Task<int> ExecuteAsync(
        CommandContext context,
        MonitorSettings settings,
        CancellationToken cancellationToken)
    {
        var logLevel = ParseLogLevel(settings.LogLevel);

        var services = new ServiceCollection();

        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.AddDebug();
            builder.SetMinimumLevel(logLevel);
        });

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            services.AddSingleton<ICpuService, Core.Services.Linux.Cpu>();
            services.AddSingleton<IGeneralService, Core.Services.Linux.General>();
            services.AddSingleton<IGpuService, Core.Services.Linux.Gpu>();
            services.AddSingleton<INetworkService, Core.Services.Linux.Network>();
            services.AddSingleton<IRamService, Core.Services.Linux.Ram>();
        }
        // TODO: Uncomment and adjust once the Linux implementation is finished.
        // else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        // {
        //     services.AddSingleton<ICpuService, Core.Services.Windows.Cpu>();
        //     services.AddSingleton<IGeneralService, Core.Services.Windows.General>();
        //     services.AddSingleton<IGpuService, Core.Services.Windows.Gpu>();
        //     services.AddSingleton<INetworkService, Core.Services.Windows.Network>();
        //     services.AddSingleton<IRamService, Core.Services.Windows.Ram>();
        // }
        else
        {
            throw new PlatformNotSupportedException("ZenMonitor only supports Linux at the moment. Windows support will come in the future.");
        }

        services.AddTransient<MonitorEngine>();

        var serviceProvider = services.BuildServiceProvider();

        var engine = serviceProvider.GetRequiredService<MonitorEngine>();

        await engine.Run();

        return 0;
    }

    private static LogLevel ParseLogLevel(string level)
    {
        return level?.ToLowerInvariant() switch
        {
            "trace" => LogLevel.Trace,
            "debug" => LogLevel.Debug,
            "info" => LogLevel.Information,
            "warning" => LogLevel.Warning,
            "error" => LogLevel.Error,
            "critical" => LogLevel.Critical,
            _ => LogLevel.Warning
        };
    }
}
