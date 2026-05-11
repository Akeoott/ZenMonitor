// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using Microsoft.Extensions.Logging;

using ZenMonitor.Core.Interfaces;
using ZenMonitor.Core.Models;

namespace ZenMonitor.Core.Services.Null;

public class Drive(ILogger<Drive> logger) : IDrive
{
    private readonly ILogger<Drive> _logger = logger;
    private readonly DriveInfoSnapshot _snapshot = new([]);

    public void Update() => _logger.LogWarning("Overriding platform specific code. Returning empty snapshot...");

    public DriveMountInfo[] GetMountInfos() => _snapshot.MountInfos;
}
