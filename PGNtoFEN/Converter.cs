using chess.v4.engine.interfaces;
using chess.v4.models;
using Microsoft.Extensions.DependencyInjection;
using Tests.Models;

namespace PGNtoFEN {
	/// <summary>
	/// Going for quick and dirty here. Inheriting from a test class. Classy.
	/// </summary>
	public class Converter : TestBase {
		public IGameStateService GameStateService { get; }
		public IPGNFileService PGNFileService { get; }
		public IPGNService PGNService { get; }

		public Converter() {
			this.GameStateService = this.ServiceProvider.GetService<IGameStateService>();
			this.PGNService = this.ServiceProvider.GetService<IPGNService>();
			this.PGNFileService = this.ServiceProvider.GetService<IPGNFileService>();
		}

		internal string Convert(string pgn) {
			var gameStateResult = this.GameStateService.Initialize();
			var gameState = gameStateResult.Result;
			var gameData = this.PGNFileService.ParsePGNData(pgn);
			var fen = gameState.ToString();
			foreach (var move in gameData.Moves) {
				var moveResult = playMove(gameState, move.Value);
				gameState = moveResult.GameState;
				fen = moveResult.FEN;
			}
			return fen;
		}

		private (GameState GameState, string FEN) playMove(GameState gameState, string move) {
			var fen = string.Empty;
			var xs = move.Split('.')[1].Split(' ');
			var a = xs[0];
			var gameStateResult = this.GameStateService.MakeMove(gameState, a);
			//record and save the FEN at every step so I can figure out where things went wrong.
			fen = gameStateResult.Result.ToString();
			if (xs.Length == 1) {
				return (gameStateResult.Result, fen);
			}
			var b = xs[1];
			gameStateResult = this.GameStateService.MakeMove(gameStateResult.Result, b);
			//record and save the FEN at every step so I can figure out where things went wrong.
			fen = gameStateResult.Result.ToString();
			return (gameStateResult.Result, fen);
		}
	}
}