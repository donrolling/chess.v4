using Chess.v4.Engine.Interfaces;
using Chess.v4.Models.Enums;
using EngineTests.Models;
using EngineTests.Utility;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace EngineTests.Tests.EngineTests
{
	[TestClass]
	public class PawnAttackTests : TestBase
	{
		private readonly IGameStateService _gameStateService;

		public PawnAttackTests()
		{
			_gameStateService = ServiceProvider.GetService<IGameStateService>();
		}

		[TestMethod]
		public void Pawn_EnPassantWorks()
		{
			var fen = "2r5/6k1/p1q1p2p/3pP1p1/2pP1p2/PPR2P2/4QKPP/8 w - d6 0 34";
			var gameState = TestUtility.GetGameState(_gameStateService, fen);
			var gsr = _gameStateService.MakeMove(gameState, "exd6ep");
			Assert.IsTrue(gsr.Success, "En Passant should have worked here.");
			var assertFen = "2r5/6k1/p1qPp2p/6p1/2pP1p2/PPR2P2/4QKPP/8 b - - 0 34";
			var newFen = gsr.Result.ToString();
			Assert.AreEqual(assertFen, newFen, $"En Passant should have worked here.");
		}

		/// <summary>
		/// https://github.com/donrolling/chess.v4/issues/13
		/// https://en.wikipedia.org/wiki/Forsyth%E2%80%93Edwards_Notation#Examples
		/// </summary>
		[TestMethod]
		public void En_PassantNotAvailable_WhenPawnOn_3rdOr6thRank()
		{
			var fen = "1nbqkbnr/1ppppppp/P7/8/8/2BP3r/1PP1PPPP/RN1QKBNR b KQk - 0 7";
			var gameState = TestUtility.GetGameState(_gameStateService, fen);
			var gsr = _gameStateService.MakeMove(gameState, "b5");
			Assert.AreEqual(gsr.Result.EnPassantTargetSquare, "-", "En Passant should not be available here.");
		}

		/// <summary>
		/// https://github.com/donrolling/chess.v4/issues/13
		/// https://en.wikipedia.org/wiki/Forsyth%E2%80%93Edwards_Notation#Examples
		/// </summary>
		[TestMethod]
		public void En_PassantIsAvailable_WhenPawnOn_4thOr5thRank()
		{
			var fen = "1nbqkbnr/1ppppppp/8/P7/8/3r4/1PPBPPPP/RN1QKBNR b KQk - 0 6";
			var gameState = TestUtility.GetGameState(_gameStateService, fen);
			var gsr = _gameStateService.MakeMove(gameState, "b5");
			Assert.AreEqual(gsr.Result.EnPassantTargetSquare, "b6", "En Passant should be available here.");
		}

		[TestMethod]
		public void GameStart_Verify_PawnAttack()
		{
			var gameState = TestUtility.GetGameState(_gameStateService);
			//white queenside rook pawn, opening moves
			var attacks = gameState.Attacks.Where(a => a.AttackingSquare.Name == "a2" && !a.MayOnlyMoveHereIfOccupiedByEnemy).ToList();
			var squares = new List<int> { 16, 24 };
			TestUtility.ListContainsSquares(attacks, squares, PieceType.Pawn);
			Assert.AreEqual(2, attacks.Count());

			//white king pawn, opening moves
			attacks = gameState.Attacks.Where(a => a.AttackingSquare.Name == "e2" && !a.MayOnlyMoveHereIfOccupiedByEnemy).ToList();
			squares = new List<int> { 20, 28 };
			TestUtility.ListContainsSquares(attacks, squares, PieceType.Pawn);
			Assert.AreEqual(2, attacks.Count());

			//black
			attacks = gameState.Attacks.Where(a => a.AttackingSquare.Name == "d7" && !a.MayOnlyMoveHereIfOccupiedByEnemy).ToList();
			squares = new List<int> { 35, 43 };
			TestUtility.ListContainsSquares(attacks, squares, PieceType.Pawn);
			Assert.AreEqual(2, attacks.Count());
		}

		[TestMethod]
		public void PawnAnomalies()
		{
			var fen = "1k3r2/7p/4BPp1/1N1R4/8/2P5/PP3PPP/R5K1 w  - 3 26";
			var gameState = TestUtility.GetGameState(_gameStateService, fen);
			var attacks = gameState.Attacks.Where(a => a.AttackingSquare.Name == "f2" && !a.MayOnlyMoveHereIfOccupiedByEnemy).ToList();
			var squares = new List<int> { 21, 29 };
			TestUtility.ListContainsSquares(attacks, squares, PieceType.Pawn);
			Assert.AreEqual(2, attacks.Count());
			var gameStateResult = _gameStateService.MakeMove(gameState, 13, 20);
			Assert.IsTrue(gameStateResult.Failure, "This move was invalid, so the attempt to make it should fail.");
		}

		[TestMethod]
		public void PawnAttacksWhenOtherPawnIsBlocking()
		{
			var gameState = TestUtility.GetGameState(_gameStateService, "1r1r2k1/1bq1b1pp/p3p1p1/2np3P/4P3/2NBBP2/1PP2Q2/1K1R3R b - - 0 24");
			var attacks = gameState.Attacks.Where(a => a.AttackingSquare.Name == "g6").ToList();
			var squares = new List<int> { 37, 38, 39 };
			TestUtility.ListContainsSquares(attacks, squares, PieceType.Pawn);
			Assert.AreEqual(3, attacks.Count());
			//g7 should have no moves
			var forwardMoves = gameState.Attacks.Where(a => a.AttackingSquare.Name == "g7" && !a.MayOnlyMoveHereIfOccupiedByEnemy);
			Assert.IsFalse(
				forwardMoves.Any(),
				"There is a piece blocking forward movement of this pawn. There should be no forward moves available."
			);
		}

		[TestMethod]
		public void PawnPromoteToQueen()
		{
			var fen = "3r3r/1P1n1p2/2BQpk2/3p4/3PP1pp/p1P2N2/P5PP/R1BK3R w - - 0 32";
			var gameState = TestUtility.GetGameState(_gameStateService, fen);
			var gameStateResult = _gameStateService.MakeMove(gameState, 49, 57, PieceType.Queen);
			Assert.IsTrue(gameStateResult.Success);
			Assert.AreEqual("1Q1r3r/3n1p2/2BQpk2/3p4/3PP1pp/p1P2N2/P5PP/R1BK3R b - - 0 32", gameStateResult.Result.ToString(), "The board states should be equal.");
		}
	}
}