using chess.v4.engine.enumeration;
using chess.v4.engine.interfaces;
using common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using tests.setup;
using tests.utility;

namespace tests {

	[TestClass]
	public class PawnAttackTests {
		public IAttackService AttackService { get; }
		
		public IGameStateService GameStateService { get; }
		public IMoveService MoveService { get; }

		public PawnAttackTests() {
			var serviceProvider = new TestSetup().Setup();
			
			this.AttackService = serviceProvider.GetService<IAttackService>();
			this.GameStateService = serviceProvider.GetService<IGameStateService>();
			this.MoveService = serviceProvider.GetService<IMoveService>();
		}

		[TestMethod]
		public void PawnAnomalies() {
			//test some errors that I found involving pawns moving diagonally when they're not capturing
			var fen = "1k3r2/7p/4BPp1/1N1R4/8/2P5/PP3PPP/R5K1 w  - 3 26";
			var gameState = TestUtility.GetGameState(this.GameStateService, fen);
			var gameStateResult = this.GameStateService.MakeMove(gameState, 13, 20);
			Assert.IsFalse(gameStateResult.Success, "This move was invalid, so the attempt to make it should fail.");
		}

		[TestMethod]
		public void PawnAttack() {
			var gameState = TestUtility.GetGameState(this.GameStateService);
			var attacks = gameState.Attacks;
			//white queenside rook pawn, opening moves
			var a2PawnAttacks = attacks.Where(a => a.AttackerSquare.Name == "a2");
			Assert.IsTrue(a2PawnAttacks.Any(a => a.Index == 16));
			Assert.IsTrue(a2PawnAttacks.Any(a => a.Index == 17));
			Assert.IsTrue(a2PawnAttacks.Any(a => a.Index == 24));
			Assert.IsTrue(a2PawnAttacks.Count() == 3);

			//white king pawn, opening moves
			var e2PawnAttacks = attacks.Where(a => a.AttackerSquare.Name == "e2");
			Assert.IsTrue(e2PawnAttacks.Any(a => a.Index == 19));
			Assert.IsTrue(e2PawnAttacks.Any(a => a.Index == 20));
			Assert.IsTrue(e2PawnAttacks.Any(a => a.Index == 21));
			Assert.IsTrue(e2PawnAttacks.Any(a => a.Index == 28));
			Assert.IsTrue(e2PawnAttacks.Count() == 4);

			//black
			var d7PawnAttacks = attacks.Where(a => a.AttackerSquare.Name == "d7");
			Assert.IsTrue(d7PawnAttacks.Any(a => a.Index == 42));
			Assert.IsTrue(d7PawnAttacks.Any(a => a.Index == 43));
			Assert.IsTrue(d7PawnAttacks.Any(a => a.Index == 44));
			Assert.IsTrue(d7PawnAttacks.Any(a => a.Index == 35));
			Assert.IsTrue(d7PawnAttacks.Count() == 4);
		}

		[TestMethod]
		public void PawnPromoteToQueen() {
			var fen = "3r3r/1P1n1p2/2BQpk2/3p4/3PP1pp/p1P2N2/P5PP/R1BK3R w - - 0 32";
			var gameState = TestUtility.GetGameState(this.GameStateService, fen);
			var gameStateResult = this.GameStateService.MakeMove(gameState, 49, 57, PieceType.Queen);
			Assert.IsTrue(gameStateResult.Success);
			Assert.AreEqual("1Q1r3r/3n1p2/2BQpk2/3p4/3PP1pp/p1P2N2/P5PP/R1BK3R b - - 0 32", gameStateResult.Result.ToString(), "The board states should be equal.");
		}
	}
}