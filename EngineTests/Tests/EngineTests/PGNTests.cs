﻿using Chess.v4.Engine.Interfaces;
using Chess.v4.Engine.Utility;
using Chess.v4.Models;
using Chess.v4.Models.Enums;
using Common.IO;
using EngineTests.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace EngineTests.Tests.EngineTests
{
    [TestClass]
    public class PGNTests : TestBase
    {
        private readonly IGameStateService _gameStateService;
        private readonly IPGNService _pgnService;

        public PGNTests()
        {
            _gameStateService = ServiceProvider.GetService<IGameStateService>();
            _pgnService = ServiceProvider.GetService<IPGNService>();
        }

        [TestMethod]
        public void ParsePGN_Data()
        {
            var pgnData = FileUtility.ReadTextFile<PGNTests>("ParsePGN_Data.pgn", "Output");
            var twoParts = pgnData.Split("\r\n\r\n");
            Assert.AreEqual(2, twoParts.Length);
            var justPGN = twoParts[1];
            var moves = new Regex(@"\d*\.\s?").Split(justPGN).ToList().Where(a => !string.IsNullOrEmpty(a));
            Assert.AreEqual(22, moves.Count());
            var lastMove = moves.Last();
            Assert.AreEqual("Qxe5 1/2-1/2", lastMove);
        }

        [TestMethod]
        public void AllAttacks()
        {
            var gameStateResult = _gameStateService.Initialize();
            Assert.IsTrue(gameStateResult.Success);
            var gameState = gameStateResult.Result;

            var positions = _pgnService.PGNMoveToSquarePair(gameState, "e4");
            Assert.AreEqual(12, positions.Item1);
            Assert.AreEqual(28, positions.Item2);

            var positions2 = _pgnService.PGNMoveToSquarePair(gameState, "e3");
            Assert.AreEqual(12, positions2.Item1);
            Assert.AreEqual(20, positions2.Item2);
        }

        [TestMethod]
        public void Basic()
        {
            Assert.AreEqual(PieceType.King, PGNEngine.GetPieceTypeFromPGNMove("Kb2"));
            Assert.AreEqual(PieceType.Queen, PGNEngine.GetPieceTypeFromPGNMove("Qa2"));
            Assert.AreEqual(PieceType.Rook, PGNEngine.GetPieceTypeFromPGNMove("Ra2"));
            Assert.AreEqual(PieceType.Bishop, PGNEngine.GetPieceTypeFromPGNMove("Bb2"));
            Assert.AreEqual(PieceType.Knight, PGNEngine.GetPieceTypeFromPGNMove("Na2"));
            Assert.AreEqual(PieceType.Pawn, PGNEngine.GetPieceTypeFromPGNMove("a2"));

            Assert.AreEqual(PieceType.King, PGNEngine.GetPieceTypeFromPGNMove("kb2"));
            Assert.AreEqual(PieceType.Queen, PGNEngine.GetPieceTypeFromPGNMove("qa2"));
            Assert.AreEqual(PieceType.Rook, PGNEngine.GetPieceTypeFromPGNMove("ra2"));
            Assert.AreEqual(PieceType.Bishop, PGNEngine.GetPieceTypeFromPGNMove("bb2"));
            Assert.AreEqual(PieceType.Knight, PGNEngine.GetPieceTypeFromPGNMove("na2"));
            Assert.AreEqual(PieceType.Pawn, PGNEngine.GetPieceTypeFromPGNMove("a2"));
        }

        [TestMethod]
        public void PGNMoveDifferentiation()
        {
            // create a situation where the engine has to differentiate between two rooks of the same color on a single file
            var fen = "4r3/N2R3p/1k2BPp1/8/8/2P5/PP3PPP/3R2K1 w - - 0 30";
            var gameStateResult = _gameStateService.Initialize(fen);
            Assert.IsTrue(gameStateResult.Success);
            var gameState = gameStateResult.Result;
            // assert that two white rooks exist
            var rooks = gameState.Squares.Where(a =>
                                            a.Occupied && a.Piece.PieceType == PieceType.Rook
                                            && a.Piece.Color == Color.White);
            Assert.AreEqual(2, rooks.Count());
            // assert that both white rooks have attacks
            var rookAttacks = gameState.Attacks.Where(a =>
                                            a.AttackingSquare.Piece.PieceType == PieceType.Rook
                                            && a.AttackingSquare.Piece.Color == Color.White);
            var attackMessage = new StringBuilder();
            foreach (var rookAttack in rookAttacks)
            {
                attackMessage.AppendLine($"\r\n==========================\r\n\tAttackerSquare: { rookAttack.AttackingSquare.Index }\r\n\tAttacked Square: { rookAttack.Index }");
            }
            Logger.LogInformation(attackMessage.ToString());
            Assert.AreEqual(22, rookAttacks.Where(a => !a.IsProtecting).Count());
            // this method is kinda wonky, but it is working for this one place that it is referenced.
            var pos = _pgnService.GetCurrentPositionFromPGNMove(gameState, new Piece(PieceType.Rook, Color.White), 43, "R1d6", false);
            Assert.AreEqual(3, pos.Index);
            // should make a test that asserts that this is check
        }

        // https://github.com/donrolling/chess.v4/issues/8
        [TestMethod]
        public void PGNNotationBug1()
        {
            var fen = "rnbqkbnr/ppp2ppp/8/3pp3/3PP3/8/PPP2PPP/RNBQKBNR w KQkq e6 0 3";
            var gameStateResult = _gameStateService.Initialize(fen);
            Assert.IsTrue(gameStateResult.Success);
            var newGameStateResult = _gameStateService.MakeMove(gameStateResult.Result, "f2", "f4");
            Assert.IsTrue(newGameStateResult.Success);
            var newGameState = newGameStateResult.Result;
            Assert.AreEqual(newGameState.PGN, "3. f4");
        }

        // https://github.com/donrolling/chess.v4/issues/8
        [TestMethod]
        public void PGNNotationBug2()
        {
            var fen = "4r3/N2R3p/1k2BPp1/8/8/2P5/PP3PPP/3R2K1 w - - 0 30";
            var gameStateResult = _gameStateService.Initialize(fen);
            Assert.IsTrue(gameStateResult.Success);
            var newGameStateResult = _gameStateService.MakeMove(gameStateResult.Result, "d1", "d6");
            Assert.IsTrue(newGameStateResult.Success);
            var newGameState = newGameStateResult.Result;
            Assert.AreEqual(newGameState.PGN, "30. R1d6");
        }
    }
}