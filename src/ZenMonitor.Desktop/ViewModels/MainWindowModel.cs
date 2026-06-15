// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using System.Diagnostics;

using Avalonia;
using Avalonia.Styling;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using SukiUI;
using SukiUI.Enums;

namespace ZenMonitor.Desktop.ViewModels;

public partial class MainWindowModel(
    ProcessesViewModel processesViewModel,
    PerformanceViewModel performanceViewModel,
    ControllerViewModel controllerViewModel,
    SettingsViewModel settingsViewModel) : ViewModelBase
{
    public ProcessesViewModel ProcessesViewModel { get; } = processesViewModel;
    public PerformanceViewModel PerformanceViewModel { get; } = performanceViewModel;
    public ControllerViewModel ControllerViewModel { get; } = controllerViewModel;
    public SettingsViewModel SettingsViewModel { get; } = settingsViewModel;

    [ObservableProperty]
    public partial SukiBackgroundStyle CurrentBackgroundStyle { get; set; } = SukiBackgroundStyle.Flat;

    public RelayCommand<string?> OpenUrlCommand { get; } = new(OpenUrl);
    public RelayCommand ToggleThemeCommand { get; } = new(() => SukiTheme.GetInstance().SwitchBaseTheme());

    public RelayCommand<SukiBackgroundStyle?> ChangeBackgroundThemeCommand =>
        field ??= new RelayCommand<SukiBackgroundStyle?>(style =>
        {
            if (style is not null)
                CurrentBackgroundStyle = style.Value;

            var app = Application.Current;
            if (app == null) return;

            ForceThemeUpdate(app);
        });

    public RelayCommand<SukiColor?> ChangeColorThemeCommand { get; } = new(color =>
    {
        if (color is null) return;

        SukiTheme.GetInstance().ChangeColorTheme((SukiColor)color);

        var app = Application.Current;
        if (app == null) return;

        ForceThemeUpdate(app);
    });

    private static void ForceThemeUpdate(Application app)
    {
        var currentVariant = app.RequestedThemeVariant;

        // Briefly swap it to force tree-wide resource invalidation
        app.RequestedThemeVariant = currentVariant == ThemeVariant.Dark
            ? ThemeVariant.Light
            : ThemeVariant.Dark;
        // Immediately restore the original base theme variant
        app.RequestedThemeVariant = currentVariant;
    }

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
