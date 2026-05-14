// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

using ZenMonitor.Core.Abstractions;

namespace ZenMonitor.Tui.Views;

/// <summary>
/// Network section - currently a stub as INetwork is not fully implemented yet.
/// </summary>
public sealed class NetworkSection : View
{
    private readonly INetwork _network;
    private readonly FrameView _frame;
    private readonly Label _infoLabel;

    public NetworkSection(INetwork network)
    {
        _network = network;
        Width = Dim.Fill();

        _frame = new FrameView
        {
            Title = "Network [4]",
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill()
        };

        _infoLabel = new Label
        {
            X = 1,
            Y = 0,
            Text = "Network monitoring not yet implemented."
        };

        _frame.Add(_infoLabel);
        Add(_frame);
    }

    public void Refresh()
    {
        _infoLabel.Text = "Network monitoring not yet implemented.";
    }
}
