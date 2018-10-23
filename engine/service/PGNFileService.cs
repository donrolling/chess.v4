using chess.v4.engine.enumeration;
using chess.v4.engine.interfaces;
using chess.v4.engine.model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace chess.v4.engine.service {

	public class PGNFileService : IPGNFileService {
		public string CurrentDirectory { get; }
		private const string movePattern = @"\d*[.]((\s)?|[\r\n]|\n)[\w\d-\/+]*(\s|[\r\n]|\n)[\w\d-\/+]*";
		private const string nodePattern = @"(?<=\[)(.*?)(?=\])";
		private const string PGNDirectory = @"\PGNFiles\";
		private const string removeComments = @"\{[\w\d\s.]*\}\s\d*...\s";
		private const string SaveDirectory = @"\SavedTests\";

		public PGNFileService() {
			this.CurrentDirectory = Environment.CurrentDirectory;
		}

		public string ConvertToPGN(GameMetaData metaData) {
			StringBuilder sb = GetGameHeader(metaData);

			int i = 1;
			foreach (KeyValuePair<int, string> move in metaData.Moves) {
				sb.Append(move.Value + " ");
				if (i % 7 == 0) {
					sb.Append("\n");
				}
				i++;
			}

			return sb.ToString();
		}

		public void DisplayPGNFileHeader(GameMetaData metaData) {
			Console.WriteLine("[Event \"" + metaData.Event + "\"]");
			Console.WriteLine("[Site \"" + metaData.Site + "\"]");
			Console.WriteLine("[Date \"" + metaData.Date + "\"]");
			Console.WriteLine("[Round \"" + metaData.Round + "\"]");
			Console.WriteLine("[White \"" + metaData.White + "\"]");
			Console.WriteLine("[Black \"" + metaData.Black + "\"]");
			Console.WriteLine("[Result \"" + metaData.Result + "\"]");
			Console.WriteLine("[WhiteELO \"" + metaData.WhiteELO + "\"]");
			Console.WriteLine("[BlackELO \"" + metaData.BlackELO + "\"]");
			Console.WriteLine("[ECO \"" + metaData.ECO + "\"]");

			Console.WriteLine("");
		}

		public string GetBaseEnvironmentPath(string directory) {
			string basePath = CurrentDirectory;
			string path = string.Concat(basePath, directory);
			return path;
		}

		public StringBuilder GetGameHeader(GameMetaData gameMetaData) {
			StringBuilder sb = new StringBuilder();
			//write meta data
			var types = Enum.GetNames(typeof(MetaType));
			foreach (var type in types) {
				string value = gameMetaData.GetValue(type);
				string headerLine = string.Concat("[", type, " \"", value + "\"]");
				sb.AppendLine(headerLine);
			}
			sb.AppendLine("");

			return sb;
		}

		public string GetPGNFilePath(string filename) {
			string path = string.Concat(GetBaseEnvironmentPath(PGNDirectory), filename);
			return path;
		}

		public GameMetaData ParsePGNData(string input) {
			var metaData = new GameMetaData();

			var findNode = new Regex(nodePattern, RegexOptions.IgnoreCase);
			var nodeMatch = findNode.Match(input);

			var names = Enum.GetNames(typeof(MetaType));
			while (nodeMatch.Success) {
				for (int i = 1; i <= 2; i++) {
					var group = nodeMatch.Groups[i];
					var captureCollection = group.Captures;
					for (int j = 0; j < captureCollection.Count; j++) {
						var capture = captureCollection[j];

						var label = names.Where(n => capture.Value.Split(' ')[0] == n).FirstOrDefault();

						if (!string.IsNullOrEmpty(label)) {
							var metaType = (MetaType)Enum.Parse(typeof(MetaType), label);
							var value = capture.Value.Replace("\"", "").Replace(label, "").Trim();
							metaData.SetValue(metaType, value);
						}
					}
				}
				nodeMatch = nodeMatch.NextMatch();
			}

			int moveNum = 1;
			var modInput = Regex.Replace(input, removeComments, string.Empty);
			modInput = Regex.Replace(modInput, @"\r\n", " ");

			//so far this messes up with comments
			Regex moveNode = new Regex(movePattern, RegexOptions.Multiline);
			Match moveMatch = moveNode.Match(modInput);

			while (moveMatch.Success) {
				Group group = moveMatch.Groups[0];
				CaptureCollection captureCollection = group.Captures;
				for (int j = 0; j < captureCollection.Count; j++) {
					Capture c = captureCollection[j];
					string move = c.Value;

					if (!string.IsNullOrEmpty(move)) {
						metaData.Moves.Add(moveNum, move);
						moveNum++;
					}
				}
				moveMatch = moveMatch.NextMatch();
			}

			return metaData;
		}

		public GameMetaData ReadPGNFromFile(string fullpath) {
			string fileContents = string.Empty;
			using (StreamReader textFile = new StreamReader(fullpath)) {
				fileContents = textFile.ReadToEnd();
			}
			return ParsePGNData(fileContents);
		}

		public void SaveFile(string filename, string data) {
			DirectoryInfo di = new DirectoryInfo(CurrentDirectory);
			string basePath = di.Parent.FullName;
			if (di.Parent.Name == "bin") {
				basePath = di.Parent.Parent.FullName;
			}
			string savePath = string.Concat(basePath, @"\", SaveDirectory, @"\", filename);
			using (StreamWriter textFile = new StreamWriter(savePath)) {
				textFile.Write(data);
			}
		}
	}
}