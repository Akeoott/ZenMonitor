// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using Microsoft.Extensions.Logging;

using Moq;

using ZenMonitor.Core.Services.Null;

namespace ZenMonitor.Tests.Services.Null.Tests;

[Trait("Platform", "Linux")] // Only to define runner, does not reflect reality XP
public class MemoryTests
{
    private readonly Mock<ILogger<Memory>> _mockLogger;

    public MemoryTests()
    {
        _mockLogger = new Mock<ILogger<Memory>>();
    }

    private Memory CreateMemory() => new(_mockLogger.Object);

    [Fact]
    public void GetAll_CheckThatEverythingIsNull()
    {

        var memory = CreateMemory();
        memory.Update();
        Assert.Equal(0, memory.GetMemTotal());
        Assert.Equal(0, memory.GetMemFree());
        Assert.Equal(0, memory.GetMemAvailable());
        Assert.Equal(0, memory.GetMemUsed());
        Assert.Equal(0, memory.GetCached());
        Assert.Equal(0, memory.GetSwapTotal());
        Assert.Equal(0, memory.GetSwapFree());
    }
}
