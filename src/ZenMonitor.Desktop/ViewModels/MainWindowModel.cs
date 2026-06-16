// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using System.Diagnostics;

using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace ZenMonitor.Desktop.ViewModels;

public class MainWindowModel : ViewModelBase
{
    public MainWindowModel(
        ProcessesViewModel processesViewModel,
        PerformanceViewModel performanceViewModel,
        ControllerViewModel controllerViewModel,
        SettingsViewModel settingsViewModel)
    {
        ProcessesViewModel = processesViewModel;
        PerformanceViewModel = performanceViewModel;
        ControllerViewModel = controllerViewModel;
        SettingsViewModel = settingsViewModel;

        WeakReferenceMessenger.Default.Register<BackgroundStyleChangedMessage>(this, (_, message) =>
        {
            CurrentBackgroundStyle = message.Style;
        });
    }

    public ProcessesViewModel ProcessesViewModel { get; }
    public PerformanceViewModel PerformanceViewModel { get; }
    public ControllerViewModel ControllerViewModel { get; }
    public SettingsViewModel SettingsViewModel { get; }

    public RelayCommand<string?> OpenUrlCommand { get; } = new(OpenUrl);

    private static void OpenUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            Process.Start(new ProcessStartInfo(url.Replace("&", "^&")) { UseShellExecute = true });
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            Process.Start("xdg-open", url);
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            Process.Start("open", url);
    }
}
