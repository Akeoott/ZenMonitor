// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using System.Runtime.InteropServices;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Serilog;
using Serilog.Events;

using ZenMonitor.Core.Interfaces;

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

        services.AddSingleton<System.IO.Abstractions.IFileSystem, System.IO.Abstractions.FileSystem>();

        gpuNotSupported = false;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            services.AddSingleton<IHelper, Core.Services.Linux.Helper>();
            services.AddSingleton<ICpu, Core.Services.Linux.Cpu>();
            services.AddSingleton<IDrive, Core.Services.Linux.Drive>();

            if (Directory.Exists("/proc/driver/nvidia"))
            {
                services.AddSingleton<IGpu, Core.Services.Linux.GpuNvidia>();
            }
            else if (Directory.Exists("/sys/class/drm/card0/device/hwmon"))
            {
                services.AddSingleton<IGpu, Core.Services.Linux.GpuAmd>();
            }
            else
            {
                gpuNotSupported = true;
                services.AddSingleton<IGpu, Core.Services.Null.Gpu>();
            }

            services.AddSingleton<IMemory, Core.Services.Linux.Memory>();
            services.AddSingleton<INetwork, Core.Services.Linux.Network>();
            services.AddSingleton<ISystem, Core.Services.Linux.System>();
        }
        /*
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {

        }
        */
        else
        {
            if (settings.ForceRun)
            {
                services.AddSingleton<ICpu, Core.Services.Null.Cpu>();
                services.AddSingleton<IDrive, Core.Services.Null.Drive>();
                services.AddSingleton<IGpu, Core.Services.Null.Gpu>();
                services.AddSingleton<IMemory, Core.Services.Null.Memory>();
                services.AddSingleton<INetwork, Core.Services.Null.Network>();
                services.AddSingleton<ISystem, Core.Services.Null.System>();
            }
            else
            {
                throw new PlatformNotSupportedException(
                    "ZenMonitor only supports Linux at the moment.");
            }
        }

        switch (settings.Mode)
        {
            case "cli":
                services.AddTransient<Cli.Monitor>();
                break;
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
    internal static void ConfigureLogging(LogEventLevel logLevel, string logFilePath, bool cliLogging)
    {
        Directory.CreateDirectory("logs");
        File.WriteAllText(logFilePath, string.Empty);

        var loggerConfig = new LoggerConfiguration()
            .MinimumLevel.Is(logLevel)
            .Enrich.WithProperty("RunId", Guid.NewGuid());

        if (cliLogging)
        {
            loggerConfig.WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss.fff} {Level:u3}] {Message:lj}{NewLine}{Exception}");
        }

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

    #region Log + Safety Checks
    internal static void ApplyRuntimeSafetyChecks(
        ProgramSettings settings,
        Microsoft.Extensions.Logging.ILogger logger,
        bool gpuNotSupported)
    {
        logger.LogWarning("ZenMonitor initialized.");

        logger.LogInformation("Running on {OSDescription}", RuntimeInformation.OSDescription);

        if (settings.NoSudo)
        {
            logger.LogWarning("Bypassing sudo/admin requirements!");
        }

        if (settings.ForceRun)
        {
            logger.LogWarning("Force running! No data will be returned if your OS is not supported.");
        }

        if (gpuNotSupported)
        {
            logger.LogError("Unsupported GPU. Falling back to `Null.Gpu`, no graphics information will be returned.");
        }

        logger.LogInformation("OutputMode: {OutputMode}", settings.Mode);

        if (settings.LoopDelay > 10000)
        {
            settings.LoopDelay = 10000;
            logger.LogWarning("LoopDelay exceeds 10 seconds. Setting back to a maximum of 10 seconds.");
        }
        else if (settings.LoopDelay < 100)
        {
            settings.LoopDelay = 100;
            logger.LogWarning("LoopDelay is below 0.1 seconds. Setting back to a minimum of 0.1 seconds.");
        }
    }
    #endregion
}
