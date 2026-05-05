// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using System.IO.Abstractions.TestingHelpers;
using System.Runtime.Versioning;

using Microsoft.Extensions.Logging;

using Moq;

using ZenMonitor.Core.Interfaces;
using ZenMonitor.Core.Models;
using ZenMonitor.Core.Services.Linux;

namespace ZenMonitor.Tests.Services.Linux.DriveTests;

[SupportedOSPlatform("linux")]
public class DriveTests
{
    private readonly Mock<ILogger<Drive>> _mockLogger;
    private readonly MockFileSystem _mockFileSystem;
    private readonly Mock<IHelper> _mockHelper;

    public DriveTests()
    {
        _mockLogger = new Mock<ILogger<Drive>>();
        _mockFileSystem = new MockFileSystem();
        _mockHelper = new Mock<IHelper>();
    }

    private Drive CreateDrive() => new(_mockLogger.Object, _mockFileSystem, _mockHelper.Object);

    [Fact]
    public void GetMountInfos_ReturnsMountInfos()
    {
        _mockFileSystem.AddFile("/proc/diskstats", new MockFileData(TestData.DiskStats1()));
        _mockHelper.Setup(h => h.RunProcess("df", "-T -B1")).Returns(new ProcessResult(0, TestData.DfOutput(), ""));

        var drive = CreateDrive();
        drive.Update();

        var mountInfos = drive.GetMountInfos();

        Assert.Equal(2, mountInfos.Length);
        Assert.Equal("/", mountInfos[0].MountPoint);
        Assert.Equal("/dev/sda1", mountInfos[0].DeviceName);
        Assert.Equal("ext4", mountInfos[0].FileSystem);
        Assert.Equal(1000000000, mountInfos[0].TotalBytes);
        Assert.Equal(400000000, mountInfos[0].AvailableBytes);
        Assert.Equal(500000000, mountInfos[0].UsedBytes);
        Assert.Equal(0, mountInfos[0].IOUsage); // First call, 0

        Assert.Equal("/tmp", mountInfos[1].MountPoint);
        Assert.Equal("tmpfs", mountInfos[1].FileSystem);
        Assert.Equal(0, mountInfos[1].IOUsage); // tmpfs no device
    }

    [Fact]
    public void GetMountInfos_ReturnsIOUsageAfterUpdate()
    {
        _mockFileSystem.AddFile("/proc/diskstats", new MockFileData(TestData.DiskStats1()));
        _mockHelper.Setup(h => h.RunProcess("df", "-T -B1")).Returns(new ProcessResult(0, TestData.DfOutput(), ""));

        var drive = CreateDrive();
        drive.Update();

        _mockFileSystem.AddFile("/proc/diskstats", new MockFileData(TestData.DiskStats2()));
        drive.Update();

        var mountInfos = drive.GetMountInfos();

        Assert.Equal(2, mountInfos.Length);
        Assert.True(mountInfos[0].IOUsage >= 0); // IO usage calculated
    }
}
