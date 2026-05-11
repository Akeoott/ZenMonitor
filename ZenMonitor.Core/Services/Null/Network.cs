// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using Microsoft.Extensions.Logging;

using ZenMonitor.Core.Interfaces;
using ZenMonitor.Core.Models;

namespace ZenMonitor.Core.Services.Null;

public class Network(ILogger<Network> logger) : INetwork
{
    private readonly ILogger<Network> _logger = logger;
    private readonly NetworkInfoSnapshot _snapshot = new("");

    public void Update() => _logger.LogWarning("Overriding platform specific code. Returning empty snapshot...");

    public string GetNone() => _snapshot.None;
}
