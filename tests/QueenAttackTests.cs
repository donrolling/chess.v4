
using chess.v4.engine.enumeration;
using chess.v4.engine.extensions;
using chess.v4.engine.interfaces;
using chess.v4.engine.reference;
using common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using tests.setup;
using tests.utility;

namespace tests {

	[TestClass]
	public class QueenAttackTests {
		public IAttackService AttackService { get; }
		
		public IGameStateService GameStateService { get; }
		public IMoveService MoveService { get; }

		public QueenAttackTests() {
			var serviceProvider = new TestSetup().Setup();
			
			this.AttackService = serviceProvider.GetService<IAttackService>();
			this.GameStateService = serviceProvider.GetService<IGameStateService>();
			this.MoveService = serviceProvider.GetService<IMoveService>();
		}

		[TestMethod]
		public void WhiteQueenAttacksStartingPosition() {
			var gameState = TestUtility.GetGameState(this.GameStateService);
			var whiteQueenAttacks = gameState.Attacks.Where(a => a.AttackerSquare.Name == "d1");
			Assert.IsTrue(whiteQueenAttacks.Count() == 0);
		}

		[TestMethod]
		public void WhiteQueenAttacksEmptyBoardFromD4() {
			//only kings and a queen on the board
			var fen = "7k/8/8/8/3Q4/8/8/7K b - - 0 32";
			var gameState = TestUtility.GetGameState(this.GameStateService, fen);
			var whiteQueenAttacks = gameState.Attacks.Where(a => a.AttackerSquare.Name == "d4").ToList();
			var allSquareIndexs = new int[] { 59, 51, 43, 35, 27, 19, 11, 3, 24, 25, 26, 27, 28, 29, 30, 31, 48, 42, 34, 20, 13, 6, 0, 9, 18, 36, 45, 54, 63 };
			foreach (var x in allSquareIndexs) {
				Assert.IsNotNull(whiteQueenAttacks.GetSquare(x), $"Queen should be able to attack square: { x }");
			}
		}
	}
}