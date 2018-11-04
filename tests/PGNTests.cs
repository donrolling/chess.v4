using chess.v4.models.enumeration;
using chess.v4.engine.interfaces;
using chess.v4.models;
using common;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NLog;
using System.Linq;
using System.Text;
using tests.models;
using tests.setup;

namespace tests {

	[TestClass]
	public class PGNTests : BaseTest {
		
		public IGameStateService GameStateService { get; }
		public IPGNService PGNService { get; }

		public PGNTests() {
			var serviceProvider = new TestSetup().Setup();
			
			this.GameStateService = serviceProvider.GetService<IGameStateService>();
			this.PGNService = serviceProvider.GetService<IPGNService>();
		}

		[TestMethod]
		public void AllAttacks() {
			var gameStateResult = this.GameStateService.Initialize();
			Assert.IsTrue(gameStateResult.Success);
			var gameState = gameStateResult.Result;

			var positions = PGNService.PGNMoveToSquarePair(gameState, "e4");
			Assert.AreEqual(12, positions.Item1);
			Assert.AreEqual(28, positions.Item2);

			var positions2 = PGNService.PGNMoveToSquarePair(gameState, "e3");
			Assert.AreEqual(12, positions2.Item1);
			Assert.AreEqual(20, positions2.Item2);
		}

		[TestMethod]
		public void Basic() {
			Assert.AreEqual(PieceType.King, PGNService.GetPieceTypeFromPGNMove("Kb2"));
			Assert.AreEqual(PieceType.Queen, PGNService.GetPieceTypeFromPGNMove("Qa2"));
			Assert.AreEqual(PieceType.Rook, PGNService.GetPieceTypeFromPGNMove("Ra2"));
			Assert.AreEqual(PieceType.Bishop, PGNService.GetPieceTypeFromPGNMove("Bb2"));
			Assert.AreEqual(PieceType.Knight, PGNService.GetPieceTypeFromPGNMove("Na2"));
			Assert.AreEqual(PieceType.Pawn, PGNService.GetPieceTypeFromPGNMove("a2"));

			Assert.AreEqual(PieceType.King, PGNService.GetPieceTypeFromPGNMove("kb2"));
			Assert.AreEqual(PieceType.Queen, PGNService.GetPieceTypeFromPGNMove("qa2"));
			Assert.AreEqual(PieceType.Rook, PGNService.GetPieceTypeFromPGNMove("ra2"));
			Assert.AreEqual(PieceType.Bishop, PGNService.GetPieceTypeFromPGNMove("bb2"));
			Assert.AreEqual(PieceType.Knight, PGNService.GetPieceTypeFromPGNMove("na2"));
			Assert.AreEqual(PieceType.Pawn, PGNService.GetPieceTypeFromPGNMove("a2"));
		}

		[TestMethod]
		public void PGNMoveDifferentiation() {
			//create a situation where the engine has to differentiate between two rooks of the same color on a single file
			var fen = "4r3/N2R3p/1k2BPp1/8/8/2P5/PP3PPP/3R2K1 w - - 0 30";
			var gameStateResult = this.GameStateService.Initialize(fen);
			Assert.IsTrue(gameStateResult.Success);
			var gameState = gameStateResult.Result;
			var piece = new Piece(PieceType.Rook, Color.White);
			//assert that two white rooks exist
			var rooks = gameState.Squares.Where(a =>
											a.Occupied && a.Piece.PieceType == PieceType.Rook
											&& a.Piece.Color == Color.White);
			Assert.AreEqual(2, rooks.Count());
			//assert that both white rooks have attacks
			var rookAttacks = gameState.Attacks.Where(a =>
											a.AttackingSquare.Piece.PieceType == PieceType.Rook
											&& a.AttackingSquare.Piece.Color == Color.White);
			var attackMessage = new StringBuilder();
			foreach (var rookAttack in rookAttacks) {
				attackMessage.AppendLine($"\r\n==========================\r\n\tAttackerSquare: { rookAttack.AttackingSquare.Index }\r\n\tAttacked Square: { rookAttack.Index }");
			}
			this.Logger.LogInformation(attackMessage.ToString());
			Assert.AreEqual(22, rookAttacks.Count());
			var pos = PGNService.GetCurrentPositionFromPGNMove(gameState, piece, 43, "R1d6");
			Assert.AreEqual(3, pos.Index);
			//should make a test that asserts that this is check
		}
	}
}