﻿
using chess.v4.models.enumeration;
using chess.v4.engine.interfaces;
using chess.v4.models;
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
		private const string testData = @"\data\Tests\";
		private const string removeComments = @"\{[\w\d\s.]*\}\s\d*...\s";
		private const string saveDirectory = @"\data\output\";

		public PGNFileService() {
			this.CurrentDirectory = Environment.CurrentDirectory;
		}

		public string ConvertToPGN(GameMetaData metaData) {
			var sb = GetGameHeader(metaData);

			int i = 1;
			foreach (var move in metaData.Moves) {
				sb.Append(move.Value + " ");
				if (i % 7 == 0) {
					sb.Append("\n");
				}
				i++;
			}

			return sb.ToString();
		}

		public void DisplayPGNFileHeader(GameMetaData metaData) {
			var pgnFileHeader = GetPGNFileHeader(metaData);
			Console.WriteLine($"{ pgnFileHeader }\r\n");
		}

		public string GetPGNFileHeader(GameMetaData metaData) {
			var sb = new StringBuilder();
			sb.AppendLine($"[Event \"{ metaData.Event }\"]");
			sb.AppendLine($"[Site \"{ metaData.Site }\"]");
			sb.AppendLine($"[Date \"{ metaData.Date }\"]");
			sb.AppendLine($"[Round \"{ metaData.Round }\"]");
			sb.AppendLine($"[White \"{ metaData.White }\"]");
			sb.AppendLine($"[Black \"{ metaData.Black }\"]");
			sb.AppendLine($"[Result \"{ metaData.Result }\"]");
			sb.AppendLine($"[WhiteELO \"{ metaData.WhiteELO }\"]");
			sb.AppendLine($"[BlackELO \"{ metaData.BlackELO }\"]");
			sb.AppendLine($"[ECO \"{ metaData.ECO }\"]");
			return sb.ToString();
		}

		public string GetBaseEnvironmentPath(string directory) {
			string basePath = CurrentDirectory;
			string path = string.Concat(basePath, directory);
			return path;
		}

		public StringBuilder GetGameHeader(GameMetaData gameMetaData) {
			var sb = new StringBuilder();
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
			return string.Concat(GetBaseEnvironmentPath(testData), filename);
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
			var moveNode = new Regex(movePattern, RegexOptions.Multiline);
			var moveMatch = moveNode.Match(modInput);

			while (moveMatch.Success) {
				var group = moveMatch.Groups[0];
				var captureCollection = group.Captures;
				for (int j = 0; j < captureCollection.Count; j++) {
					var c = captureCollection[j];
					var move = c.Value;

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
			var fileContents = string.Empty;
			using (var textFile = new StreamReader(fullpath)) {
				fileContents = textFile.ReadToEnd();
			}
			return ParsePGNData(fileContents);
		}

		public void SaveFile(string filename, string data) {
			var di = new DirectoryInfo(CurrentDirectory);
			var basePath = di.Parent.FullName;
			if (di.Parent.Name == "bin") {
				basePath = di.Parent.Parent.FullName;
			}
			var savePath = string.Concat(basePath, @"\", saveDirectory, @"\", filename);
			using (var textFile = new StreamWriter(savePath)) {
				textFile.Write(data);
			}
		}
	}
}