using Business.Service.EntityServices.Interfaces;
using chess.v4.engine.interfaces;
using chess.v4.models;
using Common.IO;
using Microsoft.Extensions.DependencyInjection;
using Models.Entities;
using Omu.ValueInjecter;
using System.Linq;
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
			var data = FileUtility.ReadTextFile<Importer>("December1.pgn", "Data\\Games");
			//Console.WriteLine(data);
			var groups = data.Split("\r\n\r\n");
			for (int i = 0; i < groups.Length; i = i + 2) {
				var metadata = groups[i];
				var moves = groups[i + 1];
				var result = moves.Split(" ").Last();
				var gameData = this.PGNFileService.ParsePGNData($"{ metadata }\r\n\r\n{ moves }");
				var game = new Game();
				game.InjectFrom(gameData);
				var gameStateResult = this.GameStateService.Initialize();
				var gameState = gameStateResult.Result;
				game.FEN = gameState.ToString();
				game.PGN = moves;
				game.Result = result;
				var saveResult = await this.GameService.Create(game);
			}

			return true;
		}

		public async Task<bool> ImportDraws() {
			var data = FileUtility.ReadTextFile<Importer>("December1.pgn", "Data\\Games");
			//Console.WriteLine(data);
			var groups = data.Split("\r\n\r\n");
			for (int i = 0; i < groups.Length; i = i + 2) {
				var metadata = groups[i];
				var moves = groups[i + 1].Replace("\r\n", "");
				var result = moves.Split(" ").Last();
				if (result != "1/2-1/2") {
					continue;
				}
				var gameData = this.PGNFileService.ParsePGNData($"{ metadata }\r\n\r\n{ moves }");
				var game = new Game();
				game.InjectFrom(gameData);
				var gameStateResult = this.GameStateService.Initialize();
				var gameState = gameStateResult.Result;
				game.FEN = gameState.ToString();
				game.PGN = moves;
				game.Result = result;
				var saveResult = await this.GameService.Create(game);
				if (saveResult.Failure) {
					throw new System.Exception(saveResult.Message);
				}
			}

			return true;
		}
	}
}