using System.Diagnostics;
using System.Text.RegularExpressions;
using EveOCycle.Configuration;

namespace EveOCycle.Clients;

/// <summary>A window belonging to a monitored process, and the client it was matched to.</summary>
internal sealed record MonitoredWindow(int ProcessId, nint Handle, string Title, ClientDefinition? Definition);

/// <summary>A running EVE client that one of the configured definitions matched.</summary>
internal sealed record RunningClient(nint Handle, string Title, ClientDefinition Definition);

/// <summary>
/// Finds the EVE clients that are currently running and works out which definition each
/// one belongs to. Windows are looked up on demand rather than polled, so the roster is
/// never stale when a key is pressed.
/// </summary>
internal sealed class ClientLocator(AppConfiguration configuration)
{
	private readonly AppConfiguration _configuration = configuration;

	// Matching a title costs a regular expression pass per definition, so the verdict is
	// kept until that window's title changes
	private readonly Dictionary<nint, CachedMatch> _cache = [];

	private sealed record CachedMatch(string Title, ClientDefinition? Definition);

	/// <summary>Every window of a monitored process, matched or not. Used by --list.</summary>
	public IReadOnlyList<MonitoredWindow> Survey()
	{
		List<MonitoredWindow> windows = [];
		HashSet<nint> seen = [];

		Process[] processes = this._configuration.ProcessName == "*"
								? Process.GetProcesses()
								: Process.GetProcessesByName(this._configuration.ProcessName);

		foreach (Process process in processes)
		{
			using (process)
			{
				nint handle;
				string title;
				int processId;

				try
				{
					if (!this._configuration.MatchesProcess(process.ProcessName))
					{
						continue;
					}

					handle = process.MainWindowHandle;
					if (handle == nint.Zero)
					{
						// Not every process has something on screen
						continue;
					}

					title = process.MainWindowTitle;
					processId = process.Id;
				}
				catch (Exception exception) when (exception is InvalidOperationException or System.ComponentModel.Win32Exception)
				{
					// The process went away, or is one we are not allowed to look at
					continue;
				}

				seen.Add(handle);
				windows.Add(new MonitoredWindow(processId, handle, title, this.Resolve(handle, title)));
			}
		}

		// Forget the windows that have gone
		foreach (nint stale in this._cache.Keys.Where(handle => !seen.Contains(handle)).ToList())
		{
			this._cache.Remove(stale);
		}

		return windows;
	}

	/// <summary>The running clients that a definition matched, in no particular order.</summary>
	public IReadOnlyList<RunningClient> GetRunningClients()
	{
		List<RunningClient> clients = [];
		HashSet<ClientDefinition> claimed = [];

		foreach (MonitoredWindow window in this.Survey())
		{
			if (window.Definition is null)
			{
				continue;
			}

			if (!claimed.Add(window.Definition))
			{
				Log.Warn($"more than one window matches client '{window.Definition.Name}' - ignoring '{window.Title}'");
				continue;
			}

			clients.Add(new RunningClient(window.Handle, window.Title, window.Definition));
		}

		return clients;
	}

	private ClientDefinition? Resolve(nint handle, string title)
	{
		if (this._cache.TryGetValue(handle, out CachedMatch? cached) && (cached.Title == title))
		{
			return cached.Definition;
		}

		ClientDefinition? matched = null;

		foreach (ClientDefinition client in this._configuration.Clients)
		{
			bool isMatch;
			try
			{
				isMatch = client.Match.IsMatch(title);
			}
			catch (RegexMatchTimeoutException)
			{
				Log.Warn($"the Match pattern for client '{client.Name}' took too long against '{title}' and was given up on");
				continue;
			}

			if (!isMatch)
			{
				continue;
			}

			if (matched is null)
			{
				matched = client;
				continue;
			}

			Log.Warn($"'{title}' matches both '{matched.Name}' and '{client.Name}' - using '{matched.Name}'");
		}

		this._cache[handle] = new CachedMatch(title, matched);

		if (matched is not null)
		{
			Log.Verbose($"'{title}' is client '{matched.Name}'");
		}

		return matched;
	}
}
