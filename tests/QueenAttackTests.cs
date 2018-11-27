using chess.v4.engine.interfaces;
using chess.v4.models.enumeration;
using Common.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Tests.Models;
using Tests.Utility;

namespace Tests {
	[TestClass]
	public class QueenAttackTests : TestBase {
		public IAttackService AttackService { get; }
		public IGameStateService GameStateService { get; }
		public IMoveService MoveService { get; }

		public QueenAttackTests() {
			this.AttackService = this.ServiceProvider.GetService<IAttackService>();
			this.GameStateService = this.ServiceProvider.GetService<IGameStateService>();
			this.MoveService = this.ServiceProvider.GetService<IMoveService>();
		}

		[TestMethod]
		public void WhiteQueenAttacksEmptyBoardFromD4() {
			//only kings and a queen on the board
			var fen = "7k/8/8/8/3Q4/8/8/7K b - - 0 32";
			var gameState = TestUtility.GetGameState(this.GameStateService, fen);
			var attacks = gameState.Attacks.Where(a => a.AttackingSquare.Name == "d4").ToList();
			var allSquareIndexs = new int[] { 0, 3, 6, 9, 11, 13, 18, 19, 20, 24, 25, 26, 28, 29, 30, 31, 34, 35, 36, 41, 43, 45, 48, 51, 54, 59, 63 };
			TestUtility.ListContainsSquares(attacks, allSquareIndexs.ToList(), PieceType.Queen);
		}

		[TestMethod]
		public void WhiteQueenAttacksStartingPosition() {
			var gameState = TestUtility.GetGameState(this.GameStateService);
			var whiteQueenAttacks = gameState.Attacks.Where(a => 
				a.AttackingSquare.Name == "d1" 
				&& !a.IsProtecting
			);
			Assert.AreEqual(0, whiteQueenAttacks.Count());
		}
	}
}