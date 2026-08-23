// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using System.Threading.Tasks;

using Microsoft.AspNetCore.SignalR;

using ZenMonitor.UserConfig;

namespace ZenMonitor.Hubs;

public class ConfigHub(IConfigService configService) : Hub
{
    public void UpdateConfig(ConfigModel newConfig) => configService.UpdateConfig(newConfig);
    public ConfigModel GetConfig() => configService.Current;

    public Task LoadConfig() => configService.LoadAsync();
    public Task SaveConfig() => configService.SaveAsync();
}
