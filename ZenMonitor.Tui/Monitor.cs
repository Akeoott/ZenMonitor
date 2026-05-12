// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using Microsoft.Extensions.Logging;

using Terminal.Gui.App;
using Terminal.Gui.Configuration;

using ZenMonitor.Core.Interfaces;
using ZenMonitor.Tui.Views;

namespace ZenMonitor.Tui;

#region Primary Constructor

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
    private readonly IDrive _driveInfo = driveInfo;
    private readonly IGpu _gpuInfo = gpuInfo;
    private readonly IMemory _memoryInfo = memoryInfo;
    private readonly INetwork _networkInfo = networkInfo;
    private readonly ISystem _systemInfo = systemInfo;

    #endregion

    #region Public Methods

    public async Task InitMonitor(int loopDelay, CancellationToken cancellationToken)
    {
        ConfigurationManager.Enable(ConfigLocations.All);
        ConfigurationManager.RuntimeConfig = """
        { "Theme": "GreenOnBlack" }
        """;

        IApplication app = Application.Create().Init();
        try
        {
            var window = new Window(
                _cpuInfo, _driveInfo, _gpuInfo, _memoryInfo, _networkInfo, _systemInfo);

            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            var backgroundTask = Task.Run(() =>
                DataLoop(loopDelay, app, window, linkedCts.Token), cancellationToken);

            app.Run(window);

            linkedCts.Cancel();
            await backgroundTask;
        }
        finally
        {
            app.Dispose();
        }
    }

    #endregion

    #region Private Methods

    private async Task DataLoop(
        int loopDelay, IApplication app, Window window,
        CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            _cpuInfo.Update();
            _driveInfo.Update();
            _gpuInfo.Update();
            _memoryInfo.Update();
            _networkInfo.Update();
            _systemInfo.Update();

            app.Invoke(window.RefreshData);

            try
            {
                await Task.Delay(loopDelay, token);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
        _logger.LogDebug("Background data loop stopped.");
    }

    #endregion
}
