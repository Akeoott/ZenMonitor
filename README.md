![ZenMonitor](https://raw.githubusercontent.com/Akeoott/ZenMonitor/30e1d65adfb0b51ebb56e1745394234888284dd3/assets/images/ZenMonitor.svg)

### Powered by ZenMonitor.Core system telemetry

[![CodeFactor Grade](https://img.shields.io/codefactor/grade/github/Akeoott/ZenMonitor?style=for-the-badge&logoSize=auto&labelColor=%231a1b1f)](https://www.codefactor.io/repository/github/akeoott/zenmonitor)
[![Code Coverage](https://img.shields.io/codecov/c/github/Akeoott/ZenMonitor?style=for-the-badge&logoSize=auto&labelColor=%231d1b16)](https://codecov.io/gh/Akeoott/ZenMonitor)
[![Nuget Version](https://img.shields.io/nuget/vpre/ZenMonitor.Core?style=for-the-badge&logo=nuget&label=ZenMonitor.Core&labelColor=%231a1b1f&color=%23a8c7ff)](https://www.nuget.org/packages/ZenMonitor.Core/)

### A system monitor built using C#, Rust (Tauri) and Vue

> [!NOTE]
> After a lot of time of testing things, struggling and just being burnt out,
> I have FINALLY found a solution.
> Tauri + Vue interacting with a C# API on a different process.
> Not special I know but it works and thats what counts.

---

## Overview

ZenMonitor is a modern system monitor (with planned task manager capabilities) built using C# and Tauri + Vue.
The backend follows a modular, interface-driven architecture for system telemetry and decouples data collection from rendering
using a Producer-Consumer pattern.

**Features:**

- Real-time CPU, memory, disk, network, GPU, system information and more!
- Modular backend via [ZenMonitor.Core](https://github.com/Akeoott/ZenMonitor.Core) – hardware abstraction with
  platform-specific implementations
- Producer-Consumer pattern: data collection runs independently of the UI

The backend is fully functional on Linux.
Windows support is planned but not yet implemented in the Core library.

Tauri, aka the app itself runs on Linux and Windows alike.

---

## Quick Start

### Running the project

```bash
git clone https://github.com/Akeoott/ZenMonitor
cd ZenMonitor

# Build the API
dotnet restore
dotnet build

# Then run Tauri
cd src/ZenMonitor.App/
npm run tauri dev
```

> [!IMPORTANT]
> See [CONTRIBUTING.md](https://github.com/Akeoott/ZenMonitor/blob/main/.github/CONTRIBUTING.md)
> for the contribution workflow and
> our [Code of Conduct](https://github.com/Akeoott/ZenMonitor/blob/main/.github/CODE_OF_CONDUCT.md).

### Technical Details

- **Stack**: C# (API), Rust (Tauri), Vue + TypeScript (Frontend)
- **Key dependencies**:
    - [`ZenMonitor.Core`](https://github.com/Akeoott/ZenMonitor.Core) – Backend Library
    - `Tauri` – Desktop Application
    - `Vue + TypeScript + Tailwind` – App styling and behavior
- **Platform**: Linux (support depends on `ZenMonitor.Core`)
- **License**: [LGPL-3.0](LICENSE)

---

## Current Status

- **Backend**: Functional on Linux. Windows support is work in progress in the Core library.
- **GUI Frontend**: Currently WIP. Being rebuilt.

---

## Contributing

Please read [CONTRIBUTING.md](https://github.com/Akeoott/ZenMonitor/blob/main/.github/CONTRIBUTING.md) for guidelines on
code style, commit conventions, and pull requests.
