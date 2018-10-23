using chess.v4.engine.interfaces;
using chess.v4.engine.extensions;
using common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using tests.setup;
using tests.utility;

namespace tests {

	[TestClass]
	public class KnightAttackTests {
		public IAttackService AttackService { get; }
		public IGameStateService GameStateService { get; }
		public IMoveService MoveService { get; }

		public KnightAttackTests() {
			var serviceProvider = new TestSetup().Setup();
			
			this.AttackService = serviceProvider.GetService<IAttackService>();
			this.GameStateService = serviceProvider.GetService<IGameStateService>();
			this.MoveService = serviceProvider.GetService<IMoveService>();
		}

		[TestMethod]
		public void WhiteKnightAttackStartingPositionFromD5() {
			var fen = "rnbqkbnr/pppppppp/8/3N4/8/8/PPPPPPPP/R1BQKBNR b KQkq - 0 1";
			var gameState = TestUtility.GetGameState(this.GameStateService, fen);
			var knightAttacks = gameState.Attacks.Where(a => a.AttackerSquare.Name == "d5").ToList();
			var allSquareIndexs = new int[] { 18, 20, 25, 29, 41, 45, 50, 52 };
			foreach (var x in allSquareIndexs) {
				Assert.IsNotNull(knightAttacks.GetSquare(x), $"Knight should be able to attack square: { x }");
			}
			Assert.IsTrue(knightAttacks.Count() == 8);
		}

		[TestMethod]
		public void WhiteKnightAttackStartingPositionFromB1() {
			var fen = "rnbqkbnr/pppppppp/8/3N4/8/8/PPPPPPPP/R1BQKBNR b KQkq - 0 1";
			var gameState = TestUtility.GetGameState(this.GameStateService, fen);
			var knightAttacks = gameState.Attacks.Where(a => a.AttackerSquare.Name == "b1").ToList();
			var allSquareIndexs = new int[] { 18, 16 };
			foreach (var x in allSquareIndexs) {
				Assert.IsNotNull(knightAttacks.GetSquare(x), $"Knight should be able to attack square: { x }");
			}
			Assert.IsTrue(knightAttacks.Count() == 2);
		}

		[TestMethod]
		public void WhiteKnightAttackStartingPositionFromB4() {
			var fen = "rnbqkbnr/pppppppp/8/3N4/8/8/PPPPPPPP/R1BQKBNR b KQkq - 0 1";
			var gameState = TestUtility.GetGameState(this.GameStateService, fen);
			var knightAttacks = gameState.Attacks.Where(a => a.AttackerSquare.Name == "b4").ToList();
			var allSquareIndexs = new int[] { 19, 35, 40, 42 };
			foreach (var x in allSquareIndexs) {
				Assert.IsNotNull(knightAttacks.GetSquare(x), $"Knight should be able to attack square: { x }");
			}
			Assert.IsTrue(knightAttacks.Count() == 4);
		}
	}
}