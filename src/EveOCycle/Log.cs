namespace EveOCycle;

/// <summary>
/// The application's only output. Everything it does is announced on the console,
/// since there is no interface to look at.
/// </summary>
internal static class Log
{
	private static readonly object SyncRoot = new();

	public static bool VerboseEnabled { get; set; }

	public static void Info(string message) => Log.Write(message, null);

	public static void Detail(string message) => Log.Write(message, ConsoleColor.DarkGray);

	public static void Verbose(string message)
	{
		if (Log.VerboseEnabled)
		{
			Log.Write(message, ConsoleColor.DarkGray);
		}
	}

	public static void Warn(string message) => Log.Write($"warning: {message}", ConsoleColor.Yellow);

	public static void Error(string message) => Log.Write($"error: {message}", ConsoleColor.Red);

	/// <summary>Writes a line with no timestamp, for banners and summaries.</summary>
	public static void Plain(string message = "")
	{
		lock (Log.SyncRoot)
		{
			Console.WriteLine(message);
		}
	}

	private static void Write(string message, ConsoleColor? colour)
	{
		lock (Log.SyncRoot)
		{
			ConsoleColor original = Console.ForegroundColor;

			if (colour.HasValue)
			{
				Console.ForegroundColor = colour.Value;
			}

			Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {message}");

			if (colour.HasValue)
			{
				Console.ForegroundColor = original;
			}
		}
	}
}
