// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Versioning;

using ZenMonitor.Core.Interfaces;
using ZenMonitor.Core.Models;

namespace ZenMonitor.Core.Services.Windows;

/// <summary>
/// Used for the mockability of code. No general helpers are here.
/// Just required things for unit tests.
/// </summary>
[ExcludeFromCodeCoverage]
[SupportedOSPlatform("windows")]
public class Helper : IHelper
{
    public DateTime UtcNow => DateTime.UtcNow;

    public ProcessResult RunProcess(string fileName, string arguments)
    {
        return new(1, "", "RunProcess is NOT implemented!");
    }
}
