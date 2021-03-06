﻿using Chess.v4.Engine.Interfaces;
using Chess.v4.Models.Enums;
using EngineTests.Models;
using EngineTests.Utility;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace EngineTests.Tests.EngineTests
{
    [TestClass]
    public class CheckmateTests : TestBase
    {
        private readonly IGameStateService _gameStateService;

        public CheckmateTests()
        {
            _gameStateService = ServiceProvider.GetService<IGameStateService>();
        }

        [TestMethod]
        public void Given_CheckmateFEN_GameStateShouldReflectCheckmate()
        {
            var fen = "7Q/4b3/r5pk/8/5PK1/8/8/8 b - - 1 69";
            var gameState = TestUtility.GetGameState(_gameStateService, fen);
            var blackKingAttacks = gameState.Attacks
                                        .Where(a =>
                                            a.AttackingSquare.Piece.PieceType == PieceType.King
                                            && a.AttackingSquare.Piece.Color == Color.Black
                                            && !a.IsProtecting
                                        );
            Assert.IsFalse(blackKingAttacks.Any());
            Assert.IsTrue(gameState.StateInfo.IsCheckmate);
        }
    }
}