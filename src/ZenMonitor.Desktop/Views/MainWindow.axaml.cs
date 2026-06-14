// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;

using SukiUI.Controls;

namespace ZenMonitor.Desktop.Views;

public partial class MainWindow : SukiWindow
{
    private const int ResizeMargin = 8;

    public MainWindow()
    {
        InitializeComponent();

        // Without this, cant resize window
        PointerMoved += OnWindowPointerMoved;
        PointerPressed += OnWindowPointerPressed;
    }

    private void OnWindowPointerMoved(object? sender, PointerEventArgs e)
    {
        var position = e.GetPosition(this);
        GetWindowEdge(position);
    }

    private void OnWindowPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        var position = e.GetPosition(this);
        var edge = GetWindowEdge(position);

        if (edge == (WindowEdge)(-1)
            || !e.GetCurrentPoint(this).Properties.IsLeftButtonPressed) return;

        BeginResizeDrag(edge, e);
        e.Handled = true;
    }

    private WindowEdge GetWindowEdge(Point point)
    {
        var top = point.Y < ResizeMargin;
        var bottom = point.Y > Bounds.Height - ResizeMargin;
        var left = point.X < ResizeMargin;
        var right = point.X > Bounds.Width - ResizeMargin;

        switch (top)
        {
            case true when left:
                return WindowEdge.NorthWest;
            case true when right:
                return WindowEdge.NorthEast;
        }

        switch (bottom)
        {
            case true when left:
                return WindowEdge.SouthWest;
            case true when right:
                return WindowEdge.SouthEast;
        }

        if (top) return WindowEdge.North;
        if (bottom) return WindowEdge.South;
        if (left) return WindowEdge.West;
        if (right) return WindowEdge.East;

        return (WindowEdge)(-1);
    }
}
