using EveOCycle.Configuration;

namespace EveOCycle.Clients;

/// <summary>
/// Everything a key press can do: step through a group, jump straight to one client,
/// or put every window back where it belongs.
/// </summary>
internal sealed class CycleController(AppConfiguration configuration, ClientLocator locator)
{
	private readonly AppConfiguration _configuration = configuration;
	private readonly ClientLocator _locator = locator;

	// Where each group was left, so that a cycle started while some other application is
	// in the foreground carries on from where it got to rather than jumping back to the start
	private readonly Dictionary<string, ClientDefinition> _lastActivated = new(StringComparer.OrdinalIgnoreCase);

	public void Cycle(GroupDefinition group, bool forwards)
	{
		List<RunningClient> members = this.GetRunningMembers(group);

		if (members.Count == 0)
		{
			Log.Info($"{group.Name}: none of its clients are running");
			return;
		}

		int current = this.FindCurrentIndex(group, members);

		int target;
		bool wrapped;

		if (current < 0)
		{
			// Nothing in this group is in front, so start at whichever end we are heading from
			target = forwards ? 0 : members.Count - 1;
			wrapped = false;
		}
		else
		{
			target = (current + (forwards ? 1 : -1) + members.Count) % members.Count;
			wrapped = forwards ? (target <= current) : (target >= current);
		}

		if (wrapped && group.RestackOnWrap && (members.Count > 1))
		{
			this.Restack(group, members, members[target], forwards);
		}

		this.Activate(members[target], group.Name);
		this._lastActivated[group.Name] = members[target].Definition;
	}

	public void Select(ClientDefinition client)
	{
		RunningClient? running = this._locator.GetRunningClients().FirstOrDefault(candidate => candidate.Definition == client);

		if (running is null)
		{
			Log.Info($"{client.Name} is not running");
			return;
		}

		this.Activate(running, "select");

		// Carry on from here the next time one of its groups is cycled
		foreach (GroupDefinition group in this._configuration.Groups.Where(group => group.Members.Contains(client)))
		{
			this._lastActivated[group.Name] = client;
		}
	}

	public void ArrangeAll()
	{
		int moved = 0;
		int skipped = 0;

		foreach (RunningClient client in this._locator.GetRunningClients())
		{
			if (client.Definition.Window is null)
			{
				skipped++;
				continue;
			}

			if (WindowController.ApplyWindow(client.Handle, client.Definition.Window))
			{
				moved++;
			}
		}

		Log.Info($"arrange: moved {moved} window(s)"
				+ (skipped > 0 ? $", {skipped} have no Window set" : string.Empty));
	}

	private List<RunningClient> GetRunningMembers(GroupDefinition group)
	{
		IReadOnlyList<RunningClient> running = this._locator.GetRunningClients();
		List<RunningClient> members = [];

		// Kept in the order the group lists them, which is the cycle order
		foreach (ClientDefinition definition in group.Members)
		{
			RunningClient? client = running.FirstOrDefault(candidate => candidate.Definition == definition);

			if (client is not null)
			{
				members.Add(client);
			}
		}

		return members;
	}

	private int FindCurrentIndex(GroupDefinition group, List<RunningClient> members)
	{
		nint foreground = WindowController.GetForegroundWindow();
		int index = members.FindIndex(member => member.Handle == foreground);

		if (index >= 0)
		{
			return index;
		}

		// Not looking at this group right now - pick up where it was left instead
		return this._lastActivated.TryGetValue(group.Name, out ClientDefinition? last)
				? members.FindIndex(member => member.Definition == last)
				: -1;
	}

	/// <summary>
	/// Brings every other member forward in turn, so that once the target is activated the
	/// windows are stacked in cycle order behind it.
	/// </summary>
	private void Restack(GroupDefinition group, List<RunningClient> members, RunningClient target, bool forwards)
	{
		List<RunningClient> others = members.Where(member => member != target).ToList();

		if (forwards)
		{
			others.Reverse();
		}

		Log.Verbose($"{group.Name}: restacking {others.Count} window(s)");

		foreach (RunningClient other in others)
		{
			this.Activate(other, group.Name, announce: false);

			// Activations that come too quickly after one another get dropped
			if (this._configuration.ActivationDelayMilliseconds > 0)
			{
				Thread.Sleep(this._configuration.ActivationDelayMilliseconds);
			}
		}
	}

	private void Activate(RunningClient client, string context, bool announce = true)
	{
		WindowController.Activate(client.Handle);

		if (client.Definition.Window is not null)
		{
			WindowController.ApplyWindow(client.Handle, client.Definition.Window);
		}

		if (announce)
		{
			Log.Info($"{context}: {client.Definition.Name}  ({client.Title})");
		}
	}
}
