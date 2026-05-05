// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using System.IO.Abstractions;
using System.Linq;
using System.Runtime.Versioning;

using Microsoft.Extensions.Logging;

using ZenMonitor.Core.Interfaces;
using ZenMonitor.Core.Models;

namespace ZenMonitor.Core.Services.Linux;

[SupportedOSPlatform("linux")]
public class Drive(ILogger<Drive> logger, IFileSystem fileSystem, IHelper helper) : IDrive
{
    private readonly ILogger<Drive> _logger = logger;
    private readonly IFileSystem _fileSystem = fileSystem;
    private readonly IHelper _helper = helper;
    private DriveInfoSnapshot _snapshot = new([]);

    private readonly Dictionary<string, (long ioTime, DateTime time)> _previousDiskStats = [];

    public void Update() => _snapshot = FetchDriveInfo();

    public DriveMountInfo[] GetMountInfos() => _snapshot.MountInfos;

    private DriveInfoSnapshot FetchDriveInfo()
    {
        _logger.LogTrace("Fetching all Drive info...");

        var mountInfos = ReadMountInfos();
        var ioUsages = ReadIOUsages();

        var updatedMountInfos = mountInfos.Select(m =>
        {
            string device = m.DeviceName.StartsWith("/dev/") ? m.DeviceName[5..] : m.DeviceName;
            double ioUsage = ioUsages.GetValueOrDefault(device, 0);
            return m with { IOUsage = ioUsage };
        }).ToArray();

        return new DriveInfoSnapshot(updatedMountInfos);
    }

    private DriveMountInfo[] ReadMountInfos()
    {
        string dfOutput = RunDf("-T -B1");
        var lines = dfOutput.Split('\n', StringSplitOptions.RemoveEmptyEntries).Skip(1);
        var mountInfos = new List<DriveMountInfo>();
        int index = 0;

        // TODO: Filter out pseudo filesystems
        //       (gonna sleep now, leaving that note just in case XP)
        foreach (var line in lines)
        {
            var parts = line.Split([' '], StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 7)
            {
                string deviceName = parts[0];
                string fileSystem = parts[1];
                long totalBytes = long.Parse(parts[2]);
                long usedBytes = long.Parse(parts[3]);
                long availableBytes = long.Parse(parts[4]);
                string mountPoint = parts[6];

                mountInfos.Add(new DriveMountInfo(
                    index++,
                    mountPoint,
                    deviceName,
                    fileSystem,
                    totalBytes,
                    availableBytes,
                    usedBytes,
                    // TODO: Gotta test the ReadIOUsages method later
                    0 // IOUsage will be used a little laterrrr
                ));
            }
        }

        return [.. mountInfos];
    }

    private Dictionary<string, double> ReadIOUsages()
    {
        var ioUsages = new Dictionary<string, double>();
        var lines = _fileSystem.File.ReadAllLines("/proc/diskstats");

        foreach (var line in lines)
        {
            var parts = line.Split([' '], StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 14)
            {
                string name = parts[2];
                long ioTime = long.Parse(parts[12]);

                if (_previousDiskStats.TryGetValue(name, out var prev))
                {
                    double deltaTime = (_helper.UtcNow - prev.time).TotalMilliseconds;
                    double deltaIo = ioTime - prev.ioTime;
                    double usage = deltaTime > 0 ? deltaIo / deltaTime * 100 : 0;
                    ioUsages[name] = usage;
                }
                else
                {
                    ioUsages[name] = 0;
                }

                _previousDiskStats[name] = (ioTime, _helper.UtcNow);
            }
        }

        return ioUsages;
    }

    private string RunDf(string arguments)
    {
        var result = _helper.RunProcess("df", arguments);
        if (result.ExitCode != 0)
        {
            _logger.LogError("df failed: {Error}", result.StandardError);
            throw new InvalidOperationException($"df error: {result.StandardError}");
        }
        return result.StandardOutput;
    }
}
