// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using Microsoft.Extensions.Logging;

using ZenMonitor.Core.Interfaces;

namespace ZenMonitor.Tui;

public class Monitor(
    ILogger<Monitor> logger,
    ICpu cpuInfo,
    IGpu gpuInfo,
    IMemory memoryInfo,
    INetwork networkInfo,
    IDrive driveInfo,
    ISystem systemInfo)
{
    private readonly ILogger<Monitor> _logger = logger;
    private readonly ICpu _cpuInfo = cpuInfo;
    private readonly IGpu _gpuInfo = gpuInfo;
    private readonly IMemory _memoryInfo = memoryInfo;
    private readonly INetwork _networkInfo = networkInfo;
    private readonly IDrive _driveInfo = driveInfo;
    private readonly ISystem _systemInfo = systemInfo;

    private readonly SemaphoreSlim _dataReadyEvent = new(0, int.MaxValue);

    public async Task InitMonitor(int loopDelay, CancellationToken cts)
    {

    }
}
