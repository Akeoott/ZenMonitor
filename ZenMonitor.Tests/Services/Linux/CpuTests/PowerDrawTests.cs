// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Runtime.Versioning;

using Microsoft.Extensions.Logging;

using Moq;

using ZenMonitor.Core.Interfaces;
using ZenMonitor.Core.Services.Linux;

namespace ZenMonitor.Tests.Services.Linux.CpuTests;

[SupportedOSPlatform("linux")]
public class PowerDrawTests
{
    private readonly Mock<ILogger<Cpu>> _mockLogger;
    private readonly MockFileSystem _mockFileSystem;
    private readonly Mock<ITimeService> _timeService;

    public PowerDrawTests()
    {
        _mockLogger = new Mock<ILogger<Cpu>>();
        _mockFileSystem = new MockFileSystem();
        _timeService = new Mock<ITimeService>();
    }

    [Fact]
    public void GetPowerDraw_ReturnsPowerDraw()
    {
        // Arrange
        const string energyUjPath = "/sys/class/powercap/intel-rapl:0/energy_uj";

        string cpuinfo = TestData.CpuInfo();
        _mockFileSystem.AddFile("/proc/cpuinfo", new MockFileData(cpuinfo));
        _mockFileSystem.AddFile("/proc/stat", new MockFileData(TestData.Stat1()));

        _timeService.Setup(c => c.UtcNow).Returns(new DateTime(2026, 1, 1, 0, 0, 1));
        string energyUj1 = TestData.EnergyUj1();
        _mockFileSystem.AddFile(energyUjPath, new MockFileData(energyUj1));
        var cpu = new Cpu(_mockLogger.Object, _mockFileSystem, _timeService.Object);
        // Act
        cpu.Update();

        // Arrange
        _timeService.Setup(c => c.UtcNow).Returns(new DateTime(2026, 1, 1, 0, 0, 6));
        string energyUj2 = TestData.EnergyUj2();
        _mockFileSystem.AddFile(energyUjPath, new MockFileData(energyUj2));

        // Act
        cpu.Update();

        // Assert
        Assert.Equal(46.03 /*as in 46.03w*/, cpu.GetPowerDraw()/*should return 46.03*/);
    }
}
