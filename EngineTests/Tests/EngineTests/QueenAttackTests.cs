using Chess.v4.Engine.Interfaces;
using Chess.v4.Models.Enums;
using EngineTests.Models;
using EngineTests.Utility;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace EngineTests.Tests.EngineTests
{
    [TestClass]
    public class QueenAttackTests : TestBase
    {
        private readonly IGameStateService _gameStateService;

        public QueenAttackTests()
        {
            _gameStateService = ServiceProvider.GetService<IGameStateService>();
        }

        [TestMethod]
        public void WhiteQueenAttacksEmptyBoardFromD4()
        {
            //only kings and a queen on the board
            var fen = "7k/8/8/8/3Q4/8/8/7K b - - 0 32";
            var gameState = TestUtility.GetGameState(_gameStateService, fen);
            var attacks = gameState.Attacks.Where(a => a.AttackingSquare.Name == "d4").ToList();
            var allSquareIndexs = new int[] { 0, 3, 6, 9, 11, 13, 18, 19, 20, 24, 25, 26, 28, 29, 30, 31, 34, 35, 36, 41, 43, 45, 48, 51, 54, 59, 63 };
            TestUtility.ListContainsSquares(attacks, allSquareIndexs.ToList(), PieceType.Queen);
        }

        [TestMethod]
        public void WhiteQueenAttacksStartingPosition()
        {
            var gameState = TestUtility.GetGameState(_gameStateService);
            var whiteQueenAttacks = gameState.Attacks.Where(a =>
                a.AttackingSquare.Name == "d1"
                && !a.IsProtecting
            );
            Assert.AreEqual(0, whiteQueenAttacks.Count());
        }
    }
}