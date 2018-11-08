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
			var gameData = this.PGNFileService.ParsePGNData(gameString);
			var gameStateResult = this.GameStateService.Initialize();
			var gameState = gameStateResult.Result;
			game.FEN = GeneralReference.Starting_FEN_Position;
			foreach (var move in gameData.Moves) {
				gameStateResult = playMove(ref gameState, move);
				Assert.IsTrue(gameStateResult.Success, "Move should succeed.");
			}
		}

		private Common.Models.Envelope<GameState> playMove(ref GameState gameState, System.Collections.Generic.KeyValuePair<int, string> move) {
			Common.Models.Envelope<GameState> gameStateResult;
			var xs = move.Value.Split('.')[1].Split(' ');
			var a = xs[0];
			var b = xs[1];

			var pair1 = this.PGNService.PGNMoveToSquarePair(gameState, a);
			gameStateResult = this.GameStateService.MakeMove(gameState, pair1.piecePosition, pair1.newPiecePosition);
			Assert.IsTrue(gameStateResult.Success, "Move should succeed.");
			gameState = gameStateResult.Result;
			var pieceType = this.PGNService.GetPieceTypeFromPGNMove(move.Value);
			var piece = new Piece(pieceType, gameState.ActiveColor);
			var end = this.PGNService.GetPositionFromPGNMove(move.Value, gameState.ActiveColor);
			var start = this.PGNService.GetCurrentPositionFromPGNMove(gameState, piece, end, move.Value);
			return gameStateResult;
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
			return sb.ToString();
		}
	}
}