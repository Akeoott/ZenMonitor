// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace ZenMonitor.Tui.Views;

/// <summary>
/// Placeholder section for future use. Rendered alongside Memory/Disks/Network
/// in the right column of the bottom area.
/// </summary>
public sealed class PlaceholderSection : View
{
    private readonly FrameView _frame;
    private readonly Label _infoLabel;

    public PlaceholderSection()
    {
        Width = Dim.Fill();

        _frame = new FrameView
        {
            Title = "Placeholder [5]",
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill()
        };

        _infoLabel = new Label
        {
            X = Pos.Center(),
            Y = Pos.Center(),
            Text = "Reserved for future use"
        };

        _frame.Add(_infoLabel);
        Add(_frame);
    }

    public void Refresh()
    {
    }
}
