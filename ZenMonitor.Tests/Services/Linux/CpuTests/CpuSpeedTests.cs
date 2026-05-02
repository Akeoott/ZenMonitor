// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using System.IO.Abstractions.TestingHelpers;
using System.Runtime.Versioning;

using Microsoft.Extensions.Logging;

using Moq;

using ZenMonitor.Core.Interfaces;
using ZenMonitor.Core.Services.Linux;

namespace ZenMonitor.Tests.Services.Linux.CpuTests;

[SupportedOSPlatform("linux")]
public class CpuSpeedTests
{
    private readonly Mock<ILogger<Cpu>> _mockLogger;
    private readonly MockFileSystem _mockFileSystem;
    private readonly Mock<ITimeService> _timeService;

    public CpuSpeedTests()
    {
        _mockLogger = new Mock<ILogger<Cpu>>();
        _mockFileSystem = new MockFileSystem();
        _timeService = new Mock<ITimeService>();
    }

    [Fact]
    public void GetCpuSpeed_ReturnsCpuSpeed()
    {
        // Arrange
        string cpuinfo = TestData.CpuInfo();
        _mockFileSystem.AddFile("/proc/cpuinfo", new MockFileData(cpuinfo));

        // Act
        string stat1 = TestData.Stat1();
        _mockFileSystem.AddFile("/proc/stat", new MockFileData(stat1));
        var cpu = new Cpu(_mockLogger.Object, _mockFileSystem, _timeService.Object);

        cpu.Update();

        string stat2 = TestData.Stat2();
        _mockFileSystem.AddFile("/proc/stat", new MockFileData(stat2));

        cpu.Update();

        // Assert
        Assert.Equal(4, cpu.GetCpuSpeed());
    }
}
