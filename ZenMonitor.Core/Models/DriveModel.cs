// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

namespace ZenMonitor.Core.Models;

/// <summary>
/// An array containing the info of all mounted devices
/// </summary>
/// <param name="Index">Index of array</param>
/// <param name="MountPoint">Path to mount point (e.g. "/")</param>
/// <param name="DeviceName">Name of the drive</param>
/// <param name="FileSystem">Type of filesystem said drive uses (e.g "btrfs", "NTFS")</param>
/// <param name="TotalBytes">Total bytes in said drives</param>
/// <param name="AvailableBytes">Total available bytes in said drives</param>
/// <param name="UsedBytes">Total used bytes in said drives</param>
/// <param name="IOUsage">IO usage in %</param>
/// <returns>Array with info of mounted drives</returns>
public record DriveMountInfo(
    int Index,
    string MountPoint,
    string DeviceName,
    string FileSystem,
    long TotalBytes,
    long AvailableBytes,
    long UsedBytes,
    double IOUsage
);

public record DriveInfoSnapshot(
    DriveMountInfo[] MountInfos
);
