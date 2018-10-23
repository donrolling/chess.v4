using chess.v4.engine.enumeration;
using chess.v4.engine.interfaces;
using chess.v4.engine.reference;
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
		public void BishopAttack() {
			var gameState = TestUtility.GetGameState(this.GameStateService);

			var whiteBishopAttacks = AttackService.GetPieceAttacks(board.FEN, new KeyValuePair<int, char>(2, 'B'));
			//Assert.IsTrue(whiteBishopAttacks.Contains(9));
			//Assert.IsTrue(whiteBishopAttacks.Contains(16));
			//Assert.IsTrue(whiteBishopAttacks.Contains(11));
			//Assert.IsTrue(whiteBishopAttacks.Contains(20));
			//Assert.IsTrue(whiteBishopAttacks.Contains(29));
			//Assert.IsTrue(whiteBishopAttacks.Contains(38));
			//Assert.IsTrue(whiteBishopAttacks.Contains(47));
			Assert.IsTrue(whiteBishopAttacks.Count() == 0);

			var whiteBishopAttacks2 = AttackService.GetPieceAttacks(board.FEN, new KeyValuePair<int, char>(27, 'B'));
			Assert.IsTrue(whiteBishopAttacks2.Contains(20));
			Assert.IsTrue(whiteBishopAttacks2.Contains(34));
			Assert.IsTrue(whiteBishopAttacks2.Contains(41));
			Assert.IsTrue(whiteBishopAttacks2.Contains(48));
			Assert.IsTrue(whiteBishopAttacks2.Contains(18));
			Assert.IsTrue(whiteBishopAttacks2.Contains(36));
			Assert.IsTrue(whiteBishopAttacks2.Contains(45));
			Assert.IsTrue(whiteBishopAttacks2.Contains(54));
			Assert.IsTrue(whiteBishopAttacks2.Count() == 8);
		}
	}
}