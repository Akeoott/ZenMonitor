// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using Microsoft.Extensions.Logging;

using Terminal.Gui.App;
using Terminal.Gui.Configuration;

using ZenMonitor.Core.Interfaces;
using ZenMonitor.Tui.Views;

namespace ZenMonitor.Tui;

public class Monitor(
    ILogger<Monitor> logger,
    ICpu cpuInfo,
    IGpu gpuInfo,
    IMemory memoryInfo,
    INetwork networkInfo,
    IDrive driveInfo,
    ISystem systemInfo)
{
    private readonly ILogger<Monitor> _logger = logger;
    private readonly ICpu _cpuInfo = cpuInfo;
    private readonly IGpu _gpuInfo = gpuInfo;
    private readonly IMemory _memoryInfo = memoryInfo;
    private readonly INetwork _networkInfo = networkInfo;
    private readonly IDrive _driveInfo = driveInfo;
    private readonly ISystem _systemInfo = systemInfo;

    public async Task InitMonitor(int loopDelay, CancellationToken cts)
    {
        ConfigurationManager.Enable(ConfigLocations.All);
        ConfigurationManager.RuntimeConfig = """
        { "Theme": "GreenOnBlack" }
        """;

        IApplication app = Application.Create().Init();
        try
        {
            var window = new Window(
                _cpuInfo, _gpuInfo, _memoryInfo, _driveInfo, _networkInfo, _systemInfo);

            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cts);
            var backgroundTask = Task.Run(() =>
                RunDataLoop(loopDelay, app, window, linkedCts.Token), cts);

            app.Run(window);

            linkedCts.Cancel();
            await backgroundTask;
        }
        finally
        {
            app.Dispose();
        }
    }

    private async Task RunDataLoop(
        int loopDelay, IApplication app, Window window,
        CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            _cpuInfo.Update();
            _gpuInfo.Update();
            _memoryInfo.Update();
            _driveInfo.Update();
            _systemInfo.Update();

            // Marshal UI refresh to the main terminal thread
            app.Invoke(() => window.RefreshAll());

            try
            {
                await Task.Delay(loopDelay, token);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
        _logger.LogTrace("Background data loop stopped.");
    }
}
