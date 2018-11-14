using Business.Service.EntityServices.Interfaces;
using chess.v4.engine.interfaces;
using chess.v4.engine.reference;
using chess.v4.models;
using Data.Dapper.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models.Entities;
using System.Linq;
using System.Text;
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
			Assert.IsNotNull(game);
			var gameString = this.getGameString(game);
			var result = gameString.Split(" ").Last();
			var gameStateResult = this.GameStateService.Initialize();
			var gameState = gameStateResult.Result;
			game.FEN = GeneralReference.Starting_FEN_Position;
			
			var moveCount = 1;
			var hasCheckmate = false;
			var isDraw = game.Result == "1/2-1/2";
			var finalMove = string.Empty;
			
			var gameData = this.PGNFileService.ParsePGNData(gameString);
			var count = gameData.Moves.Count();
			foreach (var move in gameData.Moves) {
				if (move.Value.Contains('-')) {
					continue;
				}
				if (moveCount == count) {
					finalMove = move.Value;
					hasCheckmate = move.Value.Contains('#');
				}
				gameState = playMove(gameState, game, move.Value);
				moveCount++;
			}
			if (hasCheckmate) {
				Assert.IsTrue(gameState.StateInfo.IsCheckmate, $"Game should be marked as checkmate. Final move was { finalMove }.\r\n{ game.FEN }");
				Assert.AreEqual(game.Result, gameState.StateInfo.Result, $"Game Result should be the same.\r\n{ game.FEN }");
			} else if (isDraw) {
				Assert.IsTrue(gameState.StateInfo.IsDraw, $"Game should be marked as a draw.\r\n{ game.FEN }");
				Assert.AreEqual(game.Result, gameState.StateInfo.Result, $"Game Result should be the same.\r\n{ game.FEN }");
			} else {
				Assert.IsFalse(gameState.StateInfo.IsCheckmate, $"Game should not be marked as checkmate. This game must have ended in a resignation or a draw. Final move was { finalMove }.\r\n{ game.FEN }");
			}
			game.FEN = gameState.ToString();
			//so we don't run this test again
			game.IsActive = false;
			var updateResult = await this.GameService.Update(game);
			Assert.IsTrue(updateResult.Success, updateResult.Message);
		}

		private string getGameString(Game game) {
			var sb = new StringBuilder();
			sb.AppendLine($"[Event \"{ game.Event }\"]");
			sb.AppendLine($"[Site \"{ game.Site }\"]");
			sb.AppendLine($"[Date \"{ game.Date }\"]");
			sb.AppendLine($"[Round \"{ game.Round }\"]");
			sb.AppendLine($"[White \"{ game.White }\"]");
			sb.AppendLine($"[Black \"{ game.Black }\"]");
			sb.AppendLine($"[Result \"{ game.Result }\"]");
			sb.AppendLine($"[ECO \"{ game.ECO }\"]");
			sb.AppendLine($"[WhiteElo \"{ game.WhiteElo }\"]");
			sb.AppendLine($"[BlackElo \"{ game.BlackElo }\"]");
			sb.AppendLine($"[ID \"{ game.NaturalKey }\"]");
			sb.AppendLine($"[FileName \"{ game.FileName }\"]");
			sb.AppendLine($"[Annotator \"{ game.Annotator }\"]");
			sb.AppendLine($"[Source \"{ game.Source }\"]");
			sb.AppendLine($"[Remark \"{ game.Remark }\"]");
			sb.AppendLine("");
			sb.AppendLine(game.PGN);
			return sb.ToString().Trim('\n').Trim('\r');
		}

		private GameState playMove(GameState gameState, Game game, string move) {
			if (move.Contains('-')) {
				return gameState;
			}
			var xs = move.Split('.')[1].Split(' ');
			var a = xs[0];
			var regex = new Regex(@"\d\-\d");
			if (regex.Matches(a).Any()) {
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
			if (regex.Matches(b).Any()) {
				return (gameStateResult.Result);
			}
			gameStateResult = this.GameStateService.MakeMove(gameStateResult.Result, b);
			Assert.IsTrue(gameStateResult.Success, $"Move should have been successful. { b } | { game.FEN }");
			//record and save the FEN at every step so I can figure out where things went wrong.
			game.FEN = gameStateResult.Result.ToString();
			return gameStateResult.Result;
		}
	}
}