// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using Microsoft.Extensions.Logging;

using Moq;

using ZenMonitor.Core.Services.Null;

namespace ZenMonitor.Tests.Services.Null.Tests;

[Trait("Platform", "Linux")] // Only to define runner, does not reflect reality XP
public class DriveTests
{
    private readonly Mock<ILogger<Drive>> _mockLogger;

    public DriveTests()
    {
        _mockLogger = new Mock<ILogger<Drive>>();
    }

    private Drive CreateDrive() => new(_mockLogger.Object);

    [Fact]
    public void GetAll_CheckThatEverythingIsNull()
    {
        var drive = CreateDrive();
        drive.Update();
        Assert.Equal([], drive.GetMountInfos());
    }
}
