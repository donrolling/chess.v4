using chess.v4.engine.enumeration;
using chess.v4.engine.interfaces;
using common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
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
			var fen = "1k3r2/7p/4BPp1/1N1R4/8/2P5/PP3PPP/R5K1 w  - 3 26";
			var gameState = TestUtility.GetGameState(this.GameStateService, fen);
			var attacks = gameState.Attacks.Where(a => a.AttackingSquare.Name == "f2").ToList();
			var squares = new List<int> { 21, 29 };
			TestUtility.ListContainsSquares(attacks, squares, PieceType.Pawn);
			Assert.AreEqual(2, attacks.Count());
			var gameStateResult = this.GameStateService.MakeMove(gameState, 13, 20);
			Assert.IsTrue(gameStateResult.Failure, "This move was invalid, so the attempt to make it should fail.");
		}

		[TestMethod]
		public void GameStart_Verify_PawnAttack() {
			var gameState = TestUtility.GetGameState(this.GameStateService);
			//white queenside rook pawn, opening moves
			var attacks = gameState.Attacks.Where(a => a.AttackingSquare.Name == "a2").ToList();
			var squares = new List<int> { 16, 24 };
			TestUtility.ListContainsSquares(attacks, squares, PieceType.Pawn);
			Assert.AreEqual(2, attacks.Count());

			//white king pawn, opening moves
			attacks = gameState.Attacks.Where(a => a.AttackingSquare.Name == "e2").ToList();
			squares = new List<int> { 20, 28 };
			TestUtility.ListContainsSquares(attacks, squares, PieceType.Pawn);
			Assert.AreEqual(2, attacks.Count());
			
			//black
			attacks = gameState.Attacks.Where(a => a.AttackingSquare.Name == "d7").ToList();
			squares = new List<int> { 35, 43 };
			TestUtility.ListContainsSquares(attacks, squares, PieceType.Pawn);
			Assert.AreEqual(2, attacks.Count());
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