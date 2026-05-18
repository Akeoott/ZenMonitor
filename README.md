# ZenMonitor

### A light and fast system monitor

![Last Commit](https://img.shields.io/github/last-commit/Akeoott/ZenMonitor?style=for-the-badge&logoSize=auto&labelColor=%23201a19&color=%23ffb4a2)
![Stars](https://img.shields.io/github/stars/Akeoott/ZenMonitor?style=for-the-badge&labelColor=%231d1b16&color=%23e6c419)
![Repo Size](https://img.shields.io/github/repo-size/Akeoott/ZenMonitor?style=for-the-badge&labelColor=%231a1b1f&color=%23a8c7ff)<br>
[![GitHub License](https://img.shields.io/github/license/akeoott/ZenMonitor?style=for-the-badge&logoSize=auto&labelColor=%23201a19&color=%23ffb4a2)](https://github.com/Akeoott/ZenMonitor/blob/main/LICENSE)
[![Code Coverage](https://img.shields.io/codecov/c/github/Akeoott/ZenMonitor?style=for-the-badge&logoSize=auto&labelColor=%231d1b16)](https://codecov.io/gh/Akeoott/ZenMonitor)
[![CodeFactor Grade](https://img.shields.io/codefactor/grade/github/Akeoott/ZenMonitor?style=for-the-badge&logoSize=auto&labelColor=%231a1b1f)](https://www.codefactor.io/repository/github/akeoott/zenmonitor)

### A light and fast system monitor designed to show you exactly what your computer is doing.

<div>
    <a href="https://deepwiki.com/Akeoott/ZenMonitor">
        <img src="https://deepwiki.com/badge.svg" alt="Ask DeepWiki (use with caution)">
    </a>
</div>
<br>

> [!WARNING]
> WIP, limited functionality.<br>
> Backend functional with limitations.<br>
> Tui available with limitations (WIP)<br>
> No Gui available at the moment.

## Quick Start

**Prerequisites:** [.NET SDK 10.0.300](https://dotnet.microsoft.com/en-us/download/dotnet/10.0) (the exact version is pinned in [`global.json`](https://github.com/Akeoott/ZenMonitor/blob/main/global.json)).<br>
**Platform:** Depends on [ZenMonitor.Core](https://github.com/Akeoott/ZenMonitor.Core) (see [Architecture](#architecture) below).

```bash
git clone https://github.com/Akeoott/ZenMonitor
cd ZenMonitor

dotnet restore
dotnet build

# Run the CLI frontend
# (requires sudo/admin privileges, to bypass add `-n` after `-r cli`)
dotnet run --project ZenMonitor -- -r cli
```

> See [CONTRIBUTING.md](https://github.com/Akeoott/ZenMonitor/blob/main/.github/CONTRIBUTING.md) for the full contribution workflow and our [Code of Conduct](https://github.com/Akeoott/ZenMonitor/blob/main/.github/CODE_OF_CONDUCT.md).

---

## Project Summary

ZenMonitor is a modern task manager built on .NET 10.0 (C# only). It uses a modular, interface-driven backend for system telemetry and a Producer-Consumer pattern to decouple data collection from rendering. Currently Linux-only; GUI frontend is planned but not yet implemented.

---

## Architecture

### Design Overview

A background data loop calls `Update()` on each hardware service at a configurable interval. Frontends (CLI, TUI, or planned GUI) can then read the latest snapshot and render it to the user.

- **Dependency Injection**: All hardware services are registered as singletons via `Microsoft.Extensions.DependencyInjection`. Frontends (`Cli`/`Tui`/`Gui`) are registered as transient.
- **Root required**: The app checks for root (Linux) or admin (Windows) privileges at startup unless `--no-sudo` is passed.

### Hardware Interfaces

Hardware abstraction interfaces are defined in, and provided by, the [`ZenMonitor.Core`](https://github.com/Akeoott/ZenMonitor.Core) NuGet package.
Each interface exposes a `void Update()` method plus typed getters:

| Interface | Provides |
|-----------|----------|
| `ICpu`    | CPU usage, temperature, frequency |
| `IDrive`  | Disk I/O, partition usage |
| `IGpu`    | GPU utilization, VRAM |
| `IMemory` | RAM usage, swap |
| `INetwork` | Network throughput, interfaces |
| `ISystem` | OS info, uptime, hostname |

Concrete platform implementations live in the `ZenMonitor.Core` package (Linux only at this time).

### Project Structure

```
ZenMonitor/                Entry point, DI wiring, CLI argument parsing
ZenMonitor.Cli/            CLI frontend
ZenMonitor.Tui/            Terminal.Gui-based TUI frontend
ZenMonitor.Gui/            Planned GUI frontend (not yet implemented)
```

| Project | Description |
|---------|-------------|
| `ZenMonitor` | Entry point (`Program.cs`), DI wiring, logging setup via Serilog, privilege check, and mode dispatch (`cli`/`tui`/`gui`). |
| `ZenMonitor.Cli` | CLI frontend. Injects all hardware interfaces, runs a background data loop and prints raw telemetry values to stdout. |
| `ZenMonitor.Tui` | TUI frontend using **Terminal.Gui**. Injects all hardware interfaces, runs a background data loop, and updates views via `app.Invoke(window.RefreshData)`. |
| `ZenMonitor.Gui` | Planned GUI frontend (not implemented). |

### TUI Views (`ZenMonitor.Tui/Views/`)

- `HeaderView` — system info header
- `CpuSection`, `GpuSection`, `MemoryDiskSection`, `NetworkSection` — per-component panels
- `PlaceholderSection` — reserved slot
- `SectionVisibility` — toggle-state model for key-based section switching (1–5)
- `Window` — main window with dynamic grid layout, section visibility toggling, and `RefreshData` dispatch

---

## CLI Usage (building from terminal)

```bash
dotnet run --project ZenMonitor -- -r cli         # Run CLI mode
dotnet run --project ZenMonitor -- -r cli -l d    # Debug logging
dotnet run --project ZenMonitor -- -r cli -d 500  # 500ms update interval
dotnet run --project ZenMonitor -- -r cli -n      # Skip root check
dotnet run --project ZenMonitor -- -r cli -n -f   # Force launch no matter what
```

<!-- YES fucking EM-dashes oil me with them up -->

Options:
- `-r|--run <cli|tui|gui>` — required, selects frontend mode
- `-d|--delay <ms>` — update interval, 100–10000ms, default 1000
- `-n|--no-sudo <bool>` — bypass privilege check
- `-f|--force-run <bool>` — run regardless of unsupported OS (may break)
- `-c|--cli-log <bool>` — enable console log output (cli mode only)
- `-l|--log-level <level>` — `t|trace`, `d|debug`, `i|info`, `w|warning`, `e|error`, `c|critical`

Logs are written to `logs/ZenMonitor.log` (cleared on each run).

---

## Technical Details

- **Stack**: C# 100%, .NET 10.0.300
- **Key dependencies**: `ZenMonitor.Core` (hardware abstraction), `Spectre.Console.Cli` (CLI parsing), `Serilog` (logging), `Terminal.Gui` (TUI), `Microsoft.Extensions.DependencyInjection`
- **Platform**: Linux only (Windows support planned)
- **License**: LGPL-3.0

---

> [!NOTE]
> Documentation if you need more help: [DeepWiki/Akeoott/ZenMonitor](https://deepwiki.com/Akeoott/ZenMonitor)<br>
> I wanna mention that it provides a broad overview.<br>
> YOU CAN NOT 100% rely on it. You know how AI is, its not a permanent solution. Just a temporary fix.