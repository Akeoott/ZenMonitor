// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using System.IO.Abstractions.TestingHelpers;
using System.Runtime.Versioning;

using Microsoft.Extensions.Logging;

using Moq;

using ZenMonitor.Core.Interfaces;
using ZenMonitor.Core.Models;
using ZenMonitor.Core.Services.Windows;

namespace ZenMonitor.Tests.Services.Windows.Tests;

[Trait("Platform", "Windows")]
[SupportedOSPlatform("windows")]
public class CpuTests
{
    private readonly Mock<ILogger<Cpu>> _mockLogger;
    private readonly MockFileSystem _mockFileSystem;
    private readonly Mock<IHelper> _mockHelper;

    public CpuTests()
    {
        _mockLogger = new Mock<ILogger<Cpu>>();
        _mockFileSystem = new MockFileSystem();
        _mockHelper = new Mock<IHelper>();
    }

    private Cpu CreateCpu() => new(_mockLogger.Object, _mockFileSystem, _mockHelper.Object);
}
