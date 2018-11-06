using Business.Service.EntityServices.Interfaces;
using chess.v4.engine.interfaces;
using chess.v4.models;
using Common.IO;
using Microsoft.Extensions.DependencyInjection;
using Models.Entities;
using Omu.ValueInjecter;
using System.Threading.Tasks;
using Tests.Models;

namespace PGNImport {
	/// <summary>
	/// Going for quick and dirty here. Inheriting from a test class. Classy.
	/// </summary>
	public class Importer : TestBase {
		public IGameService GameService { get; }
		public IGameStateService GameStateService { get; }
		public IPGNFileService PGNFileService { get; }
		public IPGNService PGNService { get; }

		public Importer() {
			this.GameStateService = this.ServiceProvider.GetService<IGameStateService>();
			this.PGNService = this.ServiceProvider.GetService<IPGNService>();
			this.GameService = this.ServiceProvider.GetService<IGameService>();
			this.PGNFileService = this.ServiceProvider.GetService<IPGNFileService>();
		}

		public async Task<bool> Import() {
			var data = FileUtility.ReadTextFile<Importer>("sample.pgn", "Data\\Games");
			//Console.WriteLine(data);
			var groups = data.Split("\r\n\r\n");
			for (int i = 0; i < groups.Length; i = i + 2) {
				var metadata = groups[i];
				var moves = groups[i + 1];
				var gameData = this.PGNFileService.ParsePGNData($"{ metadata }\r\n\r\n{ moves }");
				var game = new Game();
				game.InjectFrom(gameData);
				var gameStateResult = this.GameStateService.Initialize();
				var gameState = gameStateResult.Result;
				foreach (var move in gameData.Moves) {
					var xs = move.Value.Split('.')[1].Split(' ');
					var a = xs[0];
					var b = xs[1];
					var pair1 = this.PGNService.PGNMoveToSquarePair(gameState, a);
					gameStateResult = this.GameStateService.MakeMove(gameState, pair1.piecePosition, pair1.newPiecePosition);
					gameState = gameStateResult.Result;
					//var pieceType = this.PGNService.GetPieceTypeFromPGNMove(move.Value);
					//var piece = new Piece(pieceType, gameState.ActiveColor);
					//var end = this.PGNService.GetPositionFromPGNMove(move.Value, gameState.ActiveColor);
					//var start = this.PGNService.GetCurrentPositionFromPGNMove(gameState, piece, end, move.Value);
					var pair2 = this.PGNService.PGNMoveToSquarePair(gameState, b);
					gameStateResult = this.GameStateService.MakeMove(gameState, pair2.piecePosition, pair2.newPiecePosition);
					gameState = gameStateResult.Result;
				}
				game.FEN = gameState.ToString();
				game.PGN = moves;
				var saveResult = await this.GameService.Create(game);
			}

			return true;
		}
	}
}