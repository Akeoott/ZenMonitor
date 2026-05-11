// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using Microsoft.Extensions.Logging;

using Moq;

using ZenMonitor.Core.Services.Null;

namespace ZenMonitor.Tests.Services.Null.Tests;

[Trait("Platform", "Linux")] // Only to define runner, does not reflect reality XP
public class GpuTests
{
    private readonly Mock<ILogger<Gpu>> _mockLogger;

    public GpuTests()
    {
        _mockLogger = new Mock<ILogger<Gpu>>();
    }

    private Gpu CreateGpu() => new(_mockLogger.Object);

    [Fact]
    public void GetAll_CheckThatEverythingIsNull()
    {
        var gpu = CreateGpu();
        gpu.Update();
        Assert.Equal("", gpu.GetGpuName());
        Assert.Equal(0, gpu.GetUsageGpu());
        Assert.Equal(0, gpu.GetUsageMemory());
        Assert.Equal(0, gpu.GetMemoryUsed());
        Assert.Equal(0, gpu.GetMemoryTotal());
        Assert.Equal(0, gpu.GetTemperatureGpu());
        Assert.Equal("", gpu.GetPowerState());
        Assert.Equal(0, gpu.GetPowerDraw());
    }
}
