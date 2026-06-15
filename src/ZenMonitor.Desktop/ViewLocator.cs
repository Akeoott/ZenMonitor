// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using System.Reflection;

using Avalonia.Controls;
using Avalonia.Controls.Templates;

using ZenMonitor.Desktop.ViewModels;

namespace ZenMonitor.Desktop;

public class ViewLocator : IDataTemplate
{
    public Control? Build(object? param)
    {
        if (param is null)
            return null;

        var name = param.GetType().FullName!.Replace("ViewModel", "View", StringComparison.Ordinal);
        var type = Assembly.GetExecutingAssembly().GetType(name);

        if (type is null)
            return new TextBlock { Text = $"View not found: {name}" };

        var control = (Control)Activator.CreateInstance(type)!;
        control.DataContext = param;
        return control;
    }

    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
}
