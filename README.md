![ZenMonitor](https://raw.githubusercontent.com/Akeoott/ZenMonitor/30e1d65adfb0b51ebb56e1745394234888284dd3/assets/images/ZenMonitor.svg)

### Powered by ZenMonitor.Core system telemetry

[![CodeFactor Grade](https://img.shields.io/codefactor/grade/github/Akeoott/ZenMonitor?style=for-the-badge&logoSize=auto&labelColor=%231a1b1f)](https://www.codefactor.io/repository/github/akeoott/zenmonitor)
[![Code Coverage](https://img.shields.io/codecov/c/github/Akeoott/ZenMonitor?style=for-the-badge&logoSize=auto&labelColor=%231d1b16)](https://codecov.io/gh/Akeoott/ZenMonitor)
[![Nuget Version](https://img.shields.io/nuget/vpre/ZenMonitor.Core?style=for-the-badge&logo=nuget&label=ZenMonitor.Core&labelColor=%231a1b1f&color=%23a8c7ff)](https://www.nuget.org/packages/ZenMonitor.Core/)

### A system monitor made to be super tiny and fast

> [!NOTE]
> After a lot of time of testing things, struggling and just being burnt out,
> I have FINALLY found a solution.
> Tauri + Vue interacting with a C# API on a different process.
> Not special I know, but it works and that's what counts.

---

## Overview

ZenMonitor is a modern system monitor (with planned task manager capabilities) built using C#, Rust (Tauri), and Vue.
The architecture is designed around a modular, interface-driven backend for system telemetry, decoupled from the UI via a Producer‑Consumer pattern.

**Planned Features:**
- System information including real time cpu usage, processes, storage and much more.
- System management like starting, killing and setting priorities for processes.
- Modular backend via [ZenMonitor.Core](https://github.com/Akeoott/ZenMonitor.Core) – hardware abstraction with platform‑specific implementations.

**Current Development Status:**
- The C# backend integration is **not yet implemented** – the dotnet process currently not doing anything.
- The Vue frontend is **under active rebuild** – it's minimal and serves as a placeholder for the eventual UI.
- The **build pipeline** and **cross‑platform packaging** (Tauri + sidecar) are **fully functional** and production‑ready.

**Frontend Status**
- Currently just something thrown together for a base.
- Does not provide any useful function at the moment.
- Will most probably be completely rewritten from how it currently looks.

The app is being developed primarily for Linux; Windows support will follow once the core library implements the required platform backends.

---

## Quick Start

```bash
git clone https://github.com/Akeoott/ZenMonitor
cd ./ZenMonitor/src/
```

### Running the project:

- **Options**
  - `npm run [command]` - Executes one of the scripts in package.json


- **Commands**:
  - `dev` – runs the frontend server
  - `dotnet` – runs the dotnet server
  - `build` – builds the frontend for production
  - `tauri:[platform] dev` – runs Tauri in dev mode with sidecar
  - `tauri:[platform] build` – builds the final bundled application


- **Platforms**:
  - `win` – Windows-x64
  - `linux` – Linux-x64
  - `mac` – MacOS-arm64


- **Example**:
  - `npm run dev` - Run the web server alone
  - `npm run tauri:win dev` - Run the tauri app for windows in dev mode
  - `npm run tauri:linux build` - Build the tauri app for linux

> [!IMPORTANT]
> See [CONTRIBUTING.md](https://github.com/Akeoott/ZenMonitor/blob/main/.github/CONTRIBUTING.md) for the contribution workflow
> and our [Code of Conduct](https://github.com/Akeoott/ZenMonitor/blob/main/.github/CODE_OF_CONDUCT.md).

### Technical Details

- **Stack**: C# (.NET 10.0 API), Rust (Tauri), Vue + TypeScript (Frontend)
- **Key dependencies**:
    - [`ZenMonitor.Core`](https://github.com/Akeoott/ZenMonitor.Core) – Backend Library (to be integrated)
    - `Tauri` – Desktop Application
    - `Vue + TypeScript + Tailwind` – App styling and behavior
- **Platform**: Linux (primary target), Windows (planned)
- **License**: [LGPL-3.0](LICENSE)

---

## Contributing

Please read [CONTRIBUTING.md](https://github.com/Akeoott/ZenMonitor/blob/main/.github/CONTRIBUTING.md) for guidelines on
code style, commit conventions, and pull requests.
.