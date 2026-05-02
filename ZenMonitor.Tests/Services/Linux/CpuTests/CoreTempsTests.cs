// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using System.IO.Abstractions.TestingHelpers;
using System.Runtime.Versioning;

using Microsoft.Extensions.Logging;

using Moq;

using ZenMonitor.Core.Interfaces;
using ZenMonitor.Core.Models;
using ZenMonitor.Core.Services.Linux;

namespace ZenMonitor.Tests.Services.Linux.CpuTests;

[SupportedOSPlatform("linux")]
public class CoreTempsTests
{
    private readonly Mock<ILogger<Cpu>> _mockLogger;
    private readonly MockFileSystem _mockFileSystem;
    private readonly Mock<ITimeService> _timeService;

    public CoreTempsTests()
    {
        _mockLogger = new Mock<ILogger<Cpu>>();
        _mockFileSystem = new MockFileSystem();
        _timeService = new Mock<ITimeService>();
    }

    [Fact]
    public void GetCoreTemps_ReturnsIntelCoreTemps()
    {
        // Arrange
        _mockFileSystem.AddFile("/proc/cpuinfo", new MockFileData(TestData.CpuInfo2Core()));
        _mockFileSystem.AddFile("/proc/stat", new MockFileData(TestData.Stat1()));
        _mockFileSystem.AddFile("/sys/class/hwmon/hwmon0/name", new MockFileData(TestData.HwmonIntelName()));
        _mockFileSystem.AddFile("/sys/class/hwmon/hwmon0/temp1_input", new MockFileData(TestData.HwmonIntelTemp1Input()));
        _mockFileSystem.AddFile("/sys/class/hwmon/hwmon0/temp1_label", new MockFileData(TestData.HwmonIntelTemp1Label()));
        _mockFileSystem.AddFile("/sys/class/hwmon/hwmon0/temp2_input", new MockFileData(TestData.HwmonIntelTemp2Input()));
        _mockFileSystem.AddFile("/sys/class/hwmon/hwmon0/temp2_label", new MockFileData(TestData.HwmonIntelTemp2Label()));
        _mockFileSystem.AddFile("/sys/class/hwmon/hwmon0/temp3_input", new MockFileData(TestData.HwmonIntelTemp3Input()));
        _mockFileSystem.AddFile("/sys/class/hwmon/hwmon0/temp3_label", new MockFileData(TestData.HwmonIntelTemp3Label()));

        var cpu = new Cpu(_mockLogger.Object, _mockFileSystem, _timeService.Object);

        // Act
        cpu.Update();

        // Assert
        Assert.Equal(new[] { new CpuCoreTemp(0, 42), new CpuCoreTemp(1, 43) }, cpu.GetCoreTemps());
    }

    [Fact]
    public void GetCoreTemps_ReturnsAmdCcdTemps()
    {
        // Arrange
        _mockFileSystem.AddFile("/proc/cpuinfo", new MockFileData(TestData.CpuInfo2Core()));
        _mockFileSystem.AddFile("/proc/stat", new MockFileData(TestData.Stat1()));
        _mockFileSystem.AddFile("/sys/class/hwmon/hwmon0/name", new MockFileData(TestData.HwmonAmdName()));
        _mockFileSystem.AddFile("/sys/class/hwmon/hwmon0/temp1_input", new MockFileData(TestData.HwmonAmdTemp1Input()));
        _mockFileSystem.AddFile("/sys/class/hwmon/hwmon0/temp1_label", new MockFileData(TestData.HwmonAmdTemp1Label()));
        _mockFileSystem.AddFile("/sys/class/hwmon/hwmon0/temp2_input", new MockFileData(TestData.HwmonAmdTemp2Input()));
        _mockFileSystem.AddFile("/sys/class/hwmon/hwmon0/temp2_label", new MockFileData(TestData.HwmonAmdTemp2Label()));
        _mockFileSystem.AddFile("/sys/class/hwmon/hwmon0/temp3_input", new MockFileData(TestData.HwmonAmdTemp3Input()));
        _mockFileSystem.AddFile("/sys/class/hwmon/hwmon0/temp3_label", new MockFileData(TestData.HwmonAmdTemp3Label()));

        var cpu = new Cpu(_mockLogger.Object, _mockFileSystem, _timeService.Object);

        // Act
        cpu.Update();

        // Assert
        Assert.Equal(new[] { new CpuCoreTemp(0, 49), new CpuCoreTemp(1, 49) }, cpu.GetCoreTemps());
    }
}
