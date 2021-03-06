﻿using Chess.v4.Engine.Extensions;
using Chess.v4.Engine.Interfaces;
using Chess.v4.Engine.Utility;
using EngineTests.Models;
using EngineTests.Utility;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace EngineTests.Tests.EngineTests
{
    [TestClass]
    public class KingAttackTests : TestBase
    {
        private readonly IGameStateService _gameStateService;

        public KingAttackTests()
        {
            _gameStateService = ServiceProvider.GetService<IGameStateService>();
        }

        [TestMethod]
        public void BlackKingAttemptsToCastleThroughCheck_Fails()
        {
            var fen = "r1bqk2r/ppp2ppp/8/3P4/1Q2P3/6P1/PPP2P1P/R1B1KB1R b KQkq - 0 9";
            var gameState = TestUtility.GetGameState(_gameStateService, fen);

            var isCastleThroughCheck = CastlingEngine.DetermineCastleThroughCheck(gameState, 60, 63);
            Assert.IsTrue(isCastleThroughCheck);

            var newGameStateResult = _gameStateService.MakeMove(gameState, 60, 62);
            Assert.IsFalse(newGameStateResult.Success);
        }

        [TestMethod]
        public void BlackKingCastles_Succeeds()
        {
            var fen = "r1bqk2r/ppp2ppp/8/3P4/2Q1P3/6P1/PPP2P1P/R1B1KB1R b KQkq - 0 9";
            var gameState = TestUtility.GetGameState(_gameStateService, fen);

            var isCastleThroughCheck = CastlingEngine.DetermineCastleThroughCheck(gameState, 60, 63);
            Assert.IsFalse(isCastleThroughCheck);

            var newGameStateResult = _gameStateService.MakeMove(gameState, 60, 62);
            Assert.IsTrue(newGameStateResult.Success, $"Castle should have succeeded. { newGameStateResult.Message }");

            var newFEN = newGameStateResult.Result.ToString();
            Assert.AreEqual("r1bq1rk1/ppp2ppp/8/3P4/2Q1P3/6P1/PPP2P1P/R1B1KB1R w KQ - 1 10", newFEN);
        }

        [TestMethod]
        public void BlackKingIsInCheckmate_HasNoValidAttacks()
        {
            var fen = "2r5/8/2n1Q3/5R1k/3Pp1p1/p1P3Pp/P6P/R1BK4 b - - 1 41";
            var gameState = TestUtility.GetGameState(_gameStateService, fen);
            var blackKingAttacks = gameState.Attacks.Where(a => a.AttackingSquare.Name == "g1").ToList();
            Assert.AreEqual(0, blackKingAttacks.Count());
            Assert.IsTrue(gameState.StateInfo.IsCheck);
            Assert.IsTrue(gameState.StateInfo.IsBlackCheck);
            Assert.IsFalse(gameState.StateInfo.IsWhiteCheck);
            Assert.IsTrue(gameState.StateInfo.IsCheckmate);
        }

        [TestMethod]
        public void KingAttacks3()
        {
            var positions = CastlingEngine.GetKingPositionsDuringCastle(60, 63);
            Assert.AreEqual(2, positions.Count());
            Assert.AreEqual(61, positions[0]);
            Assert.AreEqual(62, positions[1]);

            var positions2 = CastlingEngine.GetKingPositionsDuringCastle(60, 56);
            Assert.AreEqual(2, positions2.Count());
            Assert.AreEqual(59, positions2[0]);
            Assert.AreEqual(58, positions2[1]);

            var positions3 = CastlingEngine.GetKingPositionsDuringCastle(4, 7);
            Assert.AreEqual(2, positions3.Count());
            Assert.AreEqual(5, positions3[0]);
            Assert.AreEqual(6, positions3[1]);

            var positions4 = CastlingEngine.GetKingPositionsDuringCastle(4, 0);
            Assert.AreEqual(2, positions4.Count());
            Assert.AreEqual(3, positions4[0]);
            Assert.AreEqual(2, positions4[1]);
        }

        [TestMethod]
        public void KingMusntMoveIntoPawnAttack()
        {
            var fen = "rnbqkbnr/1pp2ppp/4p3/p2p4/8/4K3/PPPP1PPP/RNBQ1BNR w kq - 0 4";
            var gameState = TestUtility.GetGameState(_gameStateService, fen);
            var whiteKingAttacks = gameState.Attacks.Where(a => a.AttackingSquare.Name == "e3").ToList();
            Assert.IsFalse(whiteKingAttacks.Select(a => a.Name).Contains("e4"), "King should not be able to move into e4 because a pawn attacks there.");
        }

        [TestMethod]
        public void WhiteKingAttemptsToCastleThroughCheck_Fails()
        {
            var fen = "r3k2r/pp3ppp/2pq4/7b/1Q2PB2/1P6/P1P1BPPP/R3K2R w KQkq - 3 13";
            var gameState = TestUtility.GetGameState(_gameStateService, fen);

            var isCastleThroughCheck = CastlingEngine.DetermineCastleThroughCheck(gameState, 4, 0);
            Assert.IsTrue(isCastleThroughCheck);

            var newGameStateResult = _gameStateService.MakeMove(gameState, 4, 2);
            Assert.IsFalse(newGameStateResult.Success);
        }

        [TestMethod]
        public void WhiteKingAttemptsToMoveDiagonallyOutOfCheck_Fails()
        {
            //should have one valid move only
            var fen = "6k1/5pbp/5Qp1/8/r3P3/3PK3/r4PPP/2q5 w - - 0 1";
            var gameState = TestUtility.GetGameState(_gameStateService, fen);
            var whiteKingAttacks = gameState.Attacks.Where(a => a.AttackingSquare.Name == "e3").ToList();
            Assert.IsTrue(gameState.StateInfo.IsCheck);
            Assert.IsTrue(gameState.StateInfo.IsWhiteCheck);
            var gameStateResult = _gameStateService.MakeMove(gameState, "e3", "f4");
            Assert.IsTrue(gameStateResult.Failure, "Make Move should result in a failure.");
        }

        [TestMethod]
        public void WhiteKingCastles_Succeeds()
        {
            var fen = "r1bqk2r/ppp2ppp/8/3P4/2Q1P3/6P1/PPP2P1P/R3KB1R w KQkq - 0 9";
            var gameState = TestUtility.GetGameState(_gameStateService, fen);

            // This test is broken
            // I'm guessing that the code I wrote to fix the issue in WhiteKingIsInCheckmateAndHasNoValidMoves
            // is making pieces seem like they are attacking the king when they are not
            var isCastleThroughCheck = CastlingEngine.DetermineCastleThroughCheck(gameState, 4, 0);
            Assert.IsFalse(isCastleThroughCheck);

            var newGameStateResult = _gameStateService.MakeMove(gameState, 4, 2);
            Assert.IsTrue(newGameStateResult.Success);

            var newFEN = newGameStateResult.Result.ToString();
            Assert.AreEqual("r1bqk2r/ppp2ppp/8/3P4/2Q1P3/6P1/PPP2P1P/2KR1B1R b kq - 1 9", newFEN);
        }

        [TestMethod]
        public void WhiteKingHasTwoValidMoves()
        {
            var fen = "3q1rk1/5pbp/5Qp1/8/8/2B5/5PPP/6K1 w - - 0 1";
            var gameState = TestUtility.GetGameState(_gameStateService, fen);
            var whiteKingAttacks = gameState.Attacks.Where(a => a.AttackingSquare.Name == "g1").ToList();
            Assert.AreEqual(2, whiteKingAttacks.Where(a => !a.IsProtecting).Count());
        }

        [TestMethod]
        public void WhiteKingIsInCheckmateAndHasNoValidMovesTestsLackOfOrthogonalRetreat()
        {
            var fen = "5rk1/5pbp/5Qp1/8/8/8/5PPP/3q2K1 w - - 0 1";
            var gameState = TestUtility.GetGameState(_gameStateService, fen);
            var whiteKingAttacks = gameState.Attacks.Where(a => a.AttackingSquare.Name == "g1" && !a.IsProtecting).ToList();
            //king checkmate make sure that the king has no attacks
            Assert.AreEqual(0, whiteKingAttacks.Count());
            Assert.IsTrue(gameState.StateInfo.IsCheck);
            Assert.IsTrue(gameState.StateInfo.IsWhiteCheck);
            Assert.IsFalse(gameState.StateInfo.IsBlackCheck);
            Assert.IsTrue(gameState.StateInfo.IsCheckmate);
        }

        [TestMethod]
        public void WhiteKingIsInCheckmateAndHasNoValidMovesTestsLackOfDiagonalRetreat()
        {
            var fen = "8/7k/6pp/8/r5P1/5PKP/r7/4q3 w - - 1 41";
            var gameState = TestUtility.GetGameState(_gameStateService, fen);
            var whiteKingAttacks = gameState.Attacks.Where(a => a.AttackingSquare.Name == "g3" && !a.IsProtecting).ToList();
            //king checkmate make sure that the king has no attacks
            Assert.AreEqual(0, whiteKingAttacks.Count());
            Assert.IsTrue(gameState.StateInfo.IsCheck);
            Assert.IsTrue(gameState.StateInfo.IsWhiteCheck);
            Assert.IsFalse(gameState.StateInfo.IsBlackCheck);
            Assert.IsTrue(gameState.StateInfo.IsCheckmate);
        }

        [TestMethod]
        public void KingFaceoff()
        {

        }

        [TestMethod]
        public void WhiteKingMayCastle()
        {
            var fen = "r2qkbnr/p2ppppp/b1n5/1pp5/4P3/BPN5/P1PPQPPP/R3KBNR w KQkq - 3 5";
            var gameState = TestUtility.GetGameState(_gameStateService, fen);
            var whiteKingAttacks = gameState.Attacks.Where(a => a.AttackingSquare.Name == "e1").ToList();
            var allSquareIndexs = new int[] { 2, 3 };
            foreach (var x in allSquareIndexs)
            {
                Assert.IsNotNull(whiteKingAttacks.GetSquare(x), $"King should be able to attack square: { x }");
            }
            Assert.AreEqual(2, whiteKingAttacks.Where(a => !a.IsProtecting).Count());
        }
    }
}