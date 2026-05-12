// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using Terminal.Gui.Drawing;
using Terminal.Gui.Drivers;
using Terminal.Gui.Input;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

using ZenMonitor.Core.Interfaces;

namespace ZenMonitor.Tui.Views;

#region Class & Fields

/// <summary>
/// Main TUI window with a grid layout:
/// Key map: 1=CPU, 2=GPU, 3=Mem+Disk, 4=Placeholder, 5=Network.
/// </summary>
public sealed class Window : Runnable<bool>
{
    private const int ZoneAHeightPercent = 30;
    private const int PanelSplitPercent = 50;
    private const int LeftPanelInternalSplitPercent = 50;

    private readonly HeaderView _header;
    private readonly CpuSection _cpuSection;
    private readonly GpuSection _gpuSection;
    private readonly MemoryDiskSection _memDiskSection;
    private readonly NetworkSection _networkSection;
    private readonly PlaceholderSection _placeholderSection;
    private readonly Label _noSectionsLabel;

    private readonly View _zoneA;
    private readonly View _zoneB;
    private readonly View _leftPanel;
    private readonly View _rightPanel;
    private readonly List<View> _zoneContainers;

    // Reusable buffer to avoid allocating a new List on every layout pass
    private readonly List<View> _visibleZonesBuffer = new(capacity: 2);

    /// <summary>
    /// Gets or sets the visibility state of all sections.
    /// Modify this property, then call <see cref="RecalculateLayout"/> to reflow.
    /// </summary>
    public SectionVisibility SectionVisibility { get; set; } = new();

    #endregion

    #region Constructor

    public Window(
        ICpu cpu,
        IDrive drive,
        IGpu gpu,
        IMemory memory,
        INetwork network,
        ISystem system)
    {
        Title = "ZenMonitor";
        BorderStyle = LineStyle.Rounded;

        _header = new HeaderView(system);
        Add(_header);

        _cpuSection = new CpuSection(cpu);
        _gpuSection = new GpuSection(gpu);
        _memDiskSection = new MemoryDiskSection(memory, drive);
        _networkSection = new NetworkSection(network);
        _placeholderSection = new PlaceholderSection();

        // Zone A - CPU | GPU (top row)
        _zoneA = new View
        {
            X = 0,
            Width = Dim.Fill(),
            Y = Pos.Bottom(_header),
            Height = 0,
            CanFocus = false,
            Visible = false
        };
        _cpuSection.X = 0;
        _cpuSection.Width = Dim.Percent(50);
        _cpuSection.Y = 0;
        _cpuSection.Height = Dim.Fill();
        _gpuSection.X = Pos.Percent(50);
        _gpuSection.Width = Dim.Fill();
        _gpuSection.Y = 0;
        _gpuSection.Height = Dim.Fill();
        _zoneA.Add(_cpuSection, _gpuSection);

        // Zone B - LeftPanel | RightPanel (mid area)
        _zoneB = new View
        {
            X = 0,
            Width = Dim.Fill(),
            Y = Pos.Bottom(_zoneA),
            Height = 0,
            CanFocus = false,
            Visible = false
        };

        _leftPanel = new View
        {
            X = 0,
            Width = Dim.Percent(50),
            Y = 0,
            Height = Dim.Fill(),
            CanFocus = false
        };
        _memDiskSection.X = 0;
        _memDiskSection.Width = Dim.Fill();
        _memDiskSection.Y = 0;
        _memDiskSection.Height = Dim.Fill();
        _networkSection.X = 0;
        _networkSection.Width = Dim.Fill();
        _networkSection.Y = Pos.Bottom(_memDiskSection);
        _networkSection.Height = Dim.Fill();
        _leftPanel.Add(_memDiskSection, _networkSection);

        _rightPanel = new View
        {
            X = Pos.Percent(50),
            Width = Dim.Fill(),
            Y = 0,
            Height = Dim.Fill(),
            CanFocus = false
        };
        _placeholderSection.X = 0;
        _placeholderSection.Width = Dim.Fill();
        _placeholderSection.Y = 0;
        _placeholderSection.Height = Dim.Fill();
        _rightPanel.Add(_placeholderSection);

        _zoneB.Add(_leftPanel, _rightPanel);

        _zoneContainers = [_zoneA, _zoneB];
        foreach (var z in _zoneContainers) Add(z);

        _noSectionsLabel = new Label
        {
            X = Pos.Center(),
            Y = Pos.Center(),
            Text = "No sections selected - press 1-5 to show a section",
            Visible = false
        };
        Add(_noSectionsLabel);

        RecalculateLayout();

        AddCommand(Command.Quit, () =>
        {
            App?.RequestStop();
            return true;
        });
        KeyBindings.Add(Key.Esc, Command.Quit);
    }

    #endregion

    #region Key Handling

