﻿using chess_engine.Models;

namespace chess_engine_tests.Models
{
	public class PlayGameInfo
	{
		public string FinalMove { get; set; }
		public GameMetaData gameData { get; set; }
		public GameMetaData GameData { get; internal set; }
		public GameState GameState { get; set; }
		public string GameString { get; set; }
		public bool HasCheckmate { get; set; }
		public bool IsDraw { get; set; }
		public int MoveCount { get; set; }
	}
}