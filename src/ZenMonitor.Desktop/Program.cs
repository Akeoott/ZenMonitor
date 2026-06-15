// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using Avalonia;

using Microsoft.Extensions.DependencyInjection;

using ZenMonitor.Desktop.ViewModels;

namespace ZenMonitor.Desktop;

internal static class Program
{
    public static async Task<int> Main(string[] args)
    {
        AppBootstrap.ConfigureServices = services =>
        {
            services.AddTransient<ProcessesViewModel>();
            services.AddTransient<PerformanceViewModel>();
            services.AddTransient<ControllerViewModel>();
            services.AddTransient<SettingsViewModel>();
            services.AddTransient<MainWindowModel>();
        };

        AppBootstrap.PlatformStartup = () =>
        {
            BuildAvaloniaApp().StartWithClassicDesktopLifetime([]);
        };

        return await AppBootstrap.RunAsync(args);
    }

    private static AppBuilder BuildAvaloniaApp()
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
