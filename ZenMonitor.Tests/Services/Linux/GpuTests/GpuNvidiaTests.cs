// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using System.Runtime.Versioning;

using Microsoft.Extensions.Logging;

using Moq;

using ZenMonitor.Core.Interfaces;
using ZenMonitor.Core.Models;
using ZenMonitor.Core.Services.Linux;

namespace ZenMonitor.Tests.Services.Linux.GpuTests;

[SupportedOSPlatform("linux")]
public class GpuNvidiaTests
{
    private readonly Mock<ILogger<GpuNvidia>> _mockLogger;
    private readonly Mock<IHelper> _mockProcessRunner;

    public GpuNvidiaTests()
    {
        _mockLogger = new Mock<ILogger<GpuNvidia>>();
        _mockProcessRunner = new Mock<IHelper>();
    }

    [Fact]
    public void Update_ReturnGpuInfoFromGpuNvidia()
    {
        // Arrange
        string output = "GeForce RTX 4090, 12, 6, 1024, 24576, 72, P0, 450.00";
        _mockProcessRunner
            .Setup(r => r.RunProcess("nvidia-smi", It.IsAny<string>()))
            .Returns(new ProcessResult(0, output, string.Empty));

        var gpu = new GpuNvidia(_mockLogger.Object, _mockProcessRunner.Object);

        // Act
        gpu.Update();

        // Assert
        Assert.Equal("GeForce RTX 4090", gpu.GetGpuName());
        Assert.Equal("12", gpu.GetUsageGpu());
        Assert.Equal("6", gpu.GetUsageMemory());
        Assert.Equal("1024", gpu.GetMemoryUsed());
        Assert.Equal("24576", gpu.GetMemoryTotal());
        Assert.Equal("72", gpu.GetTemperatureGpu());
        Assert.Equal("P0", gpu.GetPowerState());
        Assert.Equal("450.00", gpu.GetPowerDraw());
    }

    [Fact]
    public void Update_ThrowsInvalidOperationExceptionWhenNvidiaSmiFails()
    {
        // Arrange
        _mockProcessRunner
            .Setup(r => r.RunProcess("nvidia-smi", It.IsAny<string>()))
            .Returns(new ProcessResult(1, string.Empty, "Failed to query GPU"));

        var gpu = new GpuNvidia(_mockLogger.Object, _mockProcessRunner.Object);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => gpu.Update());
        Assert.Contains("nvidia-smi error", exception.Message);
    }
}
