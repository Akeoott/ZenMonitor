// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Runtime.Versioning;

using Microsoft.Extensions.Logging;

using Moq;

using ZenMonitor.Core.Models;
using ZenMonitor.Core.Services.Linux;

namespace ZenMonitor.Tests.Services.Linux.CpuTests;

[SupportedOSPlatform("linux")]
public class CpuTempTests
{
    private readonly Mock<ILogger<Cpu>> _mockLogger;
    private readonly MockFileSystem _mockFileSystem;

    public CpuTempTests()
    {
        _mockLogger = new Mock<ILogger<Cpu>>();
        _mockFileSystem = new MockFileSystem();
    }
}
