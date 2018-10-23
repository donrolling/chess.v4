using chess.v4.engine.extensions;
using chess.v4.engine.interfaces;
using common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using tests.setup;
using tests.utility;

namespace tests {

	[TestClass]
	public class BishopAttackTests {
		public IAttackService AttackService { get; }

		public IGameStateService GameStateService { get; }
		public IMoveService MoveService { get; }

		public BishopAttackTests() {
			var serviceProvider = new TestSetup().Setup();

			this.AttackService = serviceProvider.GetService<IAttackService>();
			this.GameStateService = serviceProvider.GetService<IGameStateService>();
			this.MoveService = serviceProvider.GetService<IMoveService>();
		}

		[TestMethod]
		public void WhiteBishopAttacksStartingPositionFromD4() {
			var fen = "rnbqkbnr/pppppppp/8/8/3B4/8/PPPPPPPP/RN1QKBNR b KQkq - 0 1";
			var gameState = TestUtility.GetGameState(this.GameStateService, fen);
			var whiteBishopAttacks = gameState.Attacks.Where(a => a.AttackerSquare.Name == "d4").ToList();
			var allSquareIndexs = new int[] { 20, 34, 41, 48, 18, 36, 45, 54 };
			foreach (var x in allSquareIndexs) {
				Assert.IsNotNull(whiteBishopAttacks.GetSquare(x), $"Bishop should be able to attack square: { x }");
			}			
			Assert.IsTrue(whiteBishopAttacks.Count() == 8);
		}
	}
}