    protected override bool OnKeyDown(Key key)
    {
        KeyCode code = (KeyCode)key;

        int index = code switch
        {
            KeyCode.D1 => 0,
            KeyCode.D2 => 1,
            KeyCode.D3 => 2,
            KeyCode.D4 => 3,
            KeyCode.D5 => 4,
            _ => -1
        };

        if (index >= 0)
        {
            SectionVisibility.Toggle(index);
            RecalculateLayout();
            return true;
        }

        if ((code & KeyCode.CtrlMask) != 0 && (code & ~KeyCode.CtrlMask) == KeyCode.Q)
        {
            App?.RequestStop();
            return true;
        }

        return base.OnKeyDown(key);
    }

    #endregion

    #region Layout

    /// <summary>
    /// Reflows the grid layout based on current visibility.
    /// Zones and sections adapt proportionally to fill available space.
    /// </summary>
    public void RecalculateLayout()
    {
        ArgumentNullException.ThrowIfNull(SectionVisibility);

        SectionVisibility vis = SectionVisibility;
        ApplySectionVisibility(vis);

        bool leftHasContent = ArrangeLeftPanel(vis);
        ArrangeZoneB(vis, leftHasContent);
        ArrangeZones(vis, leftHasContent);
    }

    private void ApplySectionVisibility(SectionVisibility vis)
    {
        _cpuSection.Visible = vis.Cpu;
        _gpuSection.Visible = vis.Gpu;
        _memDiskSection.Visible = vis.MemDisk;
        _networkSection.Visible = vis.Network;
        _placeholderSection.Visible = vis.Placeholder;
    }

    /// <summary>
    /// Arranges the left panel (MemDisk + Network).
    /// Returns <c>true</c> if the left panel has any visible content.
    /// </summary>
    private bool ArrangeLeftPanel(SectionVisibility vis)
    {
        bool hasContent = vis.MemDisk || vis.Network;
        if (!hasContent)
        {
            _leftPanel.Visible = false;
            return false;
        }

        _leftPanel.Visible = true;

        if (vis.MemDisk && vis.Network)
        {
            // 50/50 split between MemDisk and Network
            _memDiskSection.Height = Dim.Percent(LeftPanelInternalSplitPercent);
            _networkSection.Height = Dim.Fill();
            _networkSection.Y = Pos.Bottom(_memDiskSection);
        }
        else if (vis.MemDisk)
        {
            // Only MemDisk visible: fills left panel
            _memDiskSection.Height = Dim.Fill();
            _networkSection.Height = 0;
        }
        else
        {
            // Only Network visible: fills left panel
            _memDiskSection.Height = 0;
            _networkSection.Y = 0;
            _networkSection.Height = Dim.Fill();
        }

        return true;
    }

    /// <summary>
    /// Arranges the Zone B split between the left panel and the right (placeholder) panel.
    /// </summary>
    private void ArrangeZoneB(SectionVisibility vis, bool leftHasContent)
    {
        if (vis.Placeholder)
        {
            _rightPanel.Visible = true;
            _leftPanel.Width = Dim.Percent(PanelSplitPercent);
            _rightPanel.X = Pos.Percent(PanelSplitPercent);
            _rightPanel.Width = Dim.Fill();
        }
        else
        {
            _rightPanel.Visible = false;
            _leftPanel.Width = Dim.Fill();
        }
    }

    /// <summary>
    /// Arranges the top-level zone containers (<see cref="_zoneA"/> and <see cref="_zoneB"/>).
    /// Shows or hides them based on content, sizes them proportionally,
    /// and handles the "no sections" fallback label.
    /// </summary>
    private void ArrangeZones(SectionVisibility vis, bool leftHasContent)
    {
        bool zoneAHasContent = vis.Cpu || vis.Gpu;
        bool zoneBHasContent = leftHasContent || vis.Placeholder;

        foreach (View z in _zoneContainers)
            z.Visible = false;

        if (!zoneAHasContent && !zoneBHasContent)
        {
            _noSectionsLabel.Visible = true;
            return;
        }
        _noSectionsLabel.Visible = false;

        _visibleZonesBuffer.Clear();
        if (zoneAHasContent) _visibleZonesBuffer.Add(_zoneA);
        if (zoneBHasContent) _visibleZonesBuffer.Add(_zoneB);

        for (int i = 0; i < _visibleZonesBuffer.Count; i++)
        {
            View zone = _visibleZonesBuffer[i];
            zone.Visible = true;

            if (_visibleZonesBuffer.Count == 1)
            {
                zone.Height = Dim.Fill();
            }
            else if (i == 0)
            {
                zone.Height = Dim.Percent(ZoneAHeightPercent);
            }
            else
            {
                zone.Height = Dim.Fill();
            }
        }
    }

    #endregion

    #region Refresh

    /// <summary>
    /// Updates all visible section data from the background loop.
    /// </summary>
    public void RefreshData()
    {
        _header.Refresh();
        _cpuSection.Refresh();
        _gpuSection.Refresh();
        _memDiskSection.Refresh();
        _networkSection.Refresh();
        _placeholderSection.Refresh();
    }

    #endregion
}
