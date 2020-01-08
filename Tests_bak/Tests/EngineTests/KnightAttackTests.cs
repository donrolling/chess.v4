﻿using Chess.v4.Engine.Interfaces;
using Chess.v4.Models.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using Tests.Models;
using Tests.Utility;

namespace Tests
{
    [TestClass]
    public class KnightAttackTests : TestBase
    {
        public IAttackService AttackService { get; }
        public IGameStateService GameStateService { get; }
        public IMoveService MoveService { get; }

        public KnightAttackTests()
        {
            this.AttackService = this.ServiceProvider.GetService<IAttackService>();
            this.GameStateService = this.ServiceProvider.GetService<IGameStateService>();
            this.MoveService = this.ServiceProvider.GetService<IMoveService>();
        }

        [TestMethod]
        public void BlackKnightProtectsQueen_WhoChecksKing()
        {
            var fen = "5k2/5n2/7q/2b2p1K/7P/7Q/8/8 w - - 2 102";
            var gameState = TestUtility.GetGameState(this.GameStateService, fen);
            var attacks = gameState.Attacks.Where(a => a.AttackingSquare.Name == "f7").ToList();
            var squares = new List<int> { 59, 63, 47, 38, 36, 43 };
            TestUtility.ListContainsSquares(attacks, squares, PieceType.Knight);
            Assert.AreEqual(squares.Count(), attacks.Count(), "Wrong number of attacks.");
        }

        [TestMethod]
        public void WhiteKnightAttackStartingPositionFromB1()
        {
            var fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
            var gameState = TestUtility.GetGameState(this.GameStateService, fen);
            var attacks = gameState.Attacks.Where(a => a.AttackingSquare.Name == "b1").ToList();
            var squares = new List<int> { 18, 16 };
            TestUtility.ListContainsSquares(attacks, squares, PieceType.Knight);
            Assert.AreEqual(squares.Count(), attacks.Count(), "Wrong number of attacks.");
        }

        [TestMethod]
        public void WhiteKnightAttackStartingPositionFromB4()
        {
            var fen = "rnbqkbnr/pppppppp/8/8/1N6/8/PPPPPPPP/R1BQKBNR b KQkq - 0 1";
            var gameState = TestUtility.GetGameState(this.GameStateService, fen);
            var attacks = gameState.Attacks.Where(a => a.AttackingSquare.Name == "b4").ToList();
            var squares = new List<int> { 19, 35, 40, 42 };
            TestUtility.ListContainsSquares(attacks, squares, PieceType.Knight);
            Assert.AreEqual(squares.Count(), attacks.Count(), "Wrong number of attacks.");
        }

        [TestMethod]
        public void WhiteKnightAttackStartingPositionFromD5()
        {
            var fen = "rnbqkbnr/pppppppp/8/3N4/8/8/PPPPPPPP/R1BQKBNR b KQkq - 0 1";
            var gameState = TestUtility.GetGameState(this.GameStateService, fen);
            var attacks = gameState.Attacks.Where(a => a.AttackingSquare.Name == "d5").ToList();
            var squares = new List<int> { 18, 20, 25, 29, 41, 45, 50, 52 };
            TestUtility.ListContainsSquares(attacks, squares, PieceType.Knight);
            Assert.AreEqual(squares.Count(), attacks.Count(), "Wrong number of attacks.");
        }
    }
}