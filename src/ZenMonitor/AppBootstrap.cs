// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using Spectre.Console.Cli;

namespace ZenMonitor;

/// <summary>
/// Bootstrap entry point for any ZenMonitor platform application.
/// Platform-specific hosts (Desktop, Mobile, etc.) should set <see cref="PlatformStartup"/>
/// and then call <see cref="RunAsync"/>.
/// </summary>
public static class AppBootstrap
{
    /// <summary>
    /// Delegate invoked by the shared CLI command to start the platform-specific UI.
    /// </summary>
    public static Action? PlatformStartup { get; set; }

    /// <summary>
    /// Runs the shared CLI, which handles logging configuration, DI setup,
    /// privilege checks, and eventually calls <see cref="PlatformStartup"/>.
    /// </summary>
    public static async Task<int> RunAsync(string[] args)
    {
        var app = new CommandApp<Initialize>();
        return await app.RunAsync(args);
    }
}
