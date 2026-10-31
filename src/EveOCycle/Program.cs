using EveOCycle.Clients;
using EveOCycle.Configuration;
using EveOCycle.Hotkeys;

namespace EveOCycle;

internal static class Program
{
	private const int ExitOk = 0;
	private const int ExitConfigurationProblem = 1;
	private const int ExitConfigurationWritten = 2;

	private static int Main(string[] args)
	{
		if (!Options.TryParse(args, out Options options, out string argumentError))
		{
			Log.Error(argumentError);
			Log.Plain();
			Program.PrintUsage();
			return Program.ExitConfigurationProblem;
		}

		if (options.ShowHelp)
		{
			Program.PrintUsage();
			return Program.ExitOk;
		}

		Log.VerboseEnabled = options.Verbose;

		string path = options.ConfigurationPath ?? Program.FindConfigurationFile();

		if (!File.Exists(path))
		{
			return Program.WriteStarterConfiguration(path);
		}

		AppConfiguration configuration;
		try
		{
			configuration = ConfigurationLoader.Load(path);
		}
		catch (ConfigurationException exception)
		{
			Log.Error(exception.Message);
			foreach (string problem in exception.Problems)
			{
				Log.Plain($"  - {problem}");
			}

			return Program.ExitConfigurationProblem;
		}

		Log.Detail($"configuration: {path}");

		ClientLocator locator = new(configuration);

		if (options.ListClients)
		{
			Program.ListClients(configuration, locator);
			return Program.ExitOk;
		}

		Program.PrintSummary(configuration);

		if (options.ValidateOnly)
		{
			Log.Info("the configuration is valid");
			return Program.ExitOk;
		}

		return Program.Run(configuration, locator);
	}

	private static int Run(AppConfiguration configuration, ClientLocator locator)
	{
		CycleController controller = new(configuration, locator);
		MessageLoop loop = new();

		// The registry has to be built on this thread, the one that pumps the messages
		using HotkeyRegistry registry = new();

		int registered = 0;

		foreach (GroupDefinition group in configuration.Groups)
		{
			foreach (Hotkey hotkey in group.NextHotkeys)
			{
				registered += Program.Register(registry, hotkey, $"{group.Name} forwards", () => controller.Cycle(group, forwards: true));
			}

			foreach (Hotkey hotkey in group.PreviousHotkeys)
			{
				registered += Program.Register(registry, hotkey, $"{group.Name} backwards", () => controller.Cycle(group, forwards: false));
			}
		}

		foreach (ClientDefinition client in configuration.Clients)
		{
			foreach (Hotkey hotkey in client.Hotkeys)
			{
				registered += Program.Register(registry, hotkey, $"select {client.Name}", () => controller.Select(client));
			}
		}

		foreach (Hotkey hotkey in configuration.ArrangeHotkeys)
		{
			registered += Program.Register(registry, hotkey, "arrange all windows", controller.ArrangeAll);
		}

		if (registered == 0)
		{
			Log.Error("not one hotkey could be registered, so there is nothing to wait for");
			return Program.ExitConfigurationProblem;
		}

		Console.CancelKeyPress += (_, eventArgs) =>
		{
			eventArgs.Cancel = true;
			Log.Info("stopping");
			loop.Stop();
		};

		Log.Info($"ready - {registered} hotkey(s) registered. Press Ctrl+C to stop.");

		loop.Run(registry);

		Log.Info("hotkeys released");
		return Program.ExitOk;
	}

	private static int Register(HotkeyRegistry registry, Hotkey hotkey, string description, Action action)
	{
		if (registry.TryRegister(hotkey, description, action, out string error))
		{
			Log.Verbose($"{hotkey} -> {description}");
			return 1;
		}

		Log.Warn($"{hotkey} ({description}) could not be registered: {error}");
		return 0;
	}

	private static string FindConfigurationFile()
	{
		string beside = Path.Combine(AppContext.BaseDirectory, ConfigurationLoader.DefaultFileName);

		if (File.Exists(beside))
		{
			return beside;
		}

		string working = Path.Combine(Directory.GetCurrentDirectory(), ConfigurationLoader.DefaultFileName);

		return File.Exists(working) ? working : beside;
	}

	private static int WriteStarterConfiguration(string path)
	{
		Log.Warn($"no configuration file at {path}");

		try
		{
			ConfigurationLoader.WriteSample(path);
		}
		catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
		{
			Log.Error($"a starter configuration could not be written: {exception.Message}");
			return Program.ExitConfigurationProblem;
		}

		Log.Info("a commented starter configuration has been written there");
		Log.Plain();
		Log.Plain("  Edit it to describe your own characters, then run this again.");
		Log.Plain("  Run with --list while your clients are logged in to see their window titles.");

		return Program.ExitConfigurationWritten;
	}

