using chess.v4.engine.enumeration;
using chess.v4.engine.interfaces;
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
		public void WhiteRookAttacksEmptyBoardFromD4() {
			var fen = "7k/8/8/8/3R4/8/8/7K b - - 0 32";
			var gameState = TestUtility.GetGameState(this.GameStateService, fen);
			var whiteRookAttacks = gameState.Attacks.Where(a => a.AttackerSquare.Index == 27).ToList();
			var allSquareIndexs = new int[] { 24, 25, 26, 28, 29, 30, 31, 59, 51, 43, 35, 19, 11, 3 };
			foreach (var x in allSquareIndexs) {
				TestUtility.ListContainsSquare(whiteRookAttacks, PieceType.Rook, x);
			}
		}
	}
}