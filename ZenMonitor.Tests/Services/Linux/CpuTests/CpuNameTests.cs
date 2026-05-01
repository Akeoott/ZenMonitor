// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using System.IO.Abstractions.TestingHelpers;
using System.Runtime.Versioning;

using Microsoft.Extensions.Logging;

using Moq;

using ZenMonitor.Core.Services.Linux;

namespace ZenMonitor.Tests.Services.Linux.CpuTests;

[SupportedOSPlatform("linux")]
public class CpuNameTests
{
    private readonly Mock<ILogger<Cpu>> _mockLogger;
    private readonly MockFileSystem _mockFileSystem;

    public CpuNameTests()
    {
        _mockLogger = new Mock<ILogger<Cpu>>();
        _mockFileSystem = new MockFileSystem();
    }

    [Fact]
    public void GetCpuName_ReturnsModelNameFromCpuinfo()
    {
        // Arrange
        string cpuinfo = TestData.CpuInfo();
        string stat = TestData.Stat();
        _mockFileSystem.AddFile("/proc/cpuinfo", new MockFileData(cpuinfo));

        // A minimal /proc/stat is required so ReadCpuUsages doesn't throw
        _mockFileSystem.AddFile("/proc/stat", new MockFileData(stat));

        var cpu = new Cpu(_mockLogger.Object, _mockFileSystem);

        // Act
        cpu.Update();

        // Assert
        Assert.Equal("AMD Ryzen 7 7800X3D 8-Core Processor", cpu.GetCpuName());
    }
}
