﻿using Business.Extensions;
using Business.Service.EntityServices.Interfaces;
using chess.v4.engine.interfaces;
using chess.v4.engine.reference;
using chess.v4.models;
using Common.IO;
using Data.Dapper.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models.Entities;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tests.Models;

namespace Tests {
	[TestClass]
	public class PlayTests : TestBase {
		public IGameService GameService { get; }
		public IGameStateService GameStateService { get; }
		public IPGNFileService PGNFileService { get; }
		public IPGNService PGNService { get; }
		private Regex endgamePattern { get; } = new Regex(@"\d\-\d");

		public PlayTests() {
			this.GameStateService = this.ServiceProvider.GetService<IGameStateService>();
			this.PGNService = this.ServiceProvider.GetService<IPGNService>();
			this.GameService = this.ServiceProvider.GetService<IGameService>();
			this.PGNFileService = this.ServiceProvider.GetService<IPGNFileService>();
		}

		[TestMethod]
		public async Task PlayAGameFromTheDatabase_ArriveAtSameResult() {
			var pageInfo = new PageInfo { PageSize = 1 };
			var gamesResult = await this.GameService.ReadAll(pageInfo);
			var game = gamesResult.Data.First();
			await playGame_ArriveAtSameresult(game);
		}

		[TestMethod]
		public async Task PlayAllGamesFromTheDatabase_ArriveAtSameResult() {
			var pageInfo = new PageInfo { PageSize = 10000 };
			var gamesResult = await this.GameService.ReadAll(pageInfo);
			foreach (var game in gamesResult.Data) {
				await playGame_ArriveAtSameresult(game);
			}
		}

		private async Task playGame_ArriveAtSameresult(Game game) {
			Assert.IsNotNull(game);
			var gameString = game.GameToString();
			FileUtility.WriteFile<PlayTests>("playGame.pgn", "Output", gameString);
			var result = gameString.Split(" ").Last();
			var gameStateResult = this.GameStateService.Initialize();
			var gameState = gameStateResult.Result;
			game.FEN = GeneralReference.Starting_FEN_Position;

			var moveCount = 1;
			var hasCheckmate = gameString.Split("\r\n\r\n")[1].Contains('#');
			var isDraw = game.Result == "1/2-1/2";
			var finalMove = string.Empty;

			var gameData = this.PGNFileService.ParsePGNData(gameString);
			var count = gameData.Moves.Count();
			foreach (var move in gameData.Moves) {
				if (endgamePattern.Matches(move.Value).Any()) {
					continue;
				}
				if (moveCount == count) {
					finalMove = move.Value;
				}
				try {
					gameState = playMove(gameState, game, move.Value, moveCount);
				} catch (System.Exception ex) {
					Assert.IsTrue(false, $"{ ex.Message }\r\nGame engine failed to play a PGN move. Move: { move.Value }.\r\n{ game.FEN }\r\n{ gameString }");
				}
				moveCount++;
			}
			//I wanted to make more assertions around whether or not the game was a draw, 
			//but the engine doesn't currently recognize a draw because it 
			//is an agreement between players, not a game state.
			if (isDraw) {
				Assert.IsFalse(gameState.StateInfo.IsCheckmate, $"Game should not be marked as checkmate. This game has ended in a draw. Final move was { finalMove }.\r\n{ game.FEN }\r\n{ gameString }");
			}
			if (hasCheckmate) {
				Assert.IsTrue(gameState.StateInfo.IsCheckmate, $"Game should be marked as checkmate. Final move was { finalMove }.\r\n{ game.FEN }\r\n{ gameString }");
				Assert.AreEqual(game.Result, gameState.StateInfo.Result, $"Game Result should be the same.\r\n{ game.FEN }\r\n{ gameString }");
			} else {
				Assert.IsFalse(gameState.StateInfo.IsCheckmate, $"Game should not be marked as checkmate. This game must have ended in a resignation or a draw. Final move was { finalMove }.\r\n{ game.FEN }\r\n{ gameString }");
			}
			game.FEN = gameState.ToString();
			//so we don't run this test again
			game.IsActive = false;
			var updateResult = await this.GameService.Update(game);
			Assert.IsTrue(updateResult.Success, updateResult.Message);
		}

		private GameState playMove(GameState gameState, Game game, string move, int moveCount) {
			var xs = move.Split('.')[1].Split(' ');
			var a = xs[0];
			if (endgamePattern.Matches(a).Any()) {
				return (gameState);
			}
			var gameStateResult = this.GameStateService.MakeMove(gameState, a);
			Assert.IsTrue(gameStateResult.Success, $"Move should have been successful. { a } | { game.FEN }");
			//record and save the FEN at every step so I can figure out where things went wrong.
			game.FEN = gameStateResult.Result.ToString();
			if (xs.Length == 1) {
				return (gameStateResult.Result);
			}
			var b = xs[1];
			if (string.IsNullOrEmpty(b)) {
				return (gameStateResult.Result);
			}
			if (endgamePattern.Matches(b).Any()) {
				return (gameStateResult.Result);
			}
			if (moveCount == 24) {
				var test = "";
			}
			gameStateResult = this.GameStateService.MakeMove(gameStateResult.Result, b);
			Assert.IsTrue(gameStateResult.Success, $"Move should have been successful. { b } | { game.FEN } \r\n{ gameStateResult.Message }");
			//record and save the FEN at every step so I can figure out where things went wrong.
			game.FEN = gameStateResult.Result.ToString();
			return gameStateResult.Result;
		}
	}
}