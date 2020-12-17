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
    public class RookAttackTests : TestBase
    {
        private readonly IAttackService _attackService;
        private readonly IGameStateService _gameStateService;
        private readonly IMoveService _moveService;

        public RookAttackTests()
        {
            _attackService = ServiceProvider.GetService<IAttackService>();
            _gameStateService = ServiceProvider.GetService<IGameStateService>();
            _moveService = ServiceProvider.GetService<IMoveService>();
        }

        [TestMethod]
        public void WhiteRookAttacksEmptyBoardFromD4()
        {
            var fen = "7k/8/8/8/3R4/8/8/7K b - - 0 32";
            var gameState = TestUtility.GetGameState(_gameStateService, fen);
            var whiteRookAttacks = gameState.Attacks.Where(a => a.AttackingSquare.Index == 27).ToList();
            var allSquareIndexs = new int[] { 24, 25, 26, 28, 29, 30, 31, 59, 51, 43, 35, 19, 11, 3 };
            foreach (var x in allSquareIndexs)
            {
                TestUtility.ListContainsSquare(whiteRookAttacks, PieceType.Rook, x);
            }
        }
    }
}