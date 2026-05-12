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

<br>

## Quick Start

**Prerequisites:** [.NET SDK 10.0.203](https://dotnet.microsoft.com/en-us/download/dotnet/10.0) (the exact version is pinned in [`global.json`](global.json)).<br>
**Platform:** Linux (primary), Windows (partial, WIP).

```bash
git clone https://github.com/Akeoott/ZenMonitor
cd ZenMonitor

dotnet restore
dotnet build

# Run the CLI frontend
# (requires sudo/admin privileges, to bypass add `-n` after `-r cli`)
dotnet run --project ZenMonitor -- -r cli
```

> See [CONTRIBUTING.md](.github/CONTRIBUTING.md) for the full contribution workflow and our [Code of Conduct](.github/CODE_OF_CONDUCT.md).

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

All telemetry data flows through interfaces in `ZenMonitor.Core/Interfaces/`. Each exposes a `void Update()` method plus typed getters:

| Interface | Provides |
|-----------|----------|
| `ICpu`    | CPU usage, temperature, frequency |
| `IDrive`  | Disk I/O, partition usage |
| `IGpu`    | GPU utilization, VRAM |
| `IHelper` | Utility / helper methods |
| `IMemory` | RAM usage, swap |
| `INetwork` | Network throughput, interfaces |
| `ISystem` | OS info, uptime, hostname |

Concrete implementations live in `ZenMonitor.Core/Services/<Platform>/`.

### Adding a New Hardware Metric or Platform

1. **Implement the interface** — create a new class in `Core/Services/<Platform>/` that implements the relevant `IXxx` interface.
2. **Register in DI** — add the service in `Program.cs` → `InitProgram` using the DI container.
3. **Add a frontend view** — consume the data from a `Cli`, `Tui`, or `Gui` view.

---

## CLI Usage (building from terminal)

```bash
dotnet run --project ZenMonitor -- -r cli         # Run CLI mode
dotnet run --project ZenMonitor -- -r cli -l d    # Debug logging
dotnet run --project ZenMonitor -- -r cli -d 500  # 500ms update interval
dotnet run --project ZenMonitor -- -r cli -n      # Skip root check
dotnet run --project ZenMonitor -- -r cli -n -f   # Force launch no matter what
```

<!-- YES fucking EM-Dashes oil me with them up -->

Options:
- `-r|--run <cli|tui|gui>` — required, selects frontend mode
- `-d|--delay <ms>` — update interval, 100–10000ms, default 1000
- `-n|--no-sudo <bool>` — bypass privilege check
- `-f|--force-run <bool>` — run regardless of unsupported OS (may break)
- `-c|--cli-log <bool>` — enable console log output (cli mode only)
- `-l|--log-level <level>` — `t|trace`, `d|debug`, `i|info`, `w|warning`, `e|error`, `c|critical`

Logs are written to `logs/ZenMonitor.log` (cleared on each run).

---

## Running Tests

The project uses **xUnit** with platform-filtered test categories that match the CI workflow ([`.github/workflows/tests.yml`](.github/workflows/tests.yml)).

```bash
# Run all tests
dotnet test

# Platform-specific runs
dotnet test --filter "Platform=Linux"
dotnet test --filter "Platform=Windows"

# With code coverage (local)
dotnet test --collect:"XPlat Code Coverage" --settings coverlet.runsettings
```

- Coverage configuration is in [`coverlet.runsettings`](coverlet.runsettings) at the repo root.
- Linux service tests use `System.IO.Abstractions.MockFileSystem` to parse `/proc` and `/sys` files without real hardware.
- Test data fixtures live in `ZenMonitor.Tests/Services/Linux/TestData/`.

---

## Project Structure

- **ZenMonitor/**
  - `./Program.cs`: Entry point.<br>
    Contains `InitProgram` (Spectre.Console.Cli `AsyncCommand<ProgramSettings>`), DI wiring, logging setup via Serilog, privilege check (Linux root / Windows admin), and mode dispatch (`cli`/`tui`/`gui`).
  - `./ProgramSettings.cs`: CLI argument definitions with Spectre.Console validation.
  - `./ProgramHelper.cs`: Shared helpers (logging setup, service provider building, runtime safety checks).
- **ZenMonitor.Core/**
  - `./Interfaces/`: Hardware abstraction interfaces. Each has a `void Update()` method plus typed getters.
  - `./Models/`: Immutable C# `record` types used as data snapshots.
  - `./Services/Linux/`: Concrete Linux implementations of all interfaces.
  - `./Services/Windows/`: Concrete **future** Windows implementations of all interfaces.
- **ZenMonitor.Cli/**
  - `./Monitor.cs`: CLI frontend. Injects all hardware interfaces, runs a background data loop and prints raw telemetry values to stdout.
- **ZenMonitor.Tui/**
  - `./Monitor.cs`: TUI frontend using **Terminal.Gui**. Injects all hardware interfaces, runs a background data loop, and updates views via `app.Invoke(window.RefreshData)`.
  - `./Views/`: TUI view components:
    - `HeaderView` — system info header
    - `CpuSection`, `GpuSection`, `MemoryDiskSection`, `NetworkSection` — per-component panels
    - `PlaceholderSection` — reserved slot
    - `SectionVisibility` — toggle-state model for key-based section switching (1–5)
    - `Window` — main window with dynamic grid layout, section visibility toggling, and `RefreshData` dispatch
- **ZenMonitor.Gui/**
  - `./`: Planned GUI frontend (not implemented).
- **ZenMonitor.Tests/**
  - `./Services/Linux/`: xUnit test suite.<br>
    Uses `System.IO.Abstractions.MockFileSystem` to test Linux service parsing logic without real hardware.

---

## Technical Details

- **Stack**: C# 100%, .NET 10.0.203
- **Key dependencies**: `Spectre.Console.Cli` (CLI parsing), `Serilog` (logging), `Microsoft.Extensions.DependencyInjection`, `System.IO.Abstractions` (testability)
- **Test framework**: xUnit + `coverlet` (coverage config in `coverlet.runsettings`)
- **Platform**: Linux only (Windows support planned)
- **License**: LGPL-3.0

---

> [!NOTE]
> Documentation if you need more help: [DeepWiki/Akeoott/ZenMonitor](https://deepwiki.com/Akeoott/ZenMonitor)<br>
> I wanna mention that it provides a broad overview.<br>
> YOU CAN NOT 100% rely on it. You know how AI is, its not a permanent solution. Just a temporary fix.