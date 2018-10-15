﻿using chess.v4.engine.extensions;
using chess.v4.engine.reference;
using chess.v4.engine.service;
using Chess.ServiceLayer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace tests {

	[TestClass]
	public class GameStateTests {
		public AttackService AttackService { get; }
		public CoordinateService CoordinateService { get; }
		public GameStateService GameStateService { get; }
		public MoveService MoveService { get; }
		public NotationService NotationService { get; }

		public GameStateTests() {
			this.CoordinateService = new CoordinateService();
			this.NotationService = new NotationService(this.CoordinateService);
			this.AttackService = new AttackService(this.NotationService, this.CoordinateService);
			this.MoveService = new MoveService(this.CoordinateService, this.AttackService);
			this.GameStateService = new GameStateService(this.NotationService, this.CoordinateService, this.MoveService, this.AttackService);
		}

		[TestMethod]
		public void Given_StartPosition_AllPositions_AreCorrect() {
			//set up default position and test that it is correct
			var gamestateResult = GameStateService.SetStartPosition(GeneralReference.Starting_FEN_Position);
			Assert.IsTrue(gamestateResult.Sucess);
			Assert.AreEqual(64, gamestateResult.Output.Squares.Count());
			var squares = gamestateResult.Output.Squares;

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
			var gamestateResult = GameStateService.SetStartPosition(fen);
			Assert.IsTrue(gamestateResult.Sucess);
			//testing castle availability
			//1. e4 c5 2. Nc3 b5
			gamestateResult = GameStateService.MakeMove(gamestateResult.Output, "b7", "b5");
			Assert.IsTrue(gamestateResult.Output.Squares.Count == 32);
			Assert.AreEqual("rnbqkbnr/p2ppppp/8/1pp5/4P3/2N5/PPPP1PPP/R1BQKBNR w KQkq b6 0 3", gamestateResult.ToString());
			//1. e4 c5 2. Nc3 b5 3. b3
			gamestateResult = GameStateService.MakeMove(gamestateResult.Output, "b2", "b3");
			Assert.IsTrue(gamestateResult.Output.Squares.Count == 32);
			Assert.AreEqual("rnbqkbnr/p2ppppp/8/1pp5/4P3/1PN5/P1PP1PPP/R1BQKBNR b KQkq - 0 3", gamestateResult.ToString());
			//1. e4 c5 2. Nc3 b5 3. b3 Nc6
			gamestateResult = GameStateService.MakeMove(gamestateResult.Output, "b8", "c6");
			Assert.IsTrue(gamestateResult.Output.Squares.Count == 32);
			Assert.AreEqual("r1bqkbnr/p2ppppp/2n5/1pp5/4P3/1PN5/P1PP1PPP/R1BQKBNR w KQkq - 1 4", gamestateResult.ToString());
			//1. e4 c5 2. Nc3 b5 3. b3 Nc6 4. Ba3
			gamestateResult = GameStateService.MakeMove(gamestateResult.Output, "c1", "a3");
			Assert.IsTrue(gamestateResult.Output.Squares.Count == 32);
			Assert.AreEqual("r1bqkbnr/p2ppppp/2n5/1pp5/4P3/BPN5/P1PP1PPP/R2QKBNR b KQkq - 2 4", gamestateResult.ToString());
			//1. e4 c5 2. Nc3 b5 3. b3 Nc6 4. Ba3 Ba6
			gamestateResult = GameStateService.MakeMove(gamestateResult.Output, "c8", "a6");
			Assert.IsTrue(gamestateResult.Output.Squares.Count == 32);
			Assert.AreEqual("r2qkbnr/p2ppppp/b1n5/1pp5/4P3/BPN5/P1PP1PPP/R2QKBNR w KQkq - 3 5", gamestateResult.ToString());
			//1. e4 c5 2. Nc3 b5 3. b3 Nc6 4. Ba3 Ba6 5. Rb1
			gamestateResult = GameStateService.MakeMove(gamestateResult.Output, "a1", "b1");
			Assert.IsTrue(gamestateResult.Output.Squares.Count == 32);
			Assert.AreEqual("r2qkbnr/p2ppppp/b1n5/1pp5/4P3/BPN5/P1PP1PPP/1R1QKBNR b Kkq - 4 5", gamestateResult.ToString());
			//1. e4 c5 2. Nc3 b5 3. b3 Nc6 4. Ba3 Ba6 5. Rb1 Rb8
			gamestateResult = GameStateService.MakeMove(gamestateResult.Output, "a8", "b8");
			Assert.IsTrue(gamestateResult.Output.Squares.Count == 32);
			Assert.AreEqual("1r1qkbnr/p2ppppp/b1n5/1pp5/4P3/BPN5/P1PP1PPP/1R1QKBNR w Kk - 5 6", gamestateResult.ToString());
		}
		
		[TestMethod]
		public void ShitIsFucked() {
			var fen = "rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq e3 0 1";
			var gamestateResult = GameStateService.SetStartPosition(fen);
			Assert.IsTrue(gamestateResult.Sucess);
		}

		[TestMethod]
		public void Given_StartPosition_WhenMakeMove_FEN_MatchesExpectation_EnPassantTargetSquare_IsCorrect() {
			//testing en passant target square
			var gamestateResult = GameStateService.SetStartPosition(GeneralReference.Starting_FEN_Position);
			Assert.IsTrue(gamestateResult.Sucess);
			//1. e4
			gamestateResult = GameStateService.MakeMove(gamestateResult.Output, "e2", "e4");
			Assert.IsTrue(gamestateResult.Output.Squares.GetPiece(28).Identity == 'P');
			Assert.AreEqual("rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq e3 0 1", gamestateResult.Output.ToString());
			//1. e4 c5
			//testing fullmove number
			gamestateResult = GameStateService.MakeMove(gamestateResult.Output, "c7", "c5");
			Assert.IsTrue(gamestateResult.Output.Squares.GetPiece(34).Identity == 'p');
			Assert.AreEqual("rnbqkbnr/pp1ppppp/8/2p5/4P3/8/PPPP1PPP/RNBQKBNR w KQkq c6 0 2", gamestateResult.ToString());
			//1. e4 c5 2. Nc3
			//testing the halfmove clock
			gamestateResult = GameStateService.MakeMove(gamestateResult.Output, "b1", "c3");
			Assert.IsTrue(gamestateResult.Output.Squares.GetPiece(18).Identity == 'N');
			Assert.AreEqual("rnbqkbnr/pp1ppppp/8/2p5/4P3/2N5/PPPP1PPP/R1BQKBNR b KQkq - 1 2", gamestateResult.ToString());
		}
	}
}