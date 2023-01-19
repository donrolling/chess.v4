using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace Common.IO
{
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

		public static T ReadJSONConfig<T>(string relativePath, string node, bool errorIfEmptyNode = false) where T : class
		{
			var fileContents = ReadAllText(relativePath);
			if (string.IsNullOrEmpty(fileContents))
			{
				throw new Exception("Config file is empty!");
			}
			if (string.IsNullOrEmpty(node))
			{
				return JsonConvert.DeserializeObject<T>(fileContents);
			}
			var jobject = JObject.Parse(fileContents);
			if (jobject[node] == null || !jobject[node].HasValues)
			{
				if (errorIfEmptyNode)
				{
					throw new Exception($"Config file node: {node} is empty!");
				}
			}
			var nodeContents = jobject[node].ToString();
			return JsonConvert.DeserializeObject<T>(nodeContents);
		}
	}
}