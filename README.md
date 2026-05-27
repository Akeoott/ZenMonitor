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

> [!NOTE]
> I'm currently focusing on [ZenMonitor.Core](https://github.com/Akeoott/ZenMonitor.Core),<br>
> the backend of ZenMonitor.Core which I originally made in this repository.<br>
> For this reason is activity from my end limited in this repository.

## Quick Start

**Prerequisites:** [.NET SDK 10.0.203](https://dotnet.microsoft.com/en-us/download/dotnet/10.0) (the exact version is pinned in [`global.json`](https://github.com/Akeoott/ZenMonitor/blob/main/global.json)).<br>
**Platform:** Depends on [ZenMonitor.Core](https://github.com/Akeoott/ZenMonitor.Core) (see [Architecture](#architecture) below).

```bash
git clone https://github.com/Akeoott/ZenMonitor
cd ZenMonitor

dotnet restore
dotnet build

# Run the TUI frontend
# (requires sudo/admin privileges, to bypass add `-n` after `-r tui`)
dotnet run --project ZenMonitor -- -r tui
```

> See [CONTRIBUTING.md](https://github.com/Akeoott/ZenMonitor/blob/main/.github/CONTRIBUTING.md) for the full contribution workflow and our [Code of Conduct](https://github.com/Akeoott/ZenMonitor/blob/main/.github/CODE_OF_CONDUCT.md).

---

## Project Summary

ZenMonitor is a modern system monitor (planned task manager) built on .NET 10.0 (C# only). It uses a modular, interface-driven backend for system telemetry and a Producer-Consumer pattern to decouple data collection from rendering.

Currently Linux-only; GUI frontend is planned but not yet implemented.<br>
See [ZenMonitor.Core](https://github.com/Akeoott/ZenMonitor.Core) for more information about the backend.

---

## CLI Usage (building from terminal)

```bash
dotnet run --project ZenMonitor -- -r tui         # Run TUI mode
dotnet run --project ZenMonitor -- -r tui -l d    # Debug logging
dotnet run --project ZenMonitor -- -r tui -d 500  # 500ms update interval
dotnet run --project ZenMonitor -- -r tui -n      # Skip root check
dotnet run --project ZenMonitor -- -r tui -n -f   # Force launch no matter what
```

<!-- YES fucking EM-dashes oil me with them up -->

Options:
- `-r|--run <tui|gui>` — required, selects frontend mode
- `-d|--delay <ms>` — update interval, 100–10000ms, default 1000
- `-n|--no-sudo <bool>` — bypass privilege check
- `-f|--force-run <bool>` — run regardless of unsupported OS (may break)
- `-l|--log-level <level>` — `t|trace`, `d|debug`, `i|info`, `w|warning`, `e|error`, `c|critical`

Logs are written to `logs/ZenMonitor.log` (cleared on each run).

---

## Technical Details

- **Stack**: C# 100%, .NET 10.0.203
- **Key dependencies**: `ZenMonitor.Core` (hardware abstraction), `Spectre.Console.Cli` (CLI parsing), `Serilog` (logging), `Terminal.Gui` (TUI), `Microsoft.Extensions.DependencyInjection`
- **Platform**: Linux only (Windows support planned)
- **License**: LGPL-3.0

---

> [!NOTE]
> Documentation if you need more help: [DeepWiki/Akeoott/ZenMonitor](https://deepwiki.com/Akeoott/ZenMonitor)<br>
> I wanna mention that it provides a broad overview.<br>
> YOU CAN NOT 100% rely on it. You know how AI is, its not a permanent solution. Just a temporary fix.<br>
> In case of questions, open a [discussion](https://github.com/Akeoott/ZenMonitor/discussions/categories/q-a) on github.