using chess.v4.engine.enumeration;
using chess.v4.engine.interfaces;
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
		public void WhiteQueenAttacksEmptyBoardFromD4() {
			//only kings and a queen on the board
			var fen = "7k/8/8/8/3Q4/8/8/7K b - - 0 32";
			var gameState = TestUtility.GetGameState(this.GameStateService, fen);
			var attacks = gameState.Attacks.Where(a => a.AttackerSquare.Name == "d4").ToList();
			var allSquareIndexs = new int[] { 0, 3, 6, 9, 11, 13, 18, 19, 20, 24, 25, 26, 28, 29, 30, 31, 34, 35, 36, 41, 43, 45, 48, 51, 54, 59, 63 };
			TestUtility.ListContainsSquares(attacks, allSquareIndexs.ToList(), PieceType.Queen);
		}

		[TestMethod]
		public void WhiteQueenAttacksStartingPosition() {
			var gameState = TestUtility.GetGameState(this.GameStateService);
			var whiteQueenAttacks = gameState.Attacks.Where(a => a.AttackerSquare.Name == "d1");
			Assert.IsTrue(whiteQueenAttacks.Count() == 0);
		}
	}
}