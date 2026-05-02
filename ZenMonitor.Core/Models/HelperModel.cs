// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

namespace ZenMonitor.Core.Models;

/// <summary>
/// Stores results from processes
/// </summary>
/// <param name="ExitCode">exit code of the process</param>
/// <param name="StandardOutput">standard output of process</param>
/// <param name="StandardError">error output of process</param>
/// <returns></returns>
public sealed record ProcessResult(int ExitCode, string StandardOutput, string StandardError);
