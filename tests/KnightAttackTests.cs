using chess.v4.engine.interfaces;
using chess.v4.models.enumeration;
using Common.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using Tests.Models;
using Tests.Utility;

namespace Tests {
	[TestClass]
	public class KnightAttackTests : TestBase {
		public IAttackService AttackService { get; }
		public IGameStateService GameStateService { get; }
		public IMoveService MoveService { get; }

		public KnightAttackTests() {
			this.AttackService = this.ServiceProvider.GetService<IAttackService>();
			this.GameStateService = this.ServiceProvider.GetService<IGameStateService>();
			this.MoveService = this.ServiceProvider.GetService<IMoveService>();
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

		[TestMethod]
		public void WhiteKnightAttackStartingPositionFromD5() {
			var fen = "rnbqkbnr/pppppppp/8/3N4/8/8/PPPPPPPP/R1BQKBNR b KQkq - 0 1";
			var gameState = TestUtility.GetGameState(this.GameStateService, fen);
			var attacks = gameState.Attacks.Where(a => a.AttackingSquare.Name == "d5").ToList();
			var squares = new List<int> { 18, 20, 25, 29, 41, 45, 50, 52 };
			TestUtility.ListContainsSquares(attacks, squares, PieceType.Knight);
			Assert.IsTrue(attacks.Count() == 8, "Wrong number of attacks.");
		}
	}
}