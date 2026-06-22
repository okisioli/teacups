using System.Diagnostics;

namespace Olivercode.Dry.System;

public static class ProcessHelper
{
    public static bool TryOpenFile(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        if (!File.Exists(filePath))
            return false;

        return Process.Start(new ProcessStartInfo(filePath)
        {
            UseShellExecute = true
        }) is not null;
    }
}
