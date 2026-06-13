// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using Avalonia;

namespace ZenMonitor.Desktop;

internal static class Program
{
    public static async Task<int> Main(string[] args)
    {
        AppBootstrap.PlatformStartup = () =>
        {
            BuildAvaloniaApp().StartWithClassicDesktopLifetime([]);
        };

        return await AppBootstrap.RunAsync(args);

        static AppBuilder BuildAvaloniaApp()
        {
            return AppBuilder.Configure<App>()
                .UsePlatformDetect()
#if DEBUG
                .WithDeveloperTools()
#endif
                .WithInterFont()
                .LogToTrace();
        }
    }
}
