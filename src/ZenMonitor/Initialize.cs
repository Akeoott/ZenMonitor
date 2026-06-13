// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

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
        try
        {
            var logLevel = Config.ParseSerilogLevel(settings.Verbosity);
            const string logFilePath = "logs/ZenMonitor.log";

            Config.ConfigureLogging(settings.Quiet, logLevel, logFilePath);

            await using var serviceProvider = DependencyInjection.BuildServiceProvider();
            AppBootstrap.ServiceProvider = serviceProvider;
            var logger = serviceProvider.GetRequiredService<ILogger<Initialize>>();

            // Debug messages for us, dev's.
            logger.LogWarning("ZenMonitor initialized.");
            logger.LogInformation("Running on {OSDescription}", RuntimeInformation.OSDescription);
            if (settings.Force)
                logger.LogWarning("Bypassing sudo/admin requirements!");

            try
            {
                AppBootstrap.PlatformStartup?.Invoke();

                logger.LogInformation("Application finished, bye bye!");
                return 0;
            }
            catch (OperationCanceledException)
            {
                logger.LogWarning("\nOperation cancelled. Shutting down, bye-bye");
                return 0;
            }
            finally
            {
                await Log.CloseAndFlushAsync();
            }
        }
        catch (Exception)
        {
            await Log.CloseAndFlushAsync();
            throw;
        }
    }
}
