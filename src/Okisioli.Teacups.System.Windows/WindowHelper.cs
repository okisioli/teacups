using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Okisioli.Teacups.System.Windows;

public static class WindowHelper
{
    private const int ExtendedStyleIndex = -20;
    private const int ToolWindowStyle = 0x00000080;
    private const int AppWindowStyle = 0x00040000;
    private const uint RefreshFrameFlags = 0x0001 | 0x0002 | 0x0004 | 0x0010 | 0x0020;

    /// <summary>
    /// Hides a native Windows window from Alt-Tab and the taskbar.
    /// </summary>
    /// <param name="windowHandle">
    /// The HWND of a window owned by the calling application. Obtain it through your UI framework.
    /// </param>
    /// <remarks>
    /// Call on the window's owning thread after its handle is created and before it is first shown.
    /// The helper works independently of WPF and Avalonia and preserves unrelated extended styles,
    /// including transparency and topmost behavior.
    /// </remarks>
    /// <example>
    /// In WPF, set <c>ShowInTaskbar="False"</c> on the window and call this method
    /// from <c>OnSourceInitialized</c>.
    /// <code>
    /// protected override void OnSourceInitialized(EventArgs e)
    /// {
    ///     base.OnSourceInitialized(e);
    ///     Okisioli.Teacups.System.Windows.WindowHelper.HideFromTaskSwitcher(
    ///         new System.Windows.Interop.WindowInteropHelper(this).Handle);
    /// }
    /// </code>
    /// </example>
    /// <exception cref="ArgumentException">The window handle is zero.</exception>
    /// <exception cref="Win32Exception">A native window operation fails.</exception>
    public static void HideFromTaskSwitcher(nint windowHandle)
    {
        if (windowHandle == 0)
            throw new ArgumentException("A nonzero window handle is required.", nameof(windowHandle));

        int styles = GetWindowLong(windowHandle, ExtendedStyleIndex);
        ThrowIfFailed(styles);

        int updatedStyles = (styles | ToolWindowStyle) & ~AppWindowStyle;
        int previousStyles = SetWindowLong(windowHandle, ExtendedStyleIndex, updatedStyles);
        ThrowIfFailed(previousStyles);

        // Refresh cached styles without moving, resizing, reordering, or activating the window.
        if (!SetWindowPos(windowHandle, 0, 0, 0, 0, 0, RefreshFrameFlags))
            throw new Win32Exception(Marshal.GetLastPInvokeError());
    }

    private static void ThrowIfFailed(int result)
    {
        int error = Marshal.GetLastPInvokeError();
        if (result == 0 && error != 0)
            throw new Win32Exception(error);
    }

    // GWL_EXSTYLE is a 32-bit value on both 32-bit and 64-bit Windows; no pointers are read or written.
    // SetLastError clears the native last error before each call, since a zero result can mean success.
    [DllImport("user32.dll", EntryPoint = "GetWindowLongW", ExactSpelling = true, SetLastError = true)]
    private static extern int GetWindowLong(nint windowHandle, int index);

    [DllImport("user32.dll", EntryPoint = "SetWindowLongW", ExactSpelling = true, SetLastError = true)]
    private static extern int SetWindowLong(nint windowHandle, int index, int value);

    [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetWindowPos(nint windowHandle, nint insertAfter, int x, int y,
        int width, int height, uint flags);
}
