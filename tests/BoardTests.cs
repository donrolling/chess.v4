using chess.v4.engine.extensions;
using chess.v4.engine.interfaces;
using chess.v4.engine.reference;
using common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using tests.setup;

namespace tests {

	[TestClass]
	public class GameStateTests {
		public ICoordinateService CoordinateService { get; }
		public IGameStateService GameStateService { get; }

		public GameStateTests() {
			var serviceProvider = new TestSetup().Setup();
			this.CoordinateService = serviceProvider.GetService<ICoordinateService>();
			this.GameStateService = serviceProvider.GetService<IGameStateService>();
		}

		[TestMethod]
		public void Given_StartPosition_AllPositions_AreCorrect() {
			//set up default position and test that it is correct
			var gamestateResult = GameStateService.Initialize(GeneralReference.Starting_FEN_Position);
			Assert.IsTrue(gamestateResult.Sucess);
			Assert.AreEqual(64, gamestateResult.Result.Squares.Count());
			var squares = gamestateResult.Result.Squares;

			//make sure we have the right number of pieces
			var thePieces = squares.Where(a => a.Index <= 15 || a.Index >= 48);
			Assert.AreEqual(32, thePieces.Count());

			//make sure we have the right number of empty squares
			var allOthers = squares.Where(a => a.Index > 15 && a.Index < 48);
			Assert.AreEqual(32, allOthers.Count());

			//make sure the right pieces are on the right squares
			Assert.AreEqual('r', squares.GetPiece(56).Identity);
			Assert.AreEqual('n', squares.GetPiece(57).Identity);
			Assert.AreEqual('b', squares.GetPiece(58).Identity);
			Assert.AreEqual('q', squares.GetPiece(59).Identity);
			Assert.AreEqual('k', squares.GetPiece(60).Identity);
			Assert.AreEqual('b', squares.GetPiece(61).Identity);
			Assert.AreEqual('n', squares.GetPiece(62).Identity);
			Assert.AreEqual('r', squares.GetPiece(63).Identity);

			Assert.AreEqual('p', squares.GetPiece(48).Identity);
			Assert.AreEqual('p', squares.GetPiece(49).Identity);
			Assert.AreEqual('p', squares.GetPiece(50).Identity);
			Assert.AreEqual('p', squares.GetPiece(51).Identity);
			Assert.AreEqual('p', squares.GetPiece(52).Identity);
			Assert.AreEqual('p', squares.GetPiece(53).Identity);
			Assert.AreEqual('p', squares.GetPiece(54).Identity);
			Assert.AreEqual('p', squares.GetPiece(55).Identity);

			Assert.AreEqual('R', squares.GetPiece(0).Identity);
			Assert.AreEqual('N', squares.GetPiece(1).Identity);
			Assert.AreEqual('B', squares.GetPiece(2).Identity);
			Assert.AreEqual('Q', squares.GetPiece(3).Identity);
			Assert.AreEqual('K', squares.GetPiece(4).Identity);
			Assert.AreEqual('B', squares.GetPiece(5).Identity);
			Assert.AreEqual('N', squares.GetPiece(6).Identity);
			Assert.AreEqual('R', squares.GetPiece(7).Identity);

			Assert.AreEqual('P', squares.GetPiece(8).Identity);
			Assert.AreEqual('P', squares.GetPiece(9).Identity);
			Assert.AreEqual('P', squares.GetPiece(10).Identity);
			Assert.AreEqual('P', squares.GetPiece(11).Identity);
			Assert.AreEqual('P', squares.GetPiece(12).Identity);
			Assert.AreEqual('P', squares.GetPiece(13).Identity);
			Assert.AreEqual('P', squares.GetPiece(14).Identity);
			Assert.AreEqual('P', squares.GetPiece(15).Identity);
		}

