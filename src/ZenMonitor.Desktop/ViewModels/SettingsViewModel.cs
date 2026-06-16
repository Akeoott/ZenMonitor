// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using Avalonia;
using Avalonia.Styling;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using SukiUI;
using SukiUI.Enums;

namespace ZenMonitor.Desktop.ViewModels;

public partial class SettingsViewModel : ViewModelBase
{
    public SettingsViewModel()
    {
        Title = "Settings";
    }

    public RelayCommand ToggleThemeCommand { get; } = new(() => SukiTheme.GetInstance().SwitchBaseTheme());

    [ObservableProperty] public partial SukiColor ActiveColor { get; set; } = SukiColor.Blue;

    public RelayCommand<SukiBackgroundStyle?> ChangeBackgroundThemeCommand =>
        field ??= new RelayCommand<SukiBackgroundStyle?>(style =>
        {
            if (style is not null)
            {
                CurrentBackgroundStyle = style.Value;
                WeakReferenceMessenger.Default.Send(new BackgroundStyleChangedMessage(style.Value));
            }

            var app = Application.Current;
            if (app == null) return;

            ForceThemeUpdate(app);
        });

    public RelayCommand<SukiColor?> ChangeColorThemeCommand =>
        field ??= new RelayCommand<SukiColor?>(color =>
        {
            if (color is null) return;

            ActiveColor = color.Value;
            SukiTheme.GetInstance().ChangeColorTheme(color.Value);

            var app = Application.Current;
            if (app == null) return;

            ForceThemeUpdate(app);
        });

    private static void ForceThemeUpdate(Application app)
    {
        var currentVariant = app.RequestedThemeVariant;
        app.RequestedThemeVariant = currentVariant == ThemeVariant.Dark
            ? ThemeVariant.Light
            : ThemeVariant.Dark;
        app.RequestedThemeVariant = currentVariant;
    }
}
