// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

namespace ZenMonitor.Tests.Services.Linux;

/// <summary>
/// Provides raw text content of mock Linux system files stored in the TestData/ subfolder.
/// The files are expected to be copied to the output directory (Copy to Output Directory = Copy if newer).
/// </summary>
public static class TestData
{
    private static readonly string BaseDir =
        Path.Combine(AppContext.BaseDirectory, "Services/Linux/TestData");

    /// <summary>
    /// /proc/cpuinfo
    /// </summary>
    public static string CpuInfo() => ReadFile("cpuinfo");

    /// <summary>
    /// /proc/stat
    /// </summary>
    public static string Stat() => ReadFile("stat");

    /// <summary>
    /// /sys/class/hwmon directory listing (not a file, we'll handle hwmon as a set of files later)
    /// </summary>
    //! For hwmon, we'll define separate methods for individual files when needed.

    /// <summary>
    /// /sys/class/powercap/intel-rapl:0/energy_uj
    /// </summary>
    public static string EnergyUj() => ReadFile("energy_uj");

    /// <summary>
    /// /proc/meminfo
    /// </summary>
    public static string MemInfo() => ReadFile("meminfo");

    /// <summary>
    /// /proc/sys/kernel/osrelease
    /// </summary>
    public static string OsRelease() => ReadFile("osrelease");

    /// <summary>
    /// /proc/sys/kernel/hostname
    /// </summary>
    public static string Hostname() => ReadFile("hostname");

    /// <summary>
    /// /proc/uptime
    /// </summary>
    public static string Uptime() => ReadFile("uptime");

    /// <summary>
    /// /proc/loadavg
    /// </summary>
    public static string LoadAvg() => ReadFile("loadavg");

    private static string ReadFile(string filename)
    {
        string path = Path.Combine(BaseDir, filename);
        if (!File.Exists(path))
            throw new FileNotFoundException($"Test data file not found: {path}. Ensure it is set to 'Copy to Output Directory'.");
        return File.ReadAllText(path);
    }
}
