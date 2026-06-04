// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using System.Runtime.InteropServices;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Serilog;

using Spectre.Console.Cli;

using ZenMonitor.Init;

namespace ZenMonitor;

internal class Program
{
    internal static async Task<int> Main(string[] args)
    {
        var app = new CommandApp<InitProgram>();
        return await app.RunAsync(args);
    }
}

public class InitProgram : AsyncCommand<Config>
{
    protected override async Task<int> ExecuteAsync(
        CommandContext context,
        Config settings,
        CancellationToken cancellationToken)
    {
        var logLevel = Config.ParseSerilogLevel(settings.LogLevel);
        const string logFilePath = "logs/ZenMonitor.log";

        Config.ConfigureLogging(logLevel, logFilePath);

        using var serviceProvider = DependencyInjection.BuildServiceProvider(out var gpuNotSupported);
        var logger = serviceProvider.GetRequiredService<ILogger<InitProgram>>();

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
            Init.Monitor.InitAvalonia();
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
