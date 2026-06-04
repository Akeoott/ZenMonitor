// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using System.Runtime.InteropServices;

using Serilog;

using Spectre.Console.Cli;

using ZenMonitor.Setup;

namespace ZenMonitor;

internal class Program
{
    internal static async Task<int> Main(string[] args)
    {
        var app = new CommandApp<Initialize>();
        return await app.RunAsync(args);
    }
}

public class Initialize : AsyncCommand<Config>
{
    protected override async Task<int> ExecuteAsync(
        CommandContext context,
        Config settings,
        CancellationToken cancellationToken)
    {
        var logLevel = Config.ParseSerilogLevel(settings.LogLevel);
        const string logFilePath = "logs/ZenMonitor.log";

        Config.Logging(logLevel, logFilePath);

        using var serviceProvider = DependencyInjection.BuildServiceProvider(out var gpuNotSupported);
        var logger = serviceProvider.GetRequiredService<ILogger<Initialize>>();

        // Debug messages for us, dev's.
        logger.LogWarning("ZenMonitor initialized.");
        logger.LogInformation("Running on {OSDescription}", RuntimeInformation.OSDescription);
        if (settings.NoSudo)
            logger.LogWarning("Bypassing sudo/admin requirements!");
        if (settings.ForceRun)
            logger.LogWarning("Force running! No data will be returned if your OS is not supported.");
        if (gpuNotSupported)
            logger.LogError("Unsupported GPU. Falling back to `Null.Gpu`, no graphics information will be returned.");

        try
        {
            var engine = serviceProvider.GetRequiredService<Setup.Monitor>();
            await engine.InitMonitor(settings.LoopDelay, cancellationToken);
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
            Log.CloseAndFlush();
        }
    }
}
