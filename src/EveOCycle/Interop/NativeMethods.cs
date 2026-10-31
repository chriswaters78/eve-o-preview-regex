using System.Runtime.InteropServices;

namespace EveOCycle.Interop;

/// <summary>
/// The whole of the Win32 surface this application needs: global hotkeys, a message
/// pump to receive them on, and enough window management to activate and place a window.
/// </summary>
internal static partial class NativeMethods
{
	public const uint WM_QUIT = 0x0012;
	public const uint WM_HOTKEY = 0x0312;

	public const uint MOD_ALT = 0x0001;
	public const uint MOD_CONTROL = 0x0002;
	public const uint MOD_SHIFT = 0x0004;
	public const uint MOD_WIN = 0x0008;

	// Stops a held down key from firing the hotkey over and over again
	public const uint MOD_NOREPEAT = 0x4000;

	public const int SW_RESTORE = 9;

	public const int ERROR_HOTKEY_ALREADY_REGISTERED = 1409;

	[LibraryImport("user32.dll", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static partial bool RegisterHotKey(nint hWnd, int id, uint fsModifiers, uint vk);

	[LibraryImport("user32.dll", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static partial bool UnregisterHotKey(nint hWnd, int id);

	[LibraryImport("user32.dll", EntryPoint = "GetMessageW")]
	public static partial int GetMessage(out MSG lpMsg, nint hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

	[LibraryImport("user32.dll", EntryPoint = "PostThreadMessageW", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static partial bool PostThreadMessage(uint idThread, uint msg, nint wParam, nint lParam);

	[LibraryImport("kernel32.dll")]
	public static partial uint GetCurrentThreadId();

	[LibraryImport("user32.dll")]
	public static partial nint GetForegroundWindow();

	[LibraryImport("user32.dll")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static partial bool SetForegroundWindow(nint hWnd);

	[LibraryImport("user32.dll")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static partial bool ShowWindow(nint hWnd, int nCmdShow);

	[LibraryImport("user32.dll")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static partial bool IsIconic(nint hWnd);

	[LibraryImport("user32.dll")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static partial bool IsZoomed(nint hWnd);

	[LibraryImport("user32.dll", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static partial bool GetWindowRect(nint hWnd, out RECT lpRect);

	[LibraryImport("user32.dll", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static partial bool MoveWindow(nint hWnd, int x, int y, int width, int height, [MarshalAs(UnmanagedType.Bool)] bool repaint);
}

[StructLayout(LayoutKind.Sequential)]
internal struct RECT
{
	public int Left;
	public int Top;
	public int Right;
	public int Bottom;

	public readonly int Width => this.Right - this.Left;
	public readonly int Height => this.Bottom - this.Top;
}

[StructLayout(LayoutKind.Sequential)]
internal struct MSG
{
	public nint Hwnd;
	public uint Message;
	public nint WParam;
	public nint LParam;
	public uint Time;
	public int PointX;
	public int PointY;
}
