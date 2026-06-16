// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using CommunityToolkit.Mvvm.ComponentModel;

using SukiUI.Enums;

namespace ZenMonitor.Desktop.ViewModels;

public partial class ViewModelBase : ObservableValidator
{
    [ObservableProperty] public partial string Title { get; set; } = string.Empty;

    protected record BackgroundStyleChangedMessage(SukiBackgroundStyle Style);

    [ObservableProperty]
    public partial SukiBackgroundStyle CurrentBackgroundStyle { get; set; } = SukiBackgroundStyle.Flat;
}
