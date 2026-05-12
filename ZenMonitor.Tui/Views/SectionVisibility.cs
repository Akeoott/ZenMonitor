// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

namespace ZenMonitor.Tui.Views;

/// <summary>
/// Tracks which sections are visible in the TUI dashboard.
/// All sections visible by default.
/// </summary>
public sealed record SectionVisibility
{
    public bool Cpu { get; set; } = true;
    public bool Gpu { get; set; } = true;
    public bool MemDisk { get; set; } = true;
    public bool Network { get; set; } = true;
    public bool Placeholder { get; set; } = true;

    /// <summary>
    /// Toggle a section by its 0-based index
    /// (0=CPU, 1=GPU, 2=Mem+Disk, 3=Network, 4=Placeholder).
    /// </summary>
    public void Toggle(int index)
    {
        switch (index)
        {
            case 0: Cpu = !Cpu; break;
            case 1: Gpu = !Gpu; break;
            case 2: MemDisk = !MemDisk; break;
            case 3: Network = !Network; break;
            case 4: Placeholder = !Placeholder; break;
        }
    }
}
