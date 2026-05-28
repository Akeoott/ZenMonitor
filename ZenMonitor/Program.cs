// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using System.Runtime.InteropServices;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Serilog;

using Spectre.Console.Cli;

namespace ZenMonitor;

internal class Program
{
    internal static async Task<int> Main(string[] args)
    {
        var app = new CommandApp<InitProgram>();
        return await app.RunAsync(args);
    }
}

public class InitProgram : AsyncCommand<ProgramSettings>
{
    #region InitProgram
    protected override async Task<int> ExecuteAsync(
        CommandContext context,
        ProgramSettings settings,
        CancellationToken cancellationToken)
    {
        var logLevel = ProgramHelper.ParseSerilogLevel(settings.LogLevel);
        const string logFilePath = "logs/ZenMonitor.log";

        ProgramHelper.ConfigureLogging(logLevel, logFilePath);

        using var serviceProvider = ProgramHelper.BuildServiceProvider(settings, out var gpuNotSupported);
        var logger = serviceProvider.GetRequiredService<ILogger<InitProgram>>();

        // Debug messages for us, dev's.
        logger.LogWarning("ZenMonitor initialized.");
        logger.LogInformation("Running on {OSDescription}", RuntimeInformation.OSDescription);
        logger.LogInformation("OutputMode: {OutputMode}", settings.Mode);
        if (settings.NoSudo)
            logger.LogWarning("Bypassing sudo/admin requirements!");
        if (settings.ForceRun)
            logger.LogWarning("Force running! No data will be returned if your OS is not supported.");
        if (gpuNotSupported)
            logger.LogError("Unsupported GPU. Falling back to `Null.Gpu`, no graphics information will be returned.");

        try
        {
            await RunApplicationAsync(serviceProvider, settings, cancellationToken);
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
    #endregion

    #region Run Application
    private static async Task RunApplicationAsync(ServiceProvider serviceProvider, ProgramSettings settings, CancellationToken cancellationToken)
    {
        switch (settings.Mode)
        {
            case "tui":
                {
                    var engine = serviceProvider.GetRequiredService<Tui.Monitor>();
                    await engine.InitMonitor(settings.LoopDelay, cancellationToken);
                    break;
                }
            case "gui":
                {
                    Console.WriteLine("gui is not implemented, come back later! (try tui)");
                    break;
                }
            default:
                throw new InvalidOperationException($"Unsupported mode: {settings.Mode}");
        }
    }
    #endregion
}
