namespace EveOCycle.Hotkeys;

/// <summary>
/// Maps the key names used in the configuration file onto Windows virtual key codes.
/// The names follow the ones Windows itself uses, with a few aliases for the keys
/// whose printed symbol is nothing like their name - the keypad operators especially.
/// </summary>
internal static class VirtualKeys
{
	private static readonly Dictionary<string, uint> Names = BuildNames();

	// The name reported back to the user for a given code, f.e. in the startup summary
	private static readonly Dictionary<uint, string> DisplayNames = BuildDisplayNames();

	public static bool TryGetCode(string name, out uint code)
	{
		return Names.TryGetValue(name.Trim(), out code);
	}

	public static string GetDisplayName(uint code)
	{
		return DisplayNames.TryGetValue(code, out string? name) ? name : $"0x{code:X2}";
	}

	private static Dictionary<string, uint> BuildNames()
	{
		Dictionary<string, uint> names = new(StringComparer.OrdinalIgnoreCase);

		void Add(string name, uint code) => names[name] = code;

		// Letters and the digits along the top of the keyboard
		for (uint code = 'A'; code <= 'Z'; code++)
		{
			Add(((char)code).ToString(), code);
		}

		for (uint digit = 0; digit <= 9; digit++)
		{
			Add(digit.ToString(), 0x30 + digit);
			Add($"D{digit}", 0x30 + digit);
		}

		// Function keys. F13 upwards do not exist on a normal keyboard, which is
		// exactly why they are a popular choice for a gaming device to send
		for (uint index = 1; index <= 24; index++)
		{
			Add($"F{index}", 0x6F + index);
		}

		// Numeric keypad
		for (uint digit = 0; digit <= 9; digit++)
		{
			Add($"NumPad{digit}", 0x60 + digit);
		}

		Add("Multiply", 0x6A);
		Add("NumPadMultiply", 0x6A);
		Add("*", 0x6A);

		Add("Add", 0x6B);
		Add("NumPadAdd", 0x6B);
		Add("Plus", 0x6B);
		Add("+", 0x6B);

		Add("Subtract", 0x6D);
		Add("NumPadSubtract", 0x6D);
		Add("Minus", 0x6D);
		Add("-", 0x6D);

		Add("Divide", 0x6F);
		Add("NumPadDivide", 0x6F);
		Add("/", 0x6F);

		Add("Decimal", 0x6E);
		Add("NumPadDecimal", 0x6E);

		// Navigation and editing
		Add("Left", 0x25);
		Add("Up", 0x26);
		Add("Right", 0x27);
		Add("Down", 0x28);
		Add("Home", 0x24);
		Add("End", 0x23);
		Add("PageUp", 0x21);
		Add("Prior", 0x21);
		Add("PageDown", 0x22);
		Add("Next", 0x22);
		Add("Insert", 0x2D);
		Add("Delete", 0x2E);

		Add("Space", 0x20);
		Add("Enter", 0x0D);
		Add("Return", 0x0D);
		Add("Tab", 0x09);
		Add("Escape", 0x1B);
		Add("Esc", 0x1B);
		Add("Backspace", 0x08);
		Add("Back", 0x08);

		Add("Pause", 0x13);
		Add("ScrollLock", 0x91);
		Add("PrintScreen", 0x2C);

		// A handful of the printable keys that are useful as hotkeys
		Add("OemTilde", 0xC0);
		Add("`", 0xC0);
		Add("OemMinus", 0xBD);
		Add("OemPlus", 0xBB);
		Add("OemOpenBrackets", 0xDB);
		Add("[", 0xDB);
		Add("OemCloseBrackets", 0xDD);
		Add("]", 0xDD);
		Add("OemBackslash", 0xDC);
		Add("OemSemicolon", 0xBA);
		Add(";", 0xBA);
		Add("OemQuotes", 0xDE);
		Add("OemComma", 0xBC);
		Add(",", 0xBC);
		Add("OemPeriod", 0xBE);
		Add(".", 0xBE);
		Add("OemQuestion", 0xBF);

		return names;
	}

	private static Dictionary<uint, string> BuildDisplayNames()
	{
		Dictionary<uint, string> display = [];

		void Add(uint code, string name) => display[code] = name;

		for (uint code = 'A'; code <= 'Z'; code++)
		{
			Add(code, ((char)code).ToString());
		}

		for (uint digit = 0; digit <= 9; digit++)
		{
			Add(0x30 + digit, digit.ToString());
			Add(0x60 + digit, $"NumPad{digit}");
		}

		for (uint index = 1; index <= 24; index++)
		{
			Add(0x6F + index, $"F{index}");
		}

		Add(0x6A, "Multiply");
		Add(0x6B, "Add");
		Add(0x6D, "Subtract");
		Add(0x6E, "Decimal");
		Add(0x6F, "Divide");

		Add(0x25, "Left");
		Add(0x26, "Up");
		Add(0x27, "Right");
		Add(0x28, "Down");
		Add(0x24, "Home");
		Add(0x23, "End");
		Add(0x21, "PageUp");
		Add(0x22, "PageDown");
		Add(0x2D, "Insert");
		Add(0x2E, "Delete");

		Add(0x20, "Space");
		Add(0x0D, "Enter");
		Add(0x09, "Tab");
		Add(0x1B, "Escape");
		Add(0x08, "Backspace");

		Add(0x13, "Pause");
		Add(0x91, "ScrollLock");
		Add(0x2C, "PrintScreen");

		Add(0xC0, "OemTilde");
		Add(0xBD, "OemMinus");
		Add(0xBB, "OemPlus");
		Add(0xDB, "OemOpenBrackets");
		Add(0xDD, "OemCloseBrackets");
		Add(0xDC, "OemBackslash");
		Add(0xBA, "OemSemicolon");
		Add(0xDE, "OemQuotes");
		Add(0xBC, "OemComma");
		Add(0xBE, "OemPeriod");
		Add(0xBF, "OemQuestion");

		return display;
	}
}
