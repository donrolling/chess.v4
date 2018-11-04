using chess.v4.engine.interfaces;
using common;
using data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using models;
using tests.setup;

namespace tests {
	[TestClass]
	public class DatabaseStorageTests {
		public GameRepository GameRepository { get; }
		public IGameStateService GameStateService { get; }

		public DatabaseStorageTests() {
			var testSetup = new TestSetup();
			var serviceProvider = testSetup.Setup();
			this.GameStateService = serviceProvider.GetService<IGameStateService>();
			this.GameRepository = new GameRepository(TestSetup.ConnectionString);
		}

		[TestMethod]
		public void InsertTest() {
			var game = new Game();
			var result = this.GameRepository.Insert(game);
			Assert.AreNotEqual(0, result);
		}
	}
}