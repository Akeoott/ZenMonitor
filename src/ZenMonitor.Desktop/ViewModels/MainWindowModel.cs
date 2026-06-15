// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using System.Diagnostics;

using Avalonia;
using Avalonia.Styling;

using CommunityToolkit.Mvvm.Input;

using SukiUI;
using SukiUI.Enums;

namespace ZenMonitor.Desktop.ViewModels;

public class MainWindowModel : ViewModelBase
{
    public RelayCommand<string?> OpenUrlCommand { get; } = new(OpenUrl);

    public RelayCommand ToggleThemeCommand { get; } = new(() => SukiTheme.GetInstance().SwitchBaseTheme());

    public RelayCommand<SukiColor?> ChangeThemeColorCommand { get; } = new(color =>
    {
        if (color is null) return;

        SukiTheme.GetInstance().ChangeColorTheme((SukiColor)color);

        var app = Application.Current;
        if (app == null) return;

        var currentVariant = app.RequestedThemeVariant;

        // Briefly swap it to force tree-wide resource invalidation
        app.RequestedThemeVariant = currentVariant == ThemeVariant.Dark
            ? ThemeVariant.Light
            : ThemeVariant.Dark;

        // Immediately restore the original base theme variant
        app.RequestedThemeVariant = currentVariant;
    });

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
