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
        <img src="https://deepwiki.com/badge.svg" alt="Ask DeepWiki">
    </a>
</div>
<br>

> [!WARNING]
> WIP, limited functionality.<br>
> Backend functional with limitations.<br>
> No Tui or Gui available at the moment.

<br>

## Project Summary

ZenMonitor is a modern task manager built on .NET 10.0 (C# only). It uses a modular, interface-driven backend for system telemetry and a Producer-Consumer pattern to decouple data collection from rendering. Currently Linux-only; TUI and GUI frontend's are planned but not yet implemented.

---

### Key Concepts

- **Producer-Consumer pattern**: A background thread calls `Update()` on each hardware service at a configurable interval, then signals a `SemaphoreSlim`. The frontend waits on the semaphore and reads the latest snapshot data.

- **Dependency Injection**: All hardware services are registered as singletons via `Microsoft.Extensions.DependencyInjection`. The frontend's (Cli/Tui/Gui) are registered as transient.

- **Root required**: The app checks for root (Linux) or admin (Windows) privileges at startup unless `--no-sudo` is passed.

---

### CLI Usage (building from terminal)

```bash
dotnet run --project ZenMonitor -- -r cli        # Run CLI mode (only working mode)
dotnet run --project ZenMonitor -- -r cli -l d   # Debug logging
dotnet run --project ZenMonitor -- -r cli -l t   # Trace logging
dotnet run --project ZenMonitor -- -r cli -d 500 # 500ms update interval
dotnet run --project ZenMonitor -- -r cli -n     # Skip root check
```

<!-- YES fucking EM-Dashes -->

Options:
- `-r|--run <cli|tui|gui>` — required, selects frontend mode
- `-d|--delay <ms>` — update interval, 100–10000ms, default 1000
- `-n|--no-sudo <bool>` — bypass privilege check
- `-c|--cli-log <bool>` — enable console log output (cli mode only)
- `-l|--log-level <level>` — `t|trace`, `d|debug`, `i|info`, `w|warning`, `e|error`, `c|critical`

Logs are written to `logs/ZenMonitor.log` (cleared on each run).

---

### Project Structure

- **ZenMonitor/**
  - `./Program.cs`: Entry point.<br>
  Contains `MonitorCommand` (Spectre.Console.Cli `AsyncCommand`), `MonitorSettings` (CLI argument definitions), DI wiring, logging setup via Serilog, and mode dispatch (`cli`/`tui`/`gui`).
- **ZenMonitor.Core/**
  - `./Interfaces/`: Hardware abstraction interfaces. Each has a `void Update()` method plus typed getters.
  - `./Models/`: Immutable C# `record` types used as data snapshots.
  - `./Services/Linux/`: Concrete Linux implementations of all interfaces.
  - `./Services/Windows/`: Concrete **future** Windows implementations of all interfaces.
- **ZenMonitor.Cli/**
  - `./Monitor.cs`: The only working frontend at the moment. Injects all hardware interfaces, runs the backend thread, and prints raw telemetry values to stdout in a loop.
- **ZenMonitor.Tui/**
  - `./`: Planned TUI frontend (not implemented).
- **ZenMonitor.Gui/**
  - `./`: Planned GUI frontend (not implemented).
- **ZenMonitor.Tests/**
  - `./Services/Linux/`: xUnit test suite.<br>
  Uses `System.IO.Abstractions.MockFileSystem` to test Linux service parsing logic without real hardware.

---

### Technical Details

- **Stack**: C# 100%, .NET 10.0.203
- **Key dependencies**: `Spectre.Console.Cli` (CLI parsing), `Serilog` (logging), `Microsoft.Extensions.DependencyInjection`, `System.IO.Abstractions` (testability)
- **Test framework**: xUnit + `coverlet` (coverage config in `coverlet.runsettings`)
- **Platform**: Linux only (Windows support planned)
- **License**: LGPL-3.0

---

> [!NOTE]
> Documentation: [DeepWiki/Akeoott/ZenMonitor](https://deepwiki.com/Akeoott/ZenMonitor)<br>
> I wanna mention that it provides a broad overview.<br>
> YOU CAN NOT 100% rely on it. You know how AI is, its not a permanent solution. Just a temporary fix.
