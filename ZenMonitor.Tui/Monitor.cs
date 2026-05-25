// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using Microsoft.Extensions.Logging;

using Terminal.Gui.App;
using Terminal.Gui.Configuration;

using ZenMonitor.Core.Abstractions;
using ZenMonitor.Tui.Views;

namespace ZenMonitor.Tui;

#region Primary Constructor

public class Monitor(
    ILogger<Monitor> logger,
    IHardwareMonitor monitor)
{
    private readonly ILogger<Monitor> _logger = logger;
    private readonly IHardwareMonitor _monitor = monitor;

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
            var window = new Window(_monitor);

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
            _monitor.Cpu.Update();
            _monitor.Drive.Update();
            _monitor.Gpu.Update();
            _monitor.Memory.Update();
            _monitor.Network.Update();
            _monitor.System.Update();

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
