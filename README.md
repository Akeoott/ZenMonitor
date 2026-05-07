# ZenMonitor

### A light and fast system monitor


![Last Commit](https://img.shields.io/github/last-commit/Akeoott/ZenMonitor?style=for-the-badge&logoSize=auto&labelColor=%23201a19&color=%23ffb4a2)
![Stars](https://img.shields.io/github/stars/Akeoott/ZenMonitor?style=for-the-badge&labelColor=%231d1b16&color=%23e6c419)
![Repo Size](https://img.shields.io/github/repo-size/Akeoott/ZenMonitor?style=for-the-badge&labelColor=%231a1b1f&color=%23a8c7ff)<br>
[![GitHub License](https://img.shields.io/github/license/akeoott/ZenMonitor?style=for-the-badge&logoSize=auto&labelColor=%23201a19&color=%23ffb4a2)](https://github.com/Akeoott/ZenMonitor/blob/main/LICENSE)
[![Code Coverage](https://img.shields.io/codecov/c/github/Akeoott/ZenMonitor?style=for-the-badge&logoSize=auto&labelColor=%231d1b16)](https://codecov.io/gh/Akeoott/ZenMonitor)
[![CodeFactor Grade](https://img.shields.io/codefactor/grade/github/Akeoott/ZenMonitor?style=for-the-badge&logoSize=auto&labelColor=%231a1b1f)](https://www.codefactor.io/repository/github/akeoott/zenmonitor)

### ZenMonitor aims to be a fast and modern Task manager, providing you with a tone of details.

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

---

### Summary of the codebase

ZenMonitor runs on dotnet 10.0.203. It has a number of sub projects for tests, interfaces and the raw backend code.

#### Structure:

* `/` Project root and C# projects:
  * `/ZenMonitor` is the entry point of the application. It injects the other projects via dependency injection.
  * `/ZenMonitor.Core` is a class library containing all the code for gathering system telemetry, nothing else.
  * `/ZenMonitor.Cli` is a form of debug interface. It displays the raw gathered values for checking if all works.
  * `/ZenMonitor.Tui` will be a Terminal User Interface, comparable to monitors like btop.
  * `/ZenMonitor.Gui` will be a Graphical User Interface, comparable to monitors like System Monitor.
  * `/ZenMonitor.Tests` contains all unit tests which have to pass before merging a pull request (or if pushing directly via a bypass).

* `/.github`:
  * Contains workflows, some documentation, issue templates and other configurations for github.
  * Important files are `CODE_OF_CONDUCT.md`, `CONTRIBUTING.md` and `SECURITY.md`. Their purpose is to inform you about what our standards are. Please read them!
  * `settings.yml` contains some of our repository settings in code format.

---

> [!NOTE]
> Documentation: [DeepWiki/Akeoott/ZenMonitor](https://deepwiki.com/Akeoott/ZenMonitor)<br>
> I wanna mention that it provides a broad overview.<br>
> YOU CAN NOT 100% rely on it. You know how AI is, its not a permanent solution. Just a temporary fix.
