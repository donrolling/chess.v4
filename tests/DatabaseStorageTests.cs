using chess.v4.engine.interfaces;
using Data.Repository.Dapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using models;
using Tests.Models;

namespace Tests {
	[TestClass]
	public class DatabaseStorageTests : TestBase {
		public GameRepository GameRepository { get; }
		public IGameStateService GameStateService { get; }

		public DatabaseStorageTests() {
			this.GameStateService = this.ServiceProvider.GetService<IGameStateService>();
			this.GameRepository = new GameRepository(this.AppSettings.Value.ConnectionStrings.DefaultConnection);
		}

		[TestMethod]
		public void InsertTest() {
			var game = new Game();
			var insertResult = this.GameRepository.Insert(game);
			Assert.AreNotEqual(0, insertResult.Result);
			var selectResult = this.GameRepository.SelectById(insertResult.Result);
			Assert.IsTrue(selectResult.Success);
			Assert.IsNotNull(selectResult.Result);
		}
	}
}