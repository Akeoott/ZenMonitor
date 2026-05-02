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
public class CoreSpeedsTests
{
    private readonly Mock<ILogger<Cpu>> _mockLogger;
    private readonly MockFileSystem _mockFileSystem;
    private readonly Mock<ITimeService> _timeService;

    public CoreSpeedsTests()
    {
        _mockLogger = new Mock<ILogger<Cpu>>();
        _mockFileSystem = new MockFileSystem();
        _timeService = new Mock<ITimeService>();
    }

    [Fact]
    public void GetCoreSpeeds_ReturnsCoreSpeeds()
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
        _mockFileSystem.AddFile("/proc/stat", new MockFileData(stat2)); // overwrites the file

        cpu.Update(); // Fills second snapshot and returns real values

        //! Pre-Calculated values, `cpu.GetCoreSpeeds()` test must return this.
        var speeds = new List<CpuCoreSpeed>();
        double[] mhzLines = [
            4399.214, 4375.453, 4398.893, 4394.1,
            4368.925, 4395.762, 4397.023, 4398.63,
            2983.319, 4370.437, 2983.319, 4399.153,
            3817.651, 4306.206, 2983.319, 2983.319
        ];
        int coreIndex = 0;
        foreach (var mhz in mhzLines)
        {

            speeds.Add(new CpuCoreSpeed(coreIndex, mhz));
            coreIndex++;
        }
        speeds.ToArray();

        // Assert
        Assert.Equal(speeds, cpu.GetCoreSpeeds() /*must return an array with values in %*/);
    }
}
