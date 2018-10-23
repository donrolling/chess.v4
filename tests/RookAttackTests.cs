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
	public class RookAttackTests {
		public IAttackService AttackService { get; }
		
		public IGameStateService GameStateService { get; }
		public IMoveService MoveService { get; }

		public RookAttackTests() {
			var serviceProvider = new TestSetup().Setup();
			
			this.AttackService = serviceProvider.GetService<IAttackService>();
			this.GameStateService = serviceProvider.GetService<IGameStateService>();
			this.MoveService = serviceProvider.GetService<IMoveService>();
		}

		[TestMethod]
		public void RookAttack() {
			var gameState = TestUtility.GetGameState(this.GameStateService);

			var a1RookAttacks = AttackService.GetPieceAttacks(board.FEN, new KeyValuePair<int, char>(0, 'R'));
			//Assert.IsTrue(a1RookAttacks.Contains(1));
			//Assert.IsTrue(a1RookAttacks.Contains(2));
			//Assert.IsTrue(a1RookAttacks.Contains(3));
			//Assert.IsTrue(a1RookAttacks.Contains(4));
			//Assert.IsTrue(a1RookAttacks.Contains(5));
			//Assert.IsTrue(a1RookAttacks.Contains(6));
			//Assert.IsTrue(a1RookAttacks.Contains(7));
			//Assert.IsTrue(a1RookAttacks.Contains(8));
			//Assert.IsTrue(a1RookAttacks.Contains(16));
			//Assert.IsTrue(a1RookAttacks.Contains(24));
			//Assert.IsTrue(a1RookAttacks.Contains(32));
			//Assert.IsTrue(a1RookAttacks.Contains(40));
			//Assert.IsTrue(a1RookAttacks.Contains(48));
			//Assert.IsTrue(a1RookAttacks.Contains(56));
			Assert.IsTrue(a1RookAttacks.Count() == 0);

			var a8RookAttacks = AttackService.GetPieceAttacks(board.FEN, new KeyValuePair<int, char>(56, 'r'));
			//Assert.IsTrue(a8RookAttacks.Contains(0));
			//Assert.IsTrue(a8RookAttacks.Contains(8));
			//Assert.IsTrue(a8RookAttacks.Contains(16));
			//Assert.IsTrue(a8RookAttacks.Contains(24));
			//Assert.IsTrue(a8RookAttacks.Contains(32));
			//Assert.IsTrue(a8RookAttacks.Contains(40));
			//Assert.IsTrue(a8RookAttacks.Contains(48));
			//Assert.IsTrue(a8RookAttacks.Contains(57));
			//Assert.IsTrue(a8RookAttacks.Contains(58));
			//Assert.IsTrue(a8RookAttacks.Contains(59));
			//Assert.IsTrue(a8RookAttacks.Contains(60));
			//Assert.IsTrue(a8RookAttacks.Contains(61));
			//Assert.IsTrue(a8RookAttacks.Contains(62));
			//Assert.IsTrue(a8RookAttacks.Contains(63));
			Assert.IsTrue(a8RookAttacks.Count() == 0);

			var h1RookAttacks = AttackService.GetPieceAttacks(board.FEN, new KeyValuePair<int, char>(7, 'R'));
			//Assert.IsTrue(h1RookAttacks.Contains(0));
			//Assert.IsTrue(h1RookAttacks.Contains(1));
			//Assert.IsTrue(h1RookAttacks.Contains(2));
			//Assert.IsTrue(h1RookAttacks.Contains(3));
			//Assert.IsTrue(h1RookAttacks.Contains(4));
			//Assert.IsTrue(h1RookAttacks.Contains(5));
			//Assert.IsTrue(h1RookAttacks.Contains(6));
			//Assert.IsTrue(h1RookAttacks.Contains(15));
			//Assert.IsTrue(h1RookAttacks.Contains(23));
			//Assert.IsTrue(h1RookAttacks.Contains(31));
			//Assert.IsTrue(h1RookAttacks.Contains(39));
			//Assert.IsTrue(h1RookAttacks.Contains(47));
			//Assert.IsTrue(h1RookAttacks.Contains(55));
			//Assert.IsTrue(h1RookAttacks.Contains(63));
			Assert.IsTrue(h1RookAttacks.Count() == 0);

			var h8RookAttacks = AttackService.GetPieceAttacks(board.FEN, new KeyValuePair<int, char>(63, 'r'));
			//Assert.IsTrue(h8RookAttacks.Contains(7));
			//Assert.IsTrue(h8RookAttacks.Contains(15));
			//Assert.IsTrue(h8RookAttacks.Contains(23));
			//Assert.IsTrue(h8RookAttacks.Contains(31));
			//Assert.IsTrue(h8RookAttacks.Contains(39));
			//Assert.IsTrue(h8RookAttacks.Contains(47));
			//Assert.IsTrue(h8RookAttacks.Contains(55));
			//Assert.IsTrue(h8RookAttacks.Contains(56));
			//Assert.IsTrue(h8RookAttacks.Contains(57));
			//Assert.IsTrue(h8RookAttacks.Contains(58));
			//Assert.IsTrue(h8RookAttacks.Contains(59));
			//Assert.IsTrue(h8RookAttacks.Contains(60));
			//Assert.IsTrue(h8RookAttacks.Contains(61));
			//Assert.IsTrue(h8RookAttacks.Contains(62));
			Assert.IsTrue(h8RookAttacks.Count() == 0);
		}
	}
}