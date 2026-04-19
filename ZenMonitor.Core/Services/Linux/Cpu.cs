// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using System.Runtime.Versioning;

using ZenMonitor.Core.Interfaces;
using ZenMonitor.Core.Models;

namespace ZenMonitor.Core.Services.Linux;

[SupportedOSPlatform("linux")]
public class Cpu : IHardwareService
{
    public string GetCpuName()
    {
        // get cpu name :P
        var line = File.ReadLines("/proc/cpuinfo")
                       .FirstOrDefault(l => l.StartsWith("model name"));
        return line?.Split(':')[1].Trim() ?? "Unknown CPU";
    }

    public double[] GetCoreSpeeds()
    {
        // Reads the current MHz for every logical core
        return [.. File.ReadLines("/proc/cpuinfo")
            .Where(l => l.StartsWith("cpu MHz")).Select(l => double.Parse(l.Split(':')[1].Trim()))];
    }

    #region Cpu Core Usages
    private long[][] _snapshots = [];           // current ticks
    private long[][] _previousSnapshots = [];   // previous ticks

    /// <summary>
    /// Calculates CPU usage percentages for all CPUs (aggregate + per-core)
    /// using tick differences from /proc/stat.
    ///
    /// The first element (CpuIndex = 0) represents total CPU usage.
    /// Subsequent elements represent individual CPU cores.
    ///
    /// Idle time includes both "idle" and "iowait".
    /// </summary>
    public CpuUsage[] GetCoreUsages()
    {
        UpdateAllTicks(); // fills _snapshots (current buffer)

        if (_previousSnapshots.Length == 0)
        {
            EnsurePreviousMatchesCurrent();
            SwapBuffers();

            return [.. _snapshots.Select((_, i) => new CpuUsage(i, 0))];
        }

        var usages = new CpuUsage[_snapshots.Length];

        for (int i = 0; i < _snapshots.Length; i++)
        {
            var prev = _previousSnapshots[i];
            var curr = _snapshots[i];

            int len = Math.Min(prev.Length, curr.Length);

            long totalA = 0, totalB = 0;

            for (int j = 0; j < len; j++)
            {
                totalA += prev[j];
                totalB += curr[j];
            }

            long diffTotal = totalB - totalA;

            long idleA = prev.Length > 4 ? prev[3] + prev[4]
                       : prev.Length > 3 ? prev[3]
                       : 0;

            long idleB = curr.Length > 4 ? curr[3] + curr[4]
                       : curr.Length > 3 ? curr[3]
                       : 0;

            long diffIdle = idleB - idleA;

            double usage = 0;

            if (diffTotal > 0)
            {
                usage = (double)(diffTotal - diffIdle) / diffTotal * 100.0;
            }

            usages[i] = new CpuUsage(i, Math.Round(usage));
        }

        SwapBuffers();

        return usages;
    }

    private void UpdateAllTicks()
    {
        var lines = File.ReadLines("/proc/stat").TakeWhile(l => l.StartsWith("cpu"));

        int i = 0;
        foreach (var line in lines)
        {
            var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            int fieldCount = parts.Length - 1;

            if (_snapshots.Length <= i)
            {
                Array.Resize(ref _snapshots, Math.Max(i + 1, _snapshots.Length * 2));
            }

            _snapshots[i] ??= new long[fieldCount];
            if (_snapshots[i].Length != fieldCount)
            {
                _snapshots[i] = new long[fieldCount];
            }

            for (int j = 0; j < fieldCount; j++)
            {
                _snapshots[i][j] = long.Parse(parts[j + 1]);
            }

            i++;
        }

        if (_snapshots.Length > i)
            Array.Resize(ref _snapshots, i);
    }

    private void EnsurePreviousMatchesCurrent()
    {
        if (_previousSnapshots.Length != _snapshots.Length)
        {
            _previousSnapshots = new long[_snapshots.Length][];
        }

        for (int i = 0; i < _snapshots.Length; i++)
        {
            var curr = _snapshots[i];

            if (_previousSnapshots[i] == null || _previousSnapshots[i].Length != curr.Length)
            {
                _previousSnapshots[i] = new long[curr.Length];
            }

            Array.Copy(curr, _previousSnapshots[i], curr.Length);
        }
    }

    private void SwapBuffers() => (_snapshots, _previousSnapshots) = (_previousSnapshots, _snapshots);
    #endregion
}
