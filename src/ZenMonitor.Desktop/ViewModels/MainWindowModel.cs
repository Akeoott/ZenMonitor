// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SukiUI;

namespace ZenMonitor.Desktop.ViewModels;

public partial class MainWindowModel : ViewModelBase
{
    private readonly HomeViewModel _homeViewModel;
    private readonly SettingsViewModel _settingsViewModel;

    [ObservableProperty]
    private ViewModelBase? _currentViewModel;

    [ObservableProperty]
    private bool _isDarkTheme;

    public MainWindowModel(HomeViewModel homeViewModel, SettingsViewModel settingsViewModel)
    {
        _homeViewModel = homeViewModel;
        _settingsViewModel = settingsViewModel;

        // Start by showing the Home page.
        CurrentViewModel = _homeViewModel;
    }

    public string ThemeToggleIcon => IsDarkTheme ? "\u2600" : "\uD83C\uDF19";

    public ISolidColorBrush? HomeButtonBackground =>
        CurrentViewModel is HomeViewModel
            ? (IBrush?)Application.Current?.FindResource("SukiPrimaryColor") as ISolidColorBrush
            : Brushes.Transparent;

    public ISolidColorBrush? SettingsButtonBackground =>
        CurrentViewModel is SettingsViewModel
            ? (IBrush?)Application.Current?.FindResource("SukiPrimaryColor") as ISolidColorBrush
            : Brushes.Transparent;

    partial void OnCurrentViewModelChanged(ViewModelBase? value)
    {
        OnPropertyChanged(nameof(HomeButtonBackground));
        OnPropertyChanged(nameof(SettingsButtonBackground));
    }

    partial void OnIsDarkThemeChanged(bool value)
    {
        OnPropertyChanged(nameof(ThemeToggleIcon));
    }

    [RelayCommand]
    private void NavigateHome()
    {
        CurrentViewModel = _homeViewModel;
    }

    [RelayCommand]
    private void NavigateSettings()
    {
        CurrentViewModel = _settingsViewModel;
    }

    [RelayCommand]
    private void ToggleTheme()
    {
        IsDarkTheme = !IsDarkTheme;
        var theme = SukiTheme.GetInstance();
        theme.SwitchBaseTheme();
    }
}
