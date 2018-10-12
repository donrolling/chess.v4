using chess.v4.engine.enumeration;
using chess.v4.engine.extensions;
using chess.v4.engine.model;
using chess.v4.engine.reference;
using chess.v4.engine.service;
using Chess.ServiceLayer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace tests {

	[TestClass]
	public class GameStateTests {
		public GameStateService GameStateService { get; }
		public CoordinateService CoordinateService { get; }
		public MoveService MoveService { get; }
		public NotationService NotationService { get; }
		public AttackService AttackService { get; }

		public GameStateTests() {
			this.CoordinateService = new CoordinateService();
			this.MoveService = new MoveService();
			this.NotationService = new NotationService(this.CoordinateService);
			this.AttackService = new AttackService(this.NotationService, this.CoordinateService);
			this.GameStateService = new GameStateService(this.NotationService, this.CoordinateService, this.MoveService, this.AttackService);
		}

		[TestMethod]
		public void UpdateTest() {
			//set up default position and test that it is correct
			var gamestateResult = GameStateService.SetStartPosition(FEN.StartingPosition);
			Assert.AreEqual(64, gamestateResult.Output.Squares.Count());
			AssertStartingPosition(gamestateResult.Output.Squares);
			//now change the board and test if the change took place
			//testing position
			//testing en passant target square
			//1. e4
			var pos1 = CoordinateService.CoordinateToPosition("e2");
			var pos2 = CoordinateService.CoordinateToPosition("e4");
			gamestateResult = GameStateService.UpdateGameState(gamestateResult.Output, Color.White, pos1, pos2, string.Empty);
			Assert.IsTrue(gamestateResult.Output.Squares.GetPiece(28).Identity == 'P');
			Assert.AreEqual("rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq e3 0 1", gamestateResult.Output.FEN);
			//1. e4 c5
			//testing position
			//testing en passant target square
			//testing fullmove number
			pos1 = CoordinateService.CoordinateToPosition("c7");
			pos2 = CoordinateService.CoordinateToPosition("c5");
			gamestateResult = GameStateService.UpdateGameState(gamestateResult.Output, Color.Black, pos1, pos2, string.Empty);
			Assert.IsTrue(gamestateResult.Output.Squares.GetPiece(34).Identity == 'p');
			Assert.AreEqual("rnbqkbnr/pp1ppppp/8/2p5/4P3/8/PPPP1PPP/RNBQKBNR w KQkq c6 0 2", gamestateResult.Output.FEN);
			//1. e4 c5 2. Nc3
			//testing the halfmove clock
			pos1 = CoordinateService.CoordinateToPosition("b1");
			pos2 = CoordinateService.CoordinateToPosition("c3");
			gamestateResult = GameStateService.UpdateGameState(gamestateResult.Output, Color.White, pos1, pos2, string.Empty);
			Assert.IsTrue(gamestateResult.Output.Squares.GetPiece(18).Identity == 'N');
			Assert.AreEqual("rnbqkbnr/pp1ppppp/8/2p5/4P3/2N5/PPPP1PPP/R1BQKBNR b KQkq - 1 2", gamestateResult.Output.FEN);

			//testing castle availability
			//1. e4 c5 2. Nc3 b5
			pos1 = CoordinateService.CoordinateToPosition("b7");
			pos2 = CoordinateService.CoordinateToPosition("b5");
			gamestateResult = GameStateService.UpdateGameState(gamestateResult.Output, Color.Black, pos1, pos2, string.Empty);
			Assert.IsTrue(gamestateResult.Output.Squares.Count == 32);
			Assert.AreEqual("rnbqkbnr/p2ppppp/8/1pp5/4P3/2N5/PPPP1PPP/R1BQKBNR w KQkq b6 0 3", gamestateResult.Output.FEN);
			//1. e4 c5 2. Nc3 b5 3. b3
			pos1 = CoordinateService.CoordinateToPosition("b2");
			pos2 = CoordinateService.CoordinateToPosition("b3");
			gamestateResult = GameStateService.UpdateGameState(gamestateResult.Output, Color.White, pos1, pos2, string.Empty);
			Assert.IsTrue(gamestateResult.Output.Squares.Count == 32);
			Assert.AreEqual("rnbqkbnr/p2ppppp/8/1pp5/4P3/1PN5/P1PP1PPP/R1BQKBNR b KQkq - 0 3", gamestateResult.Output.FEN);
			//1. e4 c5 2. Nc3 b5 3. b3 Nc6
			pos1 = CoordinateService.CoordinateToPosition("b8");
			pos2 = CoordinateService.CoordinateToPosition("c6");
			gamestateResult = GameStateService.UpdateGameState(gamestateResult.Output, Color.Black, pos1, pos2, string.Empty);
			Assert.IsTrue(gamestateResult.Output.Squares.Count == 32);
			Assert.AreEqual("r1bqkbnr/p2ppppp/2n5/1pp5/4P3/1PN5/P1PP1PPP/R1BQKBNR w KQkq - 1 4", gamestateResult.Output.FEN);
			//1. e4 c5 2. Nc3 b5 3. b3 Nc6 4. Ba3
			pos1 = CoordinateService.CoordinateToPosition("c1");
			pos2 = CoordinateService.CoordinateToPosition("a3");
			gamestateResult = GameStateService.UpdateGameState(gamestateResult.Output, Color.White, pos1, pos2, string.Empty);
			Assert.IsTrue(gamestateResult.Output.Squares.Count == 32);
			Assert.AreEqual("r1bqkbnr/p2ppppp/2n5/1pp5/4P3/BPN5/P1PP1PPP/R2QKBNR b KQkq - 2 4", gamestateResult.Output.FEN);
			//1. e4 c5 2. Nc3 b5 3. b3 Nc6 4. Ba3 Ba6
			pos1 = CoordinateService.CoordinateToPosition("c8");
			pos2 = CoordinateService.CoordinateToPosition("a6");
			gamestateResult = GameStateService.UpdateGameState(gamestateResult.Output, Color.Black, pos1, pos2, string.Empty);
			Assert.IsTrue(gamestateResult.Output.Squares.Count == 32);
			Assert.AreEqual("r2qkbnr/p2ppppp/b1n5/1pp5/4P3/BPN5/P1PP1PPP/R2QKBNR w KQkq - 3 5", gamestateResult.Output.FEN);
			//1. e4 c5 2. Nc3 b5 3. b3 Nc6 4. Ba3 Ba6 5. Rb1
			pos1 = CoordinateService.CoordinateToPosition("a1");
			pos2 = CoordinateService.CoordinateToPosition("b1");
			gamestateResult = GameStateService.UpdateGameState(gamestateResult.Output, Color.White, pos1, pos2, string.Empty);
			Assert.IsTrue(gamestateResult.Output.Squares.Count == 32);
			Assert.AreEqual("r2qkbnr/p2ppppp/b1n5/1pp5/4P3/BPN5/P1PP1PPP/1R1QKBNR b Kkq - 4 5", gamestateResult.Output.FEN);
			//1. e4 c5 2. Nc3 b5 3. b3 Nc6 4. Ba3 Ba6 5. Rb1 Rb8
			pos1 = CoordinateService.CoordinateToPosition("a8");
			pos2 = CoordinateService.CoordinateToPosition("b8");
			gamestateResult = GameStateService.UpdateGameState(gamestateResult.Output, Color.Black, pos1, pos2, string.Empty);
			Assert.IsTrue(gamestateResult.Output.Squares.Count == 32);
			Assert.AreEqual("1r1qkbnr/p2ppppp/b1n5/1pp5/4P3/BPN5/P1PP1PPP/1R1QKBNR w Kk - 5 6", gamestateResult.Output.FEN);
		}

		private void AssertStartingPosition(List<Square> squares) {
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
	}
}