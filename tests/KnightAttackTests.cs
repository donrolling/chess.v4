using chess.v4.engine.interfaces;
using chess.v4.engine.extensions;
using common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using tests.setup;
using tests.utility;
using System.Collections.Generic;
using chess.v4.engine.enumeration;

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
			var attacks = gameState.Attacks.Where(a => a.AttackingSquare.Name == "d5").ToList();
			var squares = new List<int> { 18, 20, 25, 29, 41, 45, 50, 52 };
			TestUtility.ListContainsSquares(attacks, squares, PieceType.Knight);
			Assert.IsTrue(attacks.Count() == 8, "Wrong number of attacks.");
		}

		[TestMethod]
		public void WhiteKnightAttackStartingPositionFromB1() {
			var fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
			var gameState = TestUtility.GetGameState(this.GameStateService, fen);
			var attacks = gameState.Attacks.Where(a => a.AttackingSquare.Name == "b1").ToList();
			var squares = new List<int> { 18, 16 };
			TestUtility.ListContainsSquares(attacks, squares, PieceType.Knight);
			Assert.IsTrue(attacks.Count() == 2, "Wrong number of attacks.");
		}

		[TestMethod]
		public void WhiteKnightAttackStartingPositionFromB4() {
			var fen = "rnbqkbnr/pppppppp/8/8/1N6/8/PPPPPPPP/R1BQKBNR b KQkq - 0 1";
			var gameState = TestUtility.GetGameState(this.GameStateService, fen);
			var attacks = gameState.Attacks.Where(a => a.AttackingSquare.Name == "b4").ToList();
			var squares = new List<int> { 19, 35, 40, 42 };
			TestUtility.ListContainsSquares(attacks, squares, PieceType.Knight);
			Assert.IsTrue(attacks.Count() == 4, "Wrong number of attacks.");
		}
	}
}