		[TestMethod]
		public void Given_StartPosition_WhenMakeMove_FEN_MatchesExpectation_CastleAvailability_IsCorrect() {
			var fen = "rnbqkbnr/pp1ppppp/8/2p5/4P3/2N5/PPPP1PPP/R1BQKBNR b KQkq - 1 2";
			var gamestateResult = GameStateService.Initialize(fen);
			Assert.IsTrue(gamestateResult.Sucess);
			//testing castle availability
			//1. e4 c5 2. Nc3 b5
			gamestateResult = GameStateService.MakeMove(gamestateResult.Result, "b7", "b5");
			Assert.IsTrue(gamestateResult.Sucess);
			Assert.IsTrue(gamestateResult.Result.Squares.Count == 32);
			Assert.AreEqual("rnbqkbnr/p2ppppp/8/1pp5/4P3/2N5/PPPP1PPP/R1BQKBNR w KQkq b6 0 3", gamestateResult.ToString());
			//1. e4 c5 2. Nc3 b5 3. b3
			gamestateResult = GameStateService.MakeMove(gamestateResult.Result, "b2", "b3");
			Assert.IsTrue(gamestateResult.Sucess);
			Assert.IsTrue(gamestateResult.Result.Squares.Count == 32);
			Assert.AreEqual("rnbqkbnr/p2ppppp/8/1pp5/4P3/1PN5/P1PP1PPP/R1BQKBNR b KQkq - 0 3", gamestateResult.ToString());
			//1. e4 c5 2. Nc3 b5 3. b3 Nc6
			gamestateResult = GameStateService.MakeMove(gamestateResult.Result, "b8", "c6");
			Assert.IsTrue(gamestateResult.Sucess);
			Assert.IsTrue(gamestateResult.Result.Squares.Count == 32);
			Assert.AreEqual("r1bqkbnr/p2ppppp/2n5/1pp5/4P3/1PN5/P1PP1PPP/R1BQKBNR w KQkq - 1 4", gamestateResult.ToString());
			//1. e4 c5 2. Nc3 b5 3. b3 Nc6 4. Ba3
			gamestateResult = GameStateService.MakeMove(gamestateResult.Result, "c1", "a3");
			Assert.IsTrue(gamestateResult.Sucess);
			Assert.IsTrue(gamestateResult.Result.Squares.Count == 32);
			Assert.AreEqual("r1bqkbnr/p2ppppp/2n5/1pp5/4P3/BPN5/P1PP1PPP/R2QKBNR b KQkq - 2 4", gamestateResult.ToString());
			//1. e4 c5 2. Nc3 b5 3. b3 Nc6 4. Ba3 Ba6
			gamestateResult = GameStateService.MakeMove(gamestateResult.Result, "c8", "a6");
			Assert.IsTrue(gamestateResult.Sucess);
			Assert.IsTrue(gamestateResult.Result.Squares.Count == 32);
			Assert.AreEqual("r2qkbnr/p2ppppp/b1n5/1pp5/4P3/BPN5/P1PP1PPP/R2QKBNR w KQkq - 3 5", gamestateResult.ToString());
			//1. e4 c5 2. Nc3 b5 3. b3 Nc6 4. Ba3 Ba6 5. Rb1
			gamestateResult = GameStateService.MakeMove(gamestateResult.Result, "a1", "b1");
			Assert.IsTrue(gamestateResult.Sucess);
			Assert.IsTrue(gamestateResult.Result.Squares.Count == 32);
			Assert.AreEqual("r2qkbnr/p2ppppp/b1n5/1pp5/4P3/BPN5/P1PP1PPP/1R1QKBNR b Kkq - 4 5", gamestateResult.ToString());
			//1. e4 c5 2. Nc3 b5 3. b3 Nc6 4. Ba3 Ba6 5. Rb1 Rb8
			gamestateResult = GameStateService.MakeMove(gamestateResult.Result, "a8", "b8");
			Assert.IsTrue(gamestateResult.Sucess);
			Assert.IsTrue(gamestateResult.Result.Squares.Count == 32);
			Assert.AreEqual("1r1qkbnr/p2ppppp/b1n5/1pp5/4P3/BPN5/P1PP1PPP/1R1QKBNR w Kk - 5 6", gamestateResult.ToString());
		}

		[TestMethod]
		public void Given_StartPosition_WhenMakeMove_FEN_MatchesExpectation_EnPassantTargetSquare_IsCorrect() {
			//testing en passant target square
			var gamestateResult = GameStateService.Initialize(GeneralReference.Starting_FEN_Position);
			Assert.IsTrue(gamestateResult.Sucess);
			//1. e4
			gamestateResult = GameStateService.MakeMove(gamestateResult.Result, "e2", "e4");
			Assert.IsTrue(gamestateResult.Sucess);
			Assert.IsTrue(gamestateResult.Result.Squares.GetPiece(28).Identity == 'P');
			Assert.AreEqual("rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq e3 0 1", gamestateResult.Result.ToString());
			//1. e4 c5
			//testing fullmove number
			gamestateResult = GameStateService.MakeMove(gamestateResult.Result, "c7", "c5");
			Assert.IsTrue(gamestateResult.Sucess);
			Assert.IsTrue(gamestateResult.Result.Squares.GetPiece(34).Identity == 'p');
			Assert.AreEqual("rnbqkbnr/pp1ppppp/8/2p5/4P3/8/PPPP1PPP/RNBQKBNR w KQkq c6 0 2", gamestateResult.ToString());
			//1. e4 c5 2. Nc3
			//testing the halfmove clock
			gamestateResult = GameStateService.MakeMove(gamestateResult.Result, "b1", "c3");
			Assert.IsTrue(gamestateResult.Sucess);
			Assert.IsTrue(gamestateResult.Result.Squares.GetPiece(18).Identity == 'N');
			Assert.AreEqual("rnbqkbnr/pp1ppppp/8/2p5/4P3/2N5/PPPP1PPP/R1BQKBNR b KQkq - 1 2", gamestateResult.ToString());
		}

		[TestMethod]
		public void ShitIsFucked() {
			var fen = "rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq e3 0 1";
			var gamestateResult = GameStateService.Initialize(fen);
			Assert.IsTrue(gamestateResult.Sucess);
		}
	}
}