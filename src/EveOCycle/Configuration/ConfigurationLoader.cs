using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;
using EveOCycle.Hotkeys;

namespace EveOCycle.Configuration;

internal sealed class ConfigurationException(string summary, IReadOnlyList<string> problems) : Exception(summary)
{
	public IReadOnlyList<string> Problems { get; } = problems;
}

internal static class ConfigurationLoader
{
	public const string DefaultFileName = "EVE-O Cycle.json";

	private const string SampleResourceName = "EveOCycle.SampleConfiguration.json";

	// A pattern that takes longer than this to decide is a pattern worth rewriting
	private static readonly TimeSpan MatchTimeout = TimeSpan.FromMilliseconds(250);

	private static readonly JsonSerializerOptions SerializerOptions = new()
	{
		PropertyNameCaseInsensitive = true,
		ReadCommentHandling = JsonCommentHandling.Skip,
		AllowTrailingCommas = true,
		Converters = { new StringOrArrayConverter() }
	};

	/// <summary>
	/// Reads and validates the configuration file.
	/// </summary>
	/// <exception cref="ConfigurationException">
	/// Thrown with every problem found, rather than only the first one.
	/// </exception>
	public static AppConfiguration Load(string path)
	{
		string text;
		try
		{
			text = File.ReadAllText(path);
		}
		catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
		{
			throw new ConfigurationException($"{path} could not be read", [exception.Message]);
		}

		ConfigurationFile? file;
		try
		{
			file = JsonSerializer.Deserialize<ConfigurationFile>(text, ConfigurationLoader.SerializerOptions);
		}
		catch (JsonException exception)
		{
			string location = exception.LineNumber.HasValue ? $"line {exception.LineNumber + 1}: " : string.Empty;
			throw new ConfigurationException($"{path} is not valid JSON", [location + exception.Message]);
		}

		if (file is null)
		{
			throw new ConfigurationException($"{path} is empty", ["the file contains no configuration"]);
		}

		return ConfigurationLoader.Validate(file, path);
	}

	/// <summary>Writes the built in sample configuration to the given path.</summary>
	public static void WriteSample(string path)
	{
		using Stream? resource = Assembly.GetExecutingAssembly().GetManifestResourceStream(ConfigurationLoader.SampleResourceName)
			?? throw new InvalidOperationException("the sample configuration is missing from this build");
		using FileStream target = File.Create(path);

		resource.CopyTo(target);
	}

