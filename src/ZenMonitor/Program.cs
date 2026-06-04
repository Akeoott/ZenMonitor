// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using System.Runtime.InteropServices;

using Avalonia;

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

    public static void InitAvalonia()
    {
        BuildAvaloniaApp().StartWithClassicDesktopLifetime([]);
    }

    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
#if DEBUG
            .WithDeveloperTools()
#endif
            .WithInterFont()
            .LogToTrace();
    }
}

public class InitProgram : AsyncCommand<Config>
{
    protected override async Task<int> ExecuteAsync(
        CommandContext context,
        Config settings,
        CancellationToken cancellationToken)
    {
        var logLevel = Config.ParseSerilogLevel(settings.Verbosity);
        const string logFilePath = "logs/ZenMonitor.log";

        Config.ConfigureLogging(settings.Quiet, logLevel, logFilePath);

        using var serviceProvider = DependencyInjection.BuildServiceProvider(out var gpuNotSupported);
        var logger = serviceProvider.GetRequiredService<ILogger<InitProgram>>();

        // Debug messages for us, dev's.
        logger.LogWarning("ZenMonitor initialized.");
        logger.LogInformation("Running on {OSDescription}", RuntimeInformation.OSDescription);
        if (settings.Force)
            logger.LogWarning("Bypassing sudo/admin requirements!");
        if (gpuNotSupported)
            logger.LogError("Unsupported GPU. Falling back to `Null.Gpu`, no graphics information will be returned.");

        try
        {
            Program.InitAvalonia();
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
