// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using System;
using System.Threading.Tasks;

namespace ZenMonitor;

public static class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("Dotnet running");
        await Task.Delay(-1);
    }
}
