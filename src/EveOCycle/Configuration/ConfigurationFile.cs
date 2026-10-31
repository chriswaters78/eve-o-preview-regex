using System.Text.Json;
using System.Text.Json.Serialization;

namespace EveOCycle.Configuration;

/// <summary>
/// The configuration file exactly as it is written on disk. Everything is optional and
/// nothing is interpreted here - <see cref="ConfigurationLoader" /> turns this into the
/// validated model the rest of the application uses, and reports whatever is wrong.
/// </summary>
internal sealed class ConfigurationFile
{
	public string? ProcessName { get; set; }

	public int? ActivationDelayMilliseconds { get; set; }

	public string[]? ArrangeHotkeys { get; set; }

	public ClientSection[]? Clients { get; set; }

	public GroupSection[]? Groups { get; set; }
}

internal sealed class ClientSection
{
	public string? Name { get; set; }

	public string? Match { get; set; }

	public string[]? Hotkey { get; set; }

	public string? Window { get; set; }
}

internal sealed class GroupSection
{
	public string? Name { get; set; }

	public string[]? Next { get; set; }

	public string[]? Previous { get; set; }

	public string[]? Members { get; set; }

	public bool? RestackOnWrap { get; set; }
}

/// <summary>
/// Lets any list of hotkeys be written either as one key - "Multiply" - or as a list
/// of keys that all do the same thing - [ "Multiply", "Control+Multiply" ].
/// </summary>
internal sealed class StringOrArrayConverter : JsonConverter<string[]>
{
	public override string[]? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (reader.TokenType == JsonTokenType.Null)
		{
			return null;
		}

		if (reader.TokenType == JsonTokenType.String)
		{
			string? single = reader.GetString();
			return single is null ? null : [single];
		}

		if (reader.TokenType != JsonTokenType.StartArray)
		{
			throw new JsonException("expected either a single key or a list of keys");
		}

		List<string> values = [];

		while (reader.Read() && (reader.TokenType != JsonTokenType.EndArray))
		{
			if (reader.TokenType != JsonTokenType.String)
			{
				throw new JsonException("expected a key name");
			}

			string? value = reader.GetString();
			if (value is not null)
			{
				values.Add(value);
			}
		}

		return [.. values];
	}

	public override void Write(Utf8JsonWriter writer, string[] value, JsonSerializerOptions options)
	{
		writer.WriteStartArray();

		foreach (string item in value)
		{
			writer.WriteStringValue(item);
		}

		writer.WriteEndArray();
	}
}
