// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace ZenMonitor.UserConfig;

public interface IConfigService
{
    event EventHandler<ConfigModel> ConfigChanged;

    ConfigModel Current { get; }
    void UpdateConfig(ConfigModel newConfigModel);
    Task LoadAsync(CancellationToken cancellationToken = default);
    Task SaveAsync(CancellationToken cancellationToken = default);
}
