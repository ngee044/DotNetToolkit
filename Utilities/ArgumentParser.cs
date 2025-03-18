namespace Utilities;

using System.Text.RegularExpressions;
using System.IO;
using System.Reflection;


public sealed  class ArgumentParser
{
	private readonly Dictionary<string, string> arguments_ = new();
	private readonly string program_folder_;
	private readonly string program_name_;

	public ArgumentParser(string[] args)
	{
		var location = Assembly.GetEntryAssembly()?.Location ?? string.Empty;
		program_folder_ = Path.GetDirectoryName(location) ?? string.Empty;
		program_name_ = Path.GetFileName(location);

		arguments_ = parse(args);
	}

	public string program_folder => program_folder_;
	public string program_name => program_name_;

	public string? to_string_arg(string key)
	{
		return arguments_.TryGetValue(key, out var value) ? value : null;
	}

	public string[]? to_array_arg(string key)
	{
		var raw = to_string_arg(key);
		if (raw == null)
		{
			return null;
		}

		return raw.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
	}

	public bool to_bool_arg(string key)
	{
		var raw = to_string_arg(key);
		if (raw == null)
		{
			return false;
		}
		return raw.Equals("true", StringComparison.OrdinalIgnoreCase);
	}

	public short? to_short_arg(string key)
		=> convert_to_number<short>(to_string_arg(key));
	public int? to_int_arg(string key)
		=> convert_to_number<int>(to_string_arg(key));
	public long? to_long_arg(string key)
		=> convert_to_number<long>(to_string_arg(key));
	private static T? convert_to_number<T>(string? raw) where T : struct
	{
		if (raw == null)
		{
			return null;
		}

		try
		{
			object? temp = Convert.ChangeType(raw, typeof(T));
			if (temp is T valid) return valid;
		}
		catch
		{
		}
		return null;
	}

	private Dictionary<string, string> parse(string[] args)
	{
		var result = new Dictionary<string, string>();

		for (int i = 0; i < args.Length; ++i)
		{
			var current = args[i];
			if (!current.StartsWith("--"))
			{
				continue;
			}

			if (current.Equals("--help", StringComparison.OrdinalIgnoreCase))
			{
				result[current] = "display help";
				continue;
			}

			if (current.Equals("--version", StringComparison.OrdinalIgnoreCase))
			{
				result[current] = "display version";
				continue;
			}

			if (i + 1 < args.Length)
			{
				var next = args[i + 1];
				if (!next.StartsWith("--"))
				{
					result[current] = next;
					++i;
				}
			}
		}
		return result;
	}
}
