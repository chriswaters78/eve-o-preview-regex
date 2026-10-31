using EveOCycle.Hotkeys;
using EveOCycle.Interop;

namespace EveOCycle;

/// <summary>
/// The thread message pump that WM_HOTKEY messages arrive on. Hotkeys registered
/// without a window are delivered to the thread's own queue, so a console
/// application needs nothing more than this to receive them.
/// </summary>
internal sealed class MessageLoop
{
	private readonly uint _threadId = NativeMethods.GetCurrentThreadId();

	/// <summary>
	/// Pumps messages until <see cref="Stop" /> is called. Must run on the same thread
	/// that registered the hotkeys.
	/// </summary>
	public void Run(HotkeyRegistry registry)
	{
		while (true)
		{
			int result = NativeMethods.GetMessage(out MSG message, nint.Zero, 0, 0);

			if (result == 0)
			{
				// WM_QUIT
				return;
			}

			if (result == -1)
			{
				Log.Error("the message loop failed, shutting down");
				return;
			}

			if (message.Message == NativeMethods.WM_HOTKEY)
			{
				registry.Dispatch((int)message.WParam);
			}
		}
	}

	/// <summary>Ends the message loop. Safe to call from any thread.</summary>
	public void Stop()
	{
		NativeMethods.PostThreadMessage(this._threadId, NativeMethods.WM_QUIT, nint.Zero, nint.Zero);
	}
}
