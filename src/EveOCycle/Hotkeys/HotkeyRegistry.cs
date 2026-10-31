using System.Runtime.InteropServices;
using EveOCycle.Interop;

namespace EveOCycle.Hotkeys;

/// <summary>
/// Holds the system wide hotkey registrations and routes each WM_HOTKEY message back
/// to the action that asked for it.
/// </summary>
/// <remarks>
/// Hotkeys are registered against the calling thread rather than a window, so every
/// call here has to happen on the same thread that runs the message loop.
/// </remarks>
internal sealed class HotkeyRegistry : IDisposable
{
	private readonly Dictionary<int, Registration> _registrations = [];
	private readonly Dictionary<Hotkey, string> _descriptions = [];
	private int _nextId = 1;
	private bool _disposed;

	private sealed record Registration(Hotkey Hotkey, string Description, Action Action);

	public bool TryRegister(Hotkey hotkey, string description, Action action, out string error)
	{
		error = string.Empty;

		if (this._descriptions.TryGetValue(hotkey, out string? existing))
		{
			error = $"already used by {existing}";
			return false;
		}

		int id = this._nextId;

		if (!NativeMethods.RegisterHotKey(nint.Zero, id, hotkey.Modifiers | NativeMethods.MOD_NOREPEAT, hotkey.VirtualKey))
		{
			int lastError = Marshal.GetLastWin32Error();
			error = lastError == NativeMethods.ERROR_HOTKEY_ALREADY_REGISTERED
					? "another application has already claimed this key combination"
					: $"the system refused the registration (error {lastError})";
			return false;
		}

		this._nextId++;
		this._registrations[id] = new Registration(hotkey, description, action);
		this._descriptions[hotkey] = description;

		return true;
	}

	/// <summary>
	/// Runs the action bound to the given hotkey id. Any failure inside an action is
	/// reported and swallowed - one bad key press should not take the application down.
	/// </summary>
	public void Dispatch(int id)
	{
		if (!this._registrations.TryGetValue(id, out Registration? registration))
		{
			return;
		}

		try
		{
			registration.Action();
		}
		catch (Exception exception)
		{
			Log.Error($"{registration.Description} ({registration.Hotkey}) failed: {exception.Message}");
		}
	}

	public void Dispose()
	{
		if (this._disposed)
		{
			return;
		}

		this._disposed = true;

		foreach (int id in this._registrations.Keys)
		{
			NativeMethods.UnregisterHotKey(nint.Zero, id);
		}

		this._registrations.Clear();
		this._descriptions.Clear();
	}
}
