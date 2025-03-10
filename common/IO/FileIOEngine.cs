namespace common.IO;

using System.Text.Json;

public static class FileIOEngine
{
	private static string _currentDirectory;

	public static string CurrentDirectory
	{
		get
		{
			if (string.IsNullOrEmpty(_currentDirectory))
			{
				_currentDirectory = Directory.GetCurrentDirectory();
			}

			return _currentDirectory;
		}
	}

	/// <summary>
	/// This method makes reading from the bin easier
	/// </summary>
	/// <param name="relativePath"></param>
	/// <returns></returns>
	public static string ReadAllText(string relativePath)
	{
		return File.ReadAllText($"{CurrentDirectory}\\{relativePath}");
	}

	/// <summary>
	/// This method makes writing to files in the bin easier
	/// </summary>
	/// <param name="relativePath"></param>
	/// <returns></returns>
	public static void WriteAllText(string relativePath, string content)
	{
		File.WriteAllText($"{CurrentDirectory}\\{relativePath}", content);
	}

	public static T? ReadJSONConfig<T>(string relativePath, string node, bool errorIfEmptyNode = false)
		where T : class
	{
		var fileContents = ReadAllText(relativePath);
		if (string.IsNullOrEmpty(fileContents))
		{
			throw new Exception("Config file is empty!");
		}

		if (string.IsNullOrEmpty(node))
		{
			return JsonSerializer.Deserialize<T>(fileContents);
		}

		return JsonSerializer.Deserialize<T>(fileContents);
	}
}