	private static AppConfiguration Validate(ConfigurationFile file, string path)
	{
		List<string> problems = [];

		Dictionary<string, ClientDefinition> clients = new(StringComparer.OrdinalIgnoreCase);
		List<ClientDefinition> clientsInOrder = [];

		ClientSection[] clientSections = file.Clients ?? [];
		for (int index = 0; index < clientSections.Length; index++)
		{
			ClientSection section = clientSections[index];
			string where = $"Clients[{index}]";

			string name = (section.Name ?? string.Empty).Trim();
			if (name.Length == 0)
			{
				problems.Add($"{where}: every client needs a Name");
				continue;
			}

			where = $"client '{name}'";

			if (clients.ContainsKey(name))
			{
				problems.Add($"{where}: the name is used more than once");
				continue;
			}

			if (string.IsNullOrWhiteSpace(section.Match))
			{
				problems.Add($"{where}: needs a Match pattern to find its window by");
				continue;
			}

			Regex? match = null;
			try
			{
				match = new Regex(section.Match, RegexOptions.CultureInvariant, ConfigurationLoader.MatchTimeout);
			}
			catch (ArgumentException exception)
			{
				problems.Add($"{where}: Match is not a valid regular expression - {exception.Message}");
			}

			WindowRect? window = null;
			if (!string.IsNullOrWhiteSpace(section.Window))
			{
				if (!WindowRect.TryParse(section.Window, out window, out string windowError))
				{
					problems.Add($"{where}: Window - {windowError}");
				}
			}

			IReadOnlyList<Hotkey> hotkeys = ConfigurationLoader.ParseHotkeys(section.Hotkey, $"{where}: Hotkey", problems);

			if (match is null)
			{
				continue;
			}

			ClientDefinition definition = new()
			{
				Name = name,
				Match = match,
				Hotkeys = hotkeys,
				Window = window
			};

			clients[name] = definition;
			clientsInOrder.Add(definition);
		}

		List<GroupDefinition> groups = [];

		GroupSection[] groupSections = file.Groups ?? [];
		for (int index = 0; index < groupSections.Length; index++)
		{
			GroupSection section = groupSections[index];
			string name = (section.Name ?? string.Empty).Trim();
			string where = name.Length == 0 ? $"Groups[{index}]" : $"group '{name}'";

			if (name.Length == 0)
			{
				problems.Add($"{where}: every group needs a Name");
				continue;
			}

			List<ClientDefinition> members = [];
			foreach (string member in section.Members ?? [])
			{
				if (clients.TryGetValue(member.Trim(), out ClientDefinition? definition))
				{
					if (members.Contains(definition))
					{
						problems.Add($"{where}: '{member}' is listed twice");
						continue;
					}

					members.Add(definition);
				}
				else
				{
					problems.Add($"{where}: '{member}' is not one of the clients defined above");
				}
			}

			if (members.Count == 0)
			{
				problems.Add($"{where}: has no members, so there is nothing to cycle through");
			}

			IReadOnlyList<Hotkey> next = ConfigurationLoader.ParseHotkeys(section.Next, $"{where}: Next", problems);
			IReadOnlyList<Hotkey> previous = ConfigurationLoader.ParseHotkeys(section.Previous, $"{where}: Previous", problems);

			if ((next.Count == 0) && (previous.Count == 0))
			{
				problems.Add($"{where}: has neither Next nor Previous keys, so it can never be used");
			}

			groups.Add(new GroupDefinition
			{
				Name = name,
				Members = members,
				NextHotkeys = next,
				PreviousHotkeys = previous,
				RestackOnWrap = section.RestackOnWrap ?? false
			});
		}

		IReadOnlyList<Hotkey> arrange = ConfigurationLoader.ParseHotkeys(file.ArrangeHotkeys, "ArrangeHotkeys", problems);

		int delay = file.ActivationDelayMilliseconds ?? 30;
		if (delay is < 0 or > 1000)
		{
			problems.Add($"ActivationDelayMilliseconds: {delay} is outside the sensible range of 0 to 1000");
		}

		if (clientsInOrder.Count == 0)
		{
			problems.Add("Clients: no clients are defined, so there is nothing for the application to do");
		}

		if ((groups.Count == 0) && !clientsInOrder.Any(client => client.Hotkeys.Count > 0) && (arrange.Count == 0))
		{
			problems.Add("no groups, client hotkeys or arrange keys are defined, so no key would ever do anything");
		}

		if (problems.Count > 0)
		{
			throw new ConfigurationException($"{path} cannot be used as it stands", problems);
		}

		return new AppConfiguration
		{
			ProcessName = string.IsNullOrWhiteSpace(file.ProcessName) ? "ExeFile" : file.ProcessName.Trim(),
			ActivationDelayMilliseconds = delay,
			ArrangeHotkeys = arrange,
			Clients = clientsInOrder,
			Groups = groups
		};
	}

	private static IReadOnlyList<Hotkey> ParseHotkeys(string[]? values, string where, List<string> problems)
	{
		List<Hotkey> hotkeys = [];

		foreach (string value in values ?? [])
		{
			if (Hotkey.TryParse(value, out Hotkey hotkey, out string error))
			{
				if (!hotkeys.Contains(hotkey))
				{
					hotkeys.Add(hotkey);
				}
			}
			else
			{
				problems.Add($"{where}: '{value}' - {error}");
			}
		}

		return hotkeys;
	}
}
