// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using Microsoft.Extensions.Logging;

using Moq;

namespace ZenMonitor.Tests.Services.Null.Tests;

[Trait("Platform", "Linux")] // Only to define runner, does not reflect reality XP
public class SystemTests
{
    private readonly Mock<ILogger<Core.Services.Null.System>> _mockLogger;

    public SystemTests()
    {
        _mockLogger = new Mock<ILogger<Core.Services.Null.System>>();
    }

    private Core.Services.Null.System CreateSystem() => new(_mockLogger.Object);

    [Fact]
    public void GetAll_CheckThatEverythingIsNull()
    {
        var system = CreateSystem();
        system.Update();
        Assert.Equal("", system.GetKernelVersion());
        Assert.Equal("", system.GetHostname());
        Assert.Equal(0, system.GetUptimeSeconds());
        Assert.Equal(0, system.GetRunningTasks());
        Assert.Equal(0, system.GetTotalTasks());
    }
}