	private static void ListClients(AppConfiguration configuration, ClientLocator locator)
	{
		IReadOnlyList<MonitoredWindow> windows = locator.Survey();

		Log.Plain();
		Log.Plain($"Windows of process '{configuration.ProcessName}':");
		Log.Plain();

		if (windows.Count == 0)
		{
			Log.Plain("  (none are running)");
			Log.Plain();
			return;
		}

		foreach (MonitoredWindow window in windows.OrderBy(window => window.Title, StringComparer.OrdinalIgnoreCase))
		{
			string matched = window.Definition is null ? "no client matches this window" : window.Definition.Name;
			Log.Plain($"  {window.ProcessId,8}  {window.Title,-40}  {matched}");
		}

		Log.Plain();
	}

	private static void PrintSummary(AppConfiguration configuration)
	{
		Log.Plain();

		foreach (GroupDefinition group in configuration.Groups)
		{
			Log.Plain($"Group '{group.Name}'"
					+ (group.RestackOnWrap ? "  (restacks on wrap)" : string.Empty));
			Log.Plain($"  forwards   {Program.Describe(group.NextHotkeys)}");
			Log.Plain($"  backwards  {Program.Describe(group.PreviousHotkeys)}");

			for (int index = 0; index < group.Members.Count; index++)
			{
				ClientDefinition member = group.Members[index];
				Log.Plain($"    {index + 1}. {member.Name,-20} {Program.Describe(member)}");
			}

			Log.Plain();
		}

		ClientDefinition[] ungrouped = [.. configuration.Clients.Where(client => !configuration.Groups.Any(group => group.Members.Contains(client)))];

		if (ungrouped.Length > 0)
		{
			Log.Plain("Clients in no group");
			foreach (ClientDefinition client in ungrouped)
			{
				Log.Plain($"    {client.Name,-23} {Program.Describe(client)}");
			}

			Log.Plain();
		}

		if (configuration.ArrangeHotkeys.Count > 0)
		{
			Log.Plain($"Arrange every window   {Program.Describe(configuration.ArrangeHotkeys)}");
			Log.Plain();
		}
	}

	private static string Describe(ClientDefinition client)
	{
		string window = client.Window is null ? "window left alone" : $"window [{client.Window}]";
		string hotkeys = client.Hotkeys.Count > 0 ? $"   key {Program.Describe(client.Hotkeys)}" : string.Empty;

		return $"{window}{hotkeys}";
	}

	private static string Describe(IReadOnlyList<Hotkey> hotkeys)
	{
		return hotkeys.Count == 0 ? "(no key)" : string.Join(" or ", hotkeys);
	}

	private static void PrintUsage()
	{
		Log.Plain("EVE-O Cycle - hotkey driven client switcher for EVE Online");
		Log.Plain();
		Log.Plain("  EVE-O Cycle [options]");
		Log.Plain();
		Log.Plain("  -c, --config <path>  Configuration file to use.");
		Log.Plain($"                       Defaults to \"{ConfigurationLoader.DefaultFileName}\" next to the executable.");
		Log.Plain("  -l, --list           List the EVE windows running right now, and which client");
		Log.Plain("                       definition each one matches, then exit.");
		Log.Plain("      --validate       Check the configuration and print what it means, then exit.");
		Log.Plain("  -v, --verbose        Report every match and registration.");
		Log.Plain("  -h, --help           Show this text.");
		Log.Plain();
		Log.Plain("  With no options it registers the hotkeys and waits. Ctrl+C stops it.");
		Log.Plain();
	}

	private sealed record Options(string? ConfigurationPath, bool ListClients, bool ValidateOnly, bool Verbose, bool ShowHelp)
	{
		public static bool TryParse(string[] args, out Options options, out string error)
		{
			string? path = null;
			bool list = false;
			bool validate = false;
			bool verbose = false;
			bool help = false;

			error = string.Empty;

			for (int index = 0; index < args.Length; index++)
			{
				switch (args[index].ToLowerInvariant())
				{
					case "-c":
					case "--config":
						if (index + 1 >= args.Length)
						{
							options = default!;
							error = $"{args[index]} needs the path of a configuration file after it";
							return false;
						}

						path = args[++index];
						break;

					case "-l":
					case "--list":
						list = true;
						break;

					case "--validate":
						validate = true;
						break;

					case "-v":
					case "--verbose":
						verbose = true;
						break;

					case "-h":
					case "--help":
					case "-?":
					case "/?":
						help = true;
						break;

					default:
						options = default!;
						error = $"'{args[index]}' is not an option this application knows";
						return false;
				}
			}

			options = new Options(path, list, validate, verbose, help);
			return true;
		}
	}
}
