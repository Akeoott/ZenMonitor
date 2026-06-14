// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using System.Threading;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Serilog;

using Spectre.Console.Cli;

// ReSharper disable ClassNeverInstantiated.Global

namespace ZenMonitor;

internal class Initialize : AsyncCommand<Config>
{
    protected override async Task<int> ExecuteAsync(
        CommandContext context,
        Config settings,
        CancellationToken cancellationToken)
    {
        var logLevel = Config.ParseSerilogLevel(settings.Verbosity);
        const string logFilePath = "logs/ZenMonitor.log";

        Config.ConfigureLogging(settings.Quiet, logLevel, logFilePath);

        await using var serviceProvider = DependencyInjection.BuildServiceProvider();
        var logger = serviceProvider.GetRequiredService<ILogger<Initialize>>();
        AppBootstrap.ServiceProvider = serviceProvider;
        logger.LogInformation("Injected dependencies and configured logging.");
        logger.LogInformation(
            "Running on {OSDescription} (OSVersion: {Version})",
            RuntimeInformation.OSDescription,
            Environment.OSVersion.Version);

        if (settings.Force)
            logger.LogWarning("Bypassing sudo/admin requirements!");

        try
        {
            logger.LogInformation("ZenMonitor initialized. Invoking platform startup.");
            AppBootstrap.PlatformStartup?.Invoke();
            logger.LogInformation("Application finished, bye bye!");
            return 0;
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Something unexpected happened ;-;");
            return 1;
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
    }
}
