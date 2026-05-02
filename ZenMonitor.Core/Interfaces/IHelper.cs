// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using ZenMonitor.Core.Models;

namespace ZenMonitor.Core.Interfaces;

public interface IHelper
{
    /// <summary>
    /// Mockable DateTime.UtcNow Interface
    /// </summary>
    /// <value>DateTime.UtcNow</value>
    DateTime UtcNow { get; }

    /// <summary>
    /// Run a process on linux
    /// </summary>
    /// <param name="fileName">Name of the application (e.g `nvidia-smi`)</param>
    /// <param name="arguments">flags passed to the application</param>
    /// <returns>
    /// response of the application
    /// `ProcessResult(int ExitCode, string StandardOutput, string StandardError)`
    /// </returns>
    ProcessResult RunProcess(string fileName, string arguments);
}
