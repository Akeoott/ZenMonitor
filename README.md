![ZenMonitor](https://raw.githubusercontent.com/Akeoott/ZenMonitor/30e1d65adfb0b51ebb56e1745394234888284dd3/assets/images/ZenMonitor.svg)

### Powered by ZenMonitor.Core system telemetry

[![CodeFactor Grade](https://img.shields.io/codefactor/grade/github/Akeoott/ZenMonitor?style=for-the-badge&logoSize=auto&labelColor=%231a1b1f)](https://www.codefactor.io/repository/github/akeoott/zenmonitor)
[![Code Coverage](https://img.shields.io/codecov/c/github/Akeoott/ZenMonitor?style=for-the-badge&logoSize=auto&labelColor=%231d1b16)](https://codecov.io/gh/Akeoott/ZenMonitor)
[![Nuget Version](https://img.shields.io/nuget/vpre/ZenMonitor.Core?style=for-the-badge&logo=nuget&label=ZenMonitor.Core&labelColor=%231a1b1f&color=%23a8c7ff)](https://www.nuget.org/packages/ZenMonitor.Core/)

### A light and fast system monitor

> [!WARNING]
> This project is work in progress. Limited functionality available.<br>
> The backend is functional with some limitations.<br>
> GUI is work in progress.

> [!NOTE]
> Development focus is currently on [ZenMonitor.Core](https://github.com/Akeoott/ZenMonitor.Core), the backend library
> extracted from this repository.
> Activity on this frontend repository is therefore limited.

---

## Overview

ZenMonitor is a modern system monitor (with planned task manager capabilities) built on .NET 10.0 using C#.
It follows a modular, interface-driven architecture for system telemetry and decouples data collection from rendering
using a Producer-Consumer pattern.

**Features:**

- Real-time CPU, memory, disk, network, GPU, system information and more!
- Modular backend via [ZenMonitor.Core](https://github.com/Akeoott/ZenMonitor.Core) – hardware abstraction with
  platform-specific implementations
- Producer-Consumer pattern: data collection runs independently of the UI
- Currently, Linux-only with an Avalonia UI which is WIP

The backend is fully functional on Linux.
Windows support is planned but not yet implemented in the Core library.

---

## Quick Start

### Running the program

```bash
git clone https://github.com/Akeoott/ZenMonitor
cd ZenMonitor

dotnet restore
dotnet build

# Run the project
# (requires sudo/admin privileges. Use -f, --force to bypass)
dotnet run --project src/ZenMonitor.Desktop/ -- -f
```

> [!IMPORTANT]
> See [CONTRIBUTING.md](https://github.com/Akeoott/ZenMonitor/blob/main/.github/CONTRIBUTING.md)
> for the contribution workflow and
> our [Code of Conduct](https://github.com/Akeoott/ZenMonitor/blob/main/.github/CODE_OF_CONDUCT.md).

### Technical Details

- **Stack**: C# 100%, .NET 10.0.203
- **Key dependencies**:
    - [`ZenMonitor.Core`](https://github.com/Akeoott/ZenMonitor.Core) – hardware telemetry backend
    - `Spectre.Console` – console rendering
    - `Serilog` – structured logging
    - `Avalonia` – creating a cross-platform desktop app
    - `SukiUI` – styling Avalonia
    - `Microsoft.Extensions.DependencyInjection` – DI container
- **Platform**: Linux (support depends on `ZenMonitor.Core`)
- **License**: [LGPL-3.0](LICENSE)

---

## Architecture

ZenMonitor is split into two main parts:

1. **Backend** – [`ZenMonitor.Core`](https://github.com/Akeoott/ZenMonitor.Core) provides hardware abstraction
   interfaces and platform-specific implementations (Linux currently, Windows WIP).
   Data collection is triggered by a timer that calls `Update()` on all monitors, producing snapshots.

2. **Frontend** – This repository contains the user-facing application. It subscribes to the backend's data stream
   (Producer-Consumer pattern) and renders the information. The GUI is currently work in progress.

The Producer-Consumer pattern ensures that UI rendering never blocks data collection. The frontend consumes the latest
snapshot at its own refresh rate, independent of the collection interval.

---

## CLI Usage

Run the GUI frontend with various options:

```bash
dotnet run --project src/ZenMonitor.Desktop/           # Run Avalonia App
dotnet run --project src/ZenMonitor.Desktop/ -- -v d   # Debug logging
dotnet run --project src/ZenMonitor.Desktop/ -- -d 500 # 500ms update interval
dotnet run --project src/ZenMonitor.Desktop/ -- -f     # Skip root check
dotnet run --project src/ZenMonitor.Desktop/ -- -q     # Suppress console logging
```

### Options

| Option                    | Description                                                                                  |
|---------------------------|----------------------------------------------------------------------------------------------|
| `-d, --delay <ms>`        | Change the delay before updating, min to max is 100ms to 10000ms                             |
| `-f, --force <bool>`      | Run ZenMonitor without sudo/admin privileges (some things might not work!)                   |
| `-v, --verbosity <level>` | Set logging verbosity: `t/trace`, `d/debug`, `i/info`, `w/warning`, `e/error`, `c/critical`. |
| `-q, --quiet <bool>`      | Suppress console log output.                                                                 |

Logs are written to `logs/ZenMonitor.log`. The log file is cleared on each run.

---

## Current Status

- **Backend**: Functional on Linux. Windows support is work in progress in the Core library.
- **GUI Frontend**: Currently WIP. Fundamentals are there.

---

## Contributing

Please read [CONTRIBUTING.md](https://github.com/Akeoott/ZenMonitor/blob/main/.github/CONTRIBUTING.md) for guidelines on
code style, commit conventions, and pull requests.
