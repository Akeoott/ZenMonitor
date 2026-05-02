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
public class CoreUsagesTests
{
    private readonly Mock<ILogger<Cpu>> _mockLogger;
    private readonly MockFileSystem _mockFileSystem;
    private readonly Mock<ITimeService> _timeService;

    public CoreUsagesTests()
    {
        _mockLogger = new Mock<ILogger<Cpu>>();
        _mockFileSystem = new MockFileSystem();
        _timeService = new Mock<ITimeService>();
    }

    [Fact]
    public void GetCoreUsages_ReturnsCoreUsages()
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

        //! Pre-Calculated values, `cpu.GetCoreUsages()` test must return this.
        var usages = new List<CpuCoreUsage>();
        double[] percentLines = [
            7, 17, 3, 12,
            3, 6, 1, 2,
            1, 1, 2, 2,
            4, 1, 1, 0
        ];
        int coreIndex = 0;
        foreach (var percent in percentLines)
        {

            usages.Add(new CpuCoreUsage(coreIndex, percent));
            coreIndex++;
        }
        usages.ToArray();

        // Assert
        Assert.Equal(usages, cpu.GetCoreUsages() /*must return an array with values in %*/);
    }
}
