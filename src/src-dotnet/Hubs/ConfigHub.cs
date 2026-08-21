// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using System.Threading.Tasks;

using Microsoft.AspNetCore.SignalR;

using ZenMonitor.UserConfig;

namespace ZenMonitor.Hubs;

public class ConfigHub(IConfigService configService) : Hub
{
    public Task<ConfigModel> GetConfig() => Task.FromResult(configService.Current);

    public async Task UpdateConfig(ConfigModel newConfig)
    {
        configService.UpdateConfig(newConfig);
        await Task.CompletedTask;
    }

    public async Task LoadConfig()
    {
        await configService.LoadAsync();
        await Task.CompletedTask;
    }

    public async Task SaveConfig()
    {
        await configService.SaveAsync();
        await Task.CompletedTask;
    }
}
