using Business.Extensions;
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
	public class PlayGameInfo {
		public string FinalMove { get; set; }
		public GameMetaData gameData { get; set; }
		public GameMetaData GameData { get; internal set; }
		public GameState GameState { get; set; }
		public string GameString { get; set; }
		public bool HasCheckmate { get; set; }
		public bool IsDraw { get; set; }
		public int MoveCount { get; set; }
	}

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

		[TestMethod]
		public async Task PlayAllGamesFromTheDatabase_MarkTheBadOnes_ButPlayTheGoodOnes() {
			var pageInfo = new PageInfo { PageSize = 10000 };
			var gamesResult = await this.GameService.ReadAll(pageInfo);
			foreach (var game in gamesResult.Data) {
				await playGame_ArriveAtSameresult_NoAssertions(game);
			}
		}

		private async Task playGame_ArriveAtSameresult(Game game) {
			var playGameInfo = playGamePrep(game);
			var count = playGameInfo.GameData.Moves.Count();
			var moveCount = 0;
			foreach (var move in playGameInfo.GameData.Moves) {
				moveCount++;
				if (moveCount == count) {
					playGameInfo.FinalMove = move.Value;
				}
				try {
					playGameInfo.GameState = playMove(playGameInfo.GameState, game, move.Value, moveCount);
				} catch (System.Exception ex) {
					Assert.IsTrue(false, $"{ ex.Message }\r\nGame engine failed to play a PGN move. Move: { move.Key }. { move.Value }\r\n{ game.FEN }\r\n{ playGameInfo.GameString }");
				}
				if (moveCount != count) {
					//check to see if we're in checkmate as long as this isn't the last move.
					Assert.IsFalse(playGameInfo.GameState.StateInfo.IsCheckmate, $"The engine thinks this is checkmate, though it is not. Move: { move.Key }. { move.Value }\r\n{ game.FEN }\r\n{ playGameInfo.GameString }");
				}
			}
			//I wanted to make more assertions around whether or not the game was a draw,
			//but the engine doesn't currently recognize a draw because it
			//is an agreement between players, not a game state.
			if (playGameInfo.IsDraw) {
				//right now this is failing sometimes because for example, on game #16, there is a pawn move that
				//will capture the queen that is checking the king, but the IsCheckmate calculation doesn't
				//understand
				Assert.IsFalse(playGameInfo.GameState.StateInfo.IsCheckmate, $"Game should not be marked as checkmate. This game has ended in a draw. Final move was { playGameInfo.FinalMove }.\r\n{ game.FEN }\r\n{ playGameInfo.GameString }");
			}
			if (playGameInfo.HasCheckmate) {
				Assert.IsTrue(playGameInfo.GameState.StateInfo.IsCheckmate, $"Game should be marked as checkmate. Final move was { moveCount }. { playGameInfo.FinalMove }\r\n{ game.FEN }\r\n{ playGameInfo.GameString }");
				Assert.AreEqual(game.Result, playGameInfo.GameState.StateInfo.Result, $"Game Result should be the same.\r\n{ game.FEN }\r\n{ playGameInfo.GameString }");
			}
			game.FEN = playGameInfo.GameState.ToString();
			//so we don't run this test again
			game.IsActive = false;
			var updateResult = await this.GameService.Update(game);
			Assert.IsTrue(updateResult.Success, updateResult.Message);
		}

		private async Task playGame_ArriveAtSameresult_NoAssertions(Game game) {
			var playGameInfo = playGamePrep(game);
			var count = playGameInfo.GameData.Moves.Count();
			var failed = false;
			var moveCount = 0;
			foreach (var move in playGameInfo.GameData.Moves) {
				moveCount++;
				try {
					playGameInfo.GameState = playMove(playGameInfo.GameState, game, move.Value, moveCount);
				} catch (System.Exception ex) {
					failed = true;
					break;
				}
			}
			game.FEN = playGameInfo.GameState.ToString();
			if (!failed) {
				game.IsActive = false;
			}
			var updateResult = await this.GameService.Update(game);
		}

		private PlayGameInfo playGamePrep(Game game) {
			var playGameInfo = new PlayGameInfo();
			playGameInfo.GameString = game.GameToString();
			FileUtility.WriteFile<PlayTests>("playGame.pgn", "Output", playGameInfo.GameString);
			var result = playGameInfo.GameString.Split(" ").Last();
			var gameStateResult = this.GameStateService.Initialize();
			playGameInfo.GameState = gameStateResult.Result;
			game.FEN = GeneralReference.Starting_FEN_Position;
			playGameInfo.HasCheckmate = playGameInfo.GameString.Split("\r\n\r\n")[1].Contains('#');
			playGameInfo.IsDraw = game.Result == "1/2-1/2";
			playGameInfo.FinalMove = string.Empty;
			playGameInfo.GameData = this.PGNFileService.ParsePGNData(playGameInfo.GameString);
			return playGameInfo;
		}

		private GameState playMove(GameState gameState, Game game, string move, int moveCount) {
			var test = "";
			var moveBreak = 74;
			var xs = move.Split(' ');
			var a = xs[0];
			if (endgamePattern.Matches(a).Any()) {
				return (gameState);
			}
			if (moveCount >= moveBreak) {
				test = "";
				//fool the compiler into not giving me warnings about "test"
				if (string.IsNullOrEmpty(test)) { }
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
			if (moveCount >= moveBreak) {
				test = "";
			}
			gameStateResult = this.GameStateService.MakeMove(gameStateResult.Result, b);
			Assert.IsTrue(gameStateResult.Success, $"Move should have been successful. { b } | { game.FEN } \r\n{ gameStateResult.Message }");
			//record and save the FEN at every step so I can figure out where things went wrong.
			game.FEN = gameStateResult.Result.ToString();
			return gameStateResult.Result;
		}
	}
}