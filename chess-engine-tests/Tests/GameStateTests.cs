using chess_engine.Engine.Extensions;
using chess_engine.Engine.Interfaces;
using chess_engine.Engine.Reference;
using chess_engine_tests.Application;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace chess_engine_tests.Tests
{
	[TestClass]
	public class GameStateTests : TestBase
	{
		private readonly IGameStateService _gameStateService;

		public GameStateTests()
		{
			_gameStateService = ServiceProvider.GetService<IGameStateService>();
		}

		[TestMethod]
		public void Given_ProblematicStartPosition()
		{
			var fen = "r1bqkbnr/p2ppppp/2n5/1pp5/4P3/1PN5/P1PP1PPP/R1BQKBNR w KQkq - 1 4";
			var gameStateResult = _gameStateService.Initialize(fen);
			Assert.IsTrue(gameStateResult.Success);
			gameStateResult = _gameStateService.MakeMove(gameStateResult.Result, "c1", "a3");
			Assert.IsTrue(gameStateResult.Success, gameStateResult.Message);
			Assert.AreEqual("r1bqkbnr/p2ppppp/2n5/1pp5/4P3/BPN5/P1PP1PPP/R2QKBNR b KQkq - 2 4", gameStateResult.Result.ToString());
		}

		[TestMethod]
		public void Given_StartPosition_AllPositions_AreCorrect()
		{
			//set up default position and test that it is correct
			var gamestateResult = _gameStateService.Initialize(GeneralReference.Starting_FEN_Position);
			Assert.IsTrue(gamestateResult.Success);
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
		public void Given_StartPosition_WhenMakeMove_FEN_MatchesExpectation_CastleAvailability_IsCorrect()
		{
			var fen = "rnbqkbnr/pp1ppppp/8/2p5/4P3/2N5/PPPP1PPP/R1BQKBNR b KQkq - 1 2";
			var gameStateResult = _gameStateService.Initialize(fen);
			Assert.IsTrue(gameStateResult.Success);
			//testing castle availability
			//1. e4 c5 2. Nc3 b5
			gameStateResult = _gameStateService.MakeMove(gameStateResult.Result, "b7", "b5");
			Assert.IsTrue(gameStateResult.Success, gameStateResult.Message);
			Assert.AreEqual("rnbqkbnr/p2ppppp/8/1pp5/4P3/2N5/PPPP1PPP/R1BQKBNR w KQkq b6 0 3", gameStateResult.Result.ToString());
			//1. e4 c5 2. Nc3 b5 3. b3
			gameStateResult = _gameStateService.MakeMove(gameStateResult.Result, "b2", "b3");
			Assert.IsTrue(gameStateResult.Success, gameStateResult.Message);
			Assert.AreEqual("rnbqkbnr/p2ppppp/8/1pp5/4P3/1PN5/P1PP1PPP/R1BQKBNR b KQkq - 0 3", gameStateResult.Result.ToString());
			//1. e4 c5 2. Nc3 b5 3. b3 Nc6
			gameStateResult = _gameStateService.MakeMove(gameStateResult.Result, "b8", "c6");
			Assert.IsTrue(gameStateResult.Success, gameStateResult.Message);
			Assert.AreEqual("r1bqkbnr/p2ppppp/2n5/1pp5/4P3/1PN5/P1PP1PPP/R1BQKBNR w KQkq - 1 4", gameStateResult.Result.ToString());
			//1. e4 c5 2. Nc3 b5 3. b3 Nc6 4. Ba3
			gameStateResult = _gameStateService.MakeMove(gameStateResult.Result, "c1", "a3");
			Assert.IsTrue(gameStateResult.Success, gameStateResult.Message);
			Assert.AreEqual("r1bqkbnr/p2ppppp/2n5/1pp5/4P3/BPN5/P1PP1PPP/R2QKBNR b KQkq - 2 4", gameStateResult.Result.ToString());
			//1. e4 c5 2. Nc3 b5 3. b3 Nc6 4. Ba3 Ba6
			gameStateResult = _gameStateService.MakeMove(gameStateResult.Result, "c8", "a6");
			Assert.IsTrue(gameStateResult.Success, gameStateResult.Message);
			Assert.AreEqual("r2qkbnr/p2ppppp/b1n5/1pp5/4P3/BPN5/P1PP1PPP/R2QKBNR w KQkq - 3 5", gameStateResult.Result.ToString());
			//1. e4 c5 2. Nc3 b5 3. b3 Nc6 4. Ba3 Ba6 5. Rb1
			gameStateResult = _gameStateService.MakeMove(gameStateResult.Result, "a1", "b1");
			Assert.IsTrue(gameStateResult.Success, gameStateResult.Message);
			Assert.AreEqual("r2qkbnr/p2ppppp/b1n5/1pp5/4P3/BPN5/P1PP1PPP/1R1QKBNR b Kkq - 4 5", gameStateResult.Result.ToString());
			//1. e4 c5 2. Nc3 b5 3. b3 Nc6 4. Ba3 Ba6 5. Rb1 Rb8
			gameStateResult = _gameStateService.MakeMove(gameStateResult.Result, "a8", "b8");
			Assert.IsTrue(gameStateResult.Success, gameStateResult.Message);

			Assert.AreEqual("1r1qkbnr/p2ppppp/b1n5/1pp5/4P3/BPN5/P1PP1PPP/1R1QKBNR w Kk - 5 6", gameStateResult.Result.ToString());
		}

		[TestMethod]
		public void Given_StartPosition_WhenMakeMove_FEN_MatchesExpectation_EnPassantTargetSquare_IsCorrect()
		{
			//testing en passant target square
			var gameStateResult = _gameStateService.Initialize(GeneralReference.Starting_FEN_Position);
			Assert.IsTrue(gameStateResult.Success);
			//1. e4
			gameStateResult = _gameStateService.MakeMove(gameStateResult.Result, "e2", "e4");
			Assert.IsTrue(gameStateResult.Success, gameStateResult.Message);
			Assert.IsTrue(gameStateResult.Result.Squares.GetPiece(28).Identity == 'P');
			Assert.AreEqual("rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq e3 0 1", gameStateResult.Result.ToString());
			//1. e4 c5
			//testing fullmove number
			gameStateResult = _gameStateService.MakeMove(gameStateResult.Result, "c7", "c5");
			Assert.IsTrue(gameStateResult.Success, gameStateResult.Message);
			Assert.IsTrue(gameStateResult.Result.Squares.GetPiece(34).Identity == 'p');
			Assert.AreEqual("rnbqkbnr/pp1ppppp/8/2p5/4P3/8/PPPP1PPP/RNBQKBNR w KQkq c6 0 2", gameStateResult.Result.ToString());
			//1. e4 c5 2. Nc3
			//testing the halfmove clock
			gameStateResult = _gameStateService.MakeMove(gameStateResult.Result, "b1", "c3");
			Assert.IsTrue(gameStateResult.Success, gameStateResult.Message);
			Assert.IsTrue(gameStateResult.Result.Squares.GetPiece(18).Identity == 'N');
			Assert.AreEqual("rnbqkbnr/pp1ppppp/8/2p5/4P3/2N5/PPPP1PPP/R1BQKBNR b KQkq - 1 2", gameStateResult.Result.ToString());
		}
	}
}