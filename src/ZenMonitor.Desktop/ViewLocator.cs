// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using Avalonia.Controls;
using Avalonia.Controls.Templates;

using ZenMonitor.Desktop.ViewModels;
using ZenMonitor.Desktop.Views;

namespace ZenMonitor.Desktop;

public class ViewLocator : IDataTemplate
{
    public Control Build(object? data)
    {
        return data switch
        {
            MainWindowModel => new TextBlock { Text = $"No view found for {data?.GetType().Name}" },

            HomeViewModel vm => new HomeView { DataContext = vm },
            SettingsViewModel vm => new SettingsView { DataContext = vm },

            _ => new TextBlock { Text = $"No view found for {data?.GetType().Name}" }
        };
    }

    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
}
