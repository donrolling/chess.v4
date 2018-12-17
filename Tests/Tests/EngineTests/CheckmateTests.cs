using chess.v4.engine.interfaces;
using chess.v4.engine.Utility;
using chess.v4.models.enumeration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Tests.Models;
using Tests.Utility;

namespace Tests {
	[TestClass]
	public class CheckmateTests : TestBase {
		public IGameStateService GameStateService { get; }

		public CheckmateTests() {
			this.GameStateService = this.ServiceProvider.GetService<IGameStateService>();
		}

		[TestMethod]
		public void Given_CheckmateFEN_GameStateShouldReflectCheckmate() {
			var fen = "7Q/4b3/r5pk/8/5PK1/8/8/8 b - - 1 69";
			var gameState = TestUtility.GetGameState(this.GameStateService, fen);
			var blackKingAttacks = gameState.Attacks
										.Where(a => 
											a.AttackingSquare.Piece.PieceType == PieceType.King
											&& a.AttackingSquare.Piece.Color == Color.Black
											&& !a.IsProtecting
										);
			Assert.IsFalse(blackKingAttacks.Any());
			Assert.IsTrue(gameState.StateInfo.IsCheckmate);
		}
	}
}