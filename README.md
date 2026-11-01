# Okisioli.Teacups

A collection of tiny, self-contained .NET libraries built around the don't-repeat-yourself principle.

## Windows window helpers

`Okisioli.Teacups.System.Windows` targets `net10.0-windows` and provides
`WindowHelper.HideFromTaskSwitcher(nint windowHandle)`. It hides a native window
from both Alt-Tab and the taskbar while preserving its other extended styles.
It has no WPF or Avalonia dependency.

Call it on the window's owning thread after its HWND is created, before showing
it. For WPF, keep `ShowInTaskbar="False"` and call it from `OnSourceInitialized`:

```csharp
protected override void OnSourceInitialized(EventArgs e)
{
    base.OnSourceInitialized(e);
    Okisioli.Teacups.System.Windows.WindowHelper.HideFromTaskSwitcher(
        new System.Windows.Interop.WindowInteropHelper(this).Handle);
}
```

The helper rejects a zero handle and throws `Win32Exception` if a native operation
fails. Callers supply the native handle using their UI framework's own APIs.
