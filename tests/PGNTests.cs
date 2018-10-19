using chess.v4.engine.enumeration;
using chess.v4.engine.interfaces;
using common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using tests.setup;

namespace tests {

	[TestClass]
	public class PGNTests {
		public ICoordinateService CoordinateService { get; }
		public IGameStateService GameStateService { get; }
		public IPGNService PGNService { get; }

		public PGNTests() {
			var serviceProvider = new TestSetup().Setup();
			this.CoordinateService = serviceProvider.GetService<ICoordinateService>();
			this.GameStateService = serviceProvider.GetService<IGameStateService>();
			this.PGNService = serviceProvider.GetService<IPGNService>();
		}

		[TestMethod]
		public void AllAttacks() {
			var gameStateResult = this.GameStateService.Initialize();
			Assert.IsTrue(gameStateResult.Sucess);
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
			var fen = "4r3/N2R3p/1k2BPp1/8/8/2P5/PP3PPP/3R2K1 w - - 0 30";
			var gameStateResult = this.GameStateService.Initialize(fen);
			Assert.IsTrue(gameStateResult.Sucess);
			var gameState = gameStateResult.Result;
			var pos = PGNService.GetCurrentPositionFromPGNMove(gameState, PieceType.Rook, Color.White, 43, "R1d6");
			Assert.AreEqual(3, pos);
		}
	}
}