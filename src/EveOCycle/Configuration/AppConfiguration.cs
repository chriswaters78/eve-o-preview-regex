using System.Globalization;
using System.Text.RegularExpressions;
using EveOCycle.Hotkeys;

namespace EveOCycle.Configuration;

/// <summary>
/// Where a client's window should sit, in screen pixels. On a desktop with more than
/// one monitor the coordinates run across all of them.
/// </summary>
internal sealed record WindowRect(int X, int Y, int Width, int Height)
{
	public static bool TryParse(string text, out WindowRect? rect, out string error)
	{
		rect = null;
		error = string.Empty;

		string[] parts = text.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

		if (parts.Length != 4)
		{
			error = "expected four numbers, as in \"0, 0, 2560, 1400\"";
			return false;
		}

		int[] values = new int[4];
		for (int index = 0; index < 4; index++)
		{
			if (!int.TryParse(parts[index], NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out values[index]))
			{
				error = $"'{parts[index]}' is not a whole number";
				return false;
			}
		}

		if ((values[2] <= 0) || (values[3] <= 0))
		{
			error = "the width and the height have to be greater than zero";
			return false;
		}

		rect = new WindowRect(values[0], values[1], values[2], values[3]);
		return true;
	}

	public override string ToString()
	{
		return $"{this.X}, {this.Y}, {this.Width}, {this.Height}";
	}
}

/// <summary>One EVE character, and how its client window should be treated.</summary>
internal sealed class ClientDefinition
{
	public required string Name { get; init; }

	/// <summary>Matched against the window title, which reads "EVE - &lt;character&gt;" once logged in.</summary>
	public required Regex Match { get; init; }

	/// <summary>Keys that activate this client directly. May be empty.</summary>
	public required IReadOnlyList<Hotkey> Hotkeys { get; init; }

	/// <summary>Where to put the window when this client is activated, or null to leave it be.</summary>
	public WindowRect? Window { get; init; }
}

/// <summary>An ordered set of clients that one pair of keys cycles through.</summary>
internal sealed class GroupDefinition
{
	public required string Name { get; init; }

	public required IReadOnlyList<ClientDefinition> Members { get; init; }

	public required IReadOnlyList<Hotkey> NextHotkeys { get; init; }

	public required IReadOnlyList<Hotkey> PreviousHotkeys { get; init; }

	/// <summary>
	/// When the cycle rolls over from the last client back to the first, bring every other
	/// member forward in turn so the windows end up stacked in cycle order again. Only
	/// worth turning on when the clients in this group overlap on screen.
	/// </summary>
	public bool RestackOnWrap { get; init; }
}

internal sealed class AppConfiguration
{
	/// <summary>The process to look at. EVE Online runs as "ExeFile". "*" looks at everything.</summary>
	public required string ProcessName { get; init; }

	/// <summary>Pause between the individual window activations of a restack.</summary>
	public required int ActivationDelayMilliseconds { get; init; }

	/// <summary>Keys that put every known client window back where it belongs at once.</summary>
	public required IReadOnlyList<Hotkey> ArrangeHotkeys { get; init; }

	public required IReadOnlyList<ClientDefinition> Clients { get; init; }

	public required IReadOnlyList<GroupDefinition> Groups { get; init; }

	public bool MatchesProcess(string processName)
	{
		return (this.ProcessName == "*")
				|| string.Equals(processName, this.ProcessName, StringComparison.OrdinalIgnoreCase);
	}
}
