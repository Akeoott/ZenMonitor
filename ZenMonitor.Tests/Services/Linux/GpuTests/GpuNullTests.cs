// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using System.Runtime.Versioning;

using Microsoft.Extensions.Logging;

using Moq;

using ZenMonitor.Core.Interfaces;
using ZenMonitor.Core.Services.Linux;

namespace ZenMonitor.Tests.Services.Linux.GpuTests;

[SupportedOSPlatform("linux")]
public class GpuNullTests
{
    private readonly Mock<ILogger<GpuNull>> _mockLogger;

    public GpuNullTests()
    {
        _mockLogger = new Mock<ILogger<GpuNull>>();
    }

    [Fact]
    public void Update_ReturnEmpryStringsFromGpuNull()
    {
        // Arrange
        var gpu = new GpuNull(_mockLogger.Object);

        // Act
        gpu.Update();

        // Assert
        Assert.Equal("", gpu.GetGpuName());
        Assert.Equal(0, gpu.GetUsageGpu());
        Assert.Equal(0, gpu.GetUsageMemory());
        Assert.Equal(0.0, gpu.GetMemoryUsed());
        Assert.Equal(0.0, gpu.GetMemoryTotal());
        Assert.Equal(0, gpu.GetTemperatureGpu());
        Assert.Equal("", gpu.GetPowerState());
        Assert.Equal(0.0, gpu.GetPowerDraw());
    }
}
