using System.Text;
using EveOCycle.Interop;

namespace EveOCycle.Hotkeys;

/// <summary>
/// A parsed hotkey: zero or more modifiers plus the key itself. Two hotkeys written
/// differently in the configuration file - "Ctrl+Multiply" and "Control+*" - compare
/// equal, which is what duplicate detection relies on.
/// </summary>
internal readonly record struct Hotkey(uint Modifiers, uint VirtualKey)
{
	private static readonly (string Name, uint Flag)[] ModifierNames =
	[
		("Control", NativeMethods.MOD_CONTROL),
		("Ctrl", NativeMethods.MOD_CONTROL),
		("Alt", NativeMethods.MOD_ALT),
		("Shift", NativeMethods.MOD_SHIFT),
		("Windows", NativeMethods.MOD_WIN),
		("Win", NativeMethods.MOD_WIN)
	];

	public static bool TryParse(string? text, out Hotkey hotkey, out string error)
	{
		hotkey = default;
		error = string.Empty;

		string remaining = (text ?? string.Empty).Trim();
		if (remaining.Length == 0)
		{
			error = "the hotkey is empty";
			return false;
		}

		// Modifiers are peeled off the front one at a time rather than splitting on '+',
		// so that a hotkey whose key IS '+' - written "Control++" - still reads correctly
		uint modifiers = 0;
		bool matched = true;
		while (matched)
		{
			matched = false;
			foreach ((string name, uint flag) in Hotkey.ModifierNames)
			{
				if ((remaining.Length > name.Length)
					&& (remaining[name.Length] == '+')
					&& remaining.StartsWith(name, StringComparison.OrdinalIgnoreCase))
				{
					modifiers |= flag;
					remaining = remaining[(name.Length + 1)..].TrimStart();
					matched = true;
					break;
				}
			}
		}

		if (remaining.Length == 0)
		{
			error = "no key was given, only modifiers";
			return false;
		}

		if (!VirtualKeys.TryGetCode(remaining, out uint code))
		{
			error = $"'{remaining}' is not a key name";
			return false;
		}

		hotkey = new Hotkey(modifiers, code);
		return true;
	}

	public override string ToString()
	{
		StringBuilder text = new();

		if ((this.Modifiers & NativeMethods.MOD_CONTROL) != 0)
		{
			text.Append("Control+");
		}

		if ((this.Modifiers & NativeMethods.MOD_ALT) != 0)
		{
			text.Append("Alt+");
		}

		if ((this.Modifiers & NativeMethods.MOD_SHIFT) != 0)
		{
			text.Append("Shift+");
		}

		if ((this.Modifiers & NativeMethods.MOD_WIN) != 0)
		{
			text.Append("Windows+");
		}

		return text.Append(VirtualKeys.GetDisplayName(this.VirtualKey)).ToString();
	}
}
