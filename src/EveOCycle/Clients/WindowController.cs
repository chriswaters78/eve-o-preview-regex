using EveOCycle.Configuration;
using EveOCycle.Interop;

namespace EveOCycle.Clients;

/// <summary>Brings a client window to the front and puts it where the configuration says.</summary>
internal static class WindowController
{
	/// <summary>
	/// Activates a window, restoring it first if it was minimized.
	/// </summary>
	/// <remarks>
	/// This works from a hotkey because Windows hands the foreground right to whichever
	/// process the hotkey was registered by. It would be refused if we called it unprompted.
	/// </remarks>
	public static void Activate(nint handle)
	{
		if (NativeMethods.IsIconic(handle))
		{
			NativeMethods.ShowWindow(handle, NativeMethods.SW_RESTORE);
		}

		NativeMethods.SetForegroundWindow(handle);
	}

	/// <summary>
	/// Moves and resizes a window to the given rectangle. Does nothing if it is already there.
	/// </summary>
	/// <returns>True if the window was actually moved.</returns>
	public static bool ApplyWindow(nint handle, WindowRect rect)
	{
		// A maximized window ignores being moved, so drop it out of that state first
		if (NativeMethods.IsZoomed(handle))
		{
			NativeMethods.ShowWindow(handle, NativeMethods.SW_RESTORE);
		}

		if (NativeMethods.GetWindowRect(handle, out RECT current)
			&& (current.Left == rect.X)
			&& (current.Top == rect.Y)
			&& (current.Width == rect.Width)
			&& (current.Height == rect.Height))
		{
			return false;
		}

		return NativeMethods.MoveWindow(handle, rect.X, rect.Y, rect.Width, rect.Height, true);
	}

	public static nint GetForegroundWindow()
	{
		return NativeMethods.GetForegroundWindow();
	}
}
