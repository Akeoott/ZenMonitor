// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using System.Runtime.InteropServices;
using System.Security.Principal;

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
    [DllImport("libc")]
    private static extern uint geteuid();

    private static bool IsRoot() => RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && geteuid() == 0;
    private static bool IsAdmin() => RuntimeInformation.IsOSPlatform(OSPlatform.Windows) &&
        new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);

    protected override async Task<int> ExecuteAsync(
        CommandContext context,
        ProgramSettings settings,
        CancellationToken cancellationToken)
    {
        if (!settings.NoSudo)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && !IsRoot())
            {
                Console.Error.WriteLine("ZenMonitor requires root privileges. Please run with sudo.");
                return 1;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && !IsAdmin())
            {
                Console.Error.WriteLine("ZenMonitor requires admin privileges. Please run as admin.");
                return 1;
            }
        }

        try
        {
            var logLevel = ProgramHelper.ParseSerilogLevel(settings.LogLevel);
            const string logFilePath = "logs/ZenMonitor.log";

            ProgramHelper.ConfigureLogging(logLevel, logFilePath);

            using var serviceProvider = ProgramHelper.BuildServiceProvider(settings, out var gpuNotSupported);
            var logger = serviceProvider.GetRequiredService<ILogger<InitProgram>>();

            ProgramHelper.ApplyRuntimeSafetyChecks(settings, logger, gpuNotSupported);

            await RunApplicationAsync(serviceProvider, settings, cancellationToken);
            logger.LogInformation("Application finished, bye bye!");

            return 0;
        }
        catch (PlatformNotSupportedException ex)
        {
            Console.Error.WriteLine(ex.Message);
            Console.Write("Press any key to exit... ");
            Console.ReadKey();
            return 1;
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
                    Console.WriteLine("gui is not implemented, come back later! (try cli)");
                    break;
                }
            default:
                throw new InvalidOperationException($"Unsupported mode: {settings.Mode}");
        }
    }
    #endregion
}
