// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Serilog;
using Serilog.Events;

using ZenMonitor.Core.Hosting;

namespace ZenMonitor;

internal class ProgramHelper
{
    #region Dependency Injection
    internal static ServiceProvider BuildServiceProvider(ProgramSettings settings, out bool gpuNotSupported)
    {
        var services = new ServiceCollection();

        services.AddLogging(builder =>
        {
            builder.ClearProviders();
            builder.AddSerilog(dispose: true);
        });

        // All ZenMonitor platform services (Linux/Win detection, GPU auto-detect)
        services.AddZenMonitor(out gpuNotSupported);

        // Mode-specific UI monitors
        switch (settings.Mode)
        {
            case "tui":
                services.AddTransient<Tui.Monitor>();
                break;
            case "gui":
                //services.AddTransient<Gui.Monitor>();
                break;
        }

        return services.BuildServiceProvider();
    }
    #endregion

    #region Logging Config
    internal static void ConfigureLogging(LogEventLevel logLevel, string logFilePath)
    {
        Directory.CreateDirectory("logs");
        File.WriteAllText(logFilePath, string.Empty);

        var loggerConfig = new LoggerConfiguration()
            .MinimumLevel.Is(logLevel)
            .Enrich.WithProperty("RunId", Guid.NewGuid());

        loggerConfig.WriteTo.File(
            logFilePath,
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{RunId}] [{SourceContext}] {Message:lj}{NewLine}{Exception}");

        Log.Logger = loggerConfig.CreateLogger();
    }

    internal static LogEventLevel ParseSerilogLevel(string level)
    {
        return level?.ToLowerInvariant() switch
        {
            "t" or "trace" => LogEventLevel.Verbose,
            "d" or "debug" => LogEventLevel.Debug,
            "i" or "info" => LogEventLevel.Information,
            "w" or "warning" => LogEventLevel.Warning,
            "e" or "error" => LogEventLevel.Error,
            "c" or "critical" => LogEventLevel.Fatal,
            _ => LogEventLevel.Information
        };
    }
    #endregion
}
