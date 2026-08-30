using System.Diagnostics;

namespace Okisioli.Teacups.System;

/// <summary>Provides helpers for opening files with the operating system.</summary>
public static class ProcessHelper
{
    /// <summary>Attempts to open an existing file with its registered application.</summary>
    /// <param name="filePath">The path to the file to open.</param>
    /// <returns><see langword="true"/> when a process was started; otherwise, <see langword="false"/>.</returns>
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
