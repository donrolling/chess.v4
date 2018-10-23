using chess.v4.engine.interfaces;
using chess.v4.engine.model;
using chess.v4.engine.reference;
using chess.v4.engine.service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace tests.utility {
	internal class TestUtility {
		internal static GameState GetGameState(IGameStateService gameStateService, string fen = "") {
			if (string.IsNullOrEmpty(fen)) {
				fen = GeneralReference.Starting_FEN_Position;
			}
			var gamestateResult = gameStateService.Initialize(fen);
			Assert.IsTrue(gamestateResult.Success);
			var gamestate = gamestateResult.Result;
			return gamestate;
		}
	}
}
