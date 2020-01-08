using Chess.v4.Engine.Interfaces;
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
    public class BishopAttackTests : TestBase
    {
        public IAttackService AttackService { get; }

        public IGameStateService GameStateService { get; }
        public IMoveService MoveService { get; }

        public BishopAttackTests()
        {
            this.AttackService = this.ServiceProvider.GetService<IAttackService>();
            this.GameStateService = this.ServiceProvider.GetService<IGameStateService>();
            this.MoveService = this.ServiceProvider.GetService<IMoveService>();
        }

        [TestMethod]
        public void AllBishops_Given_SweetOpening_ValidateBasicOpeningAttacks()
        {
            var fen = "rnbqkbnr/p1p2p1p/1p4p1/3pp3/3PP3/1P4P1/P1P2P1P/RNBQKBNR w KQkq - 0 1";
            var gameState = TestUtility.GetGameState(this.GameStateService, fen);
            var attacks = gameState.Attacks.Where(a => a.AttackingSquare.Name == "c1").ToList();
            var squares = new List<int> { 9, 16, 11, 20, 29, 38, 47 };
            TestUtility.ListContainsSquares(attacks, squares, PieceType.Bishop);
            Assert.AreEqual(squares.Count(), attacks.Count());

            attacks = gameState.Attacks.Where(a => a.AttackingSquare.Name == "f1").ToList();
            squares = new List<int> { 14, 23, 12, 19, 26, 33, 40 };
            Assert.AreEqual(squares.Count(), attacks.Count());

            attacks = gameState.Attacks.Where(a => a.AttackingSquare.Name == "c8").ToList();
            squares = new List<int> { 49, 40, 51, 44, 37, 30, 23 };
            Assert.AreEqual(squares.Count(), attacks.Count());

            attacks = gameState.Attacks.Where(a => a.AttackingSquare.Name == "f8").ToList();
            squares = new List<int> { 54, 47, 52, 43, 34, 25, 16 };
            Assert.AreEqual(squares.Count(), attacks.Count());
        }

        [TestMethod]
        public void WhiteBishopAttacksStartingPositionFromD4()
        {
            var fen = "rnbqkbnr/pppppppp/8/8/3B4/8/PPPPPPPP/RN1QKBNR b KQkq - 0 1";
            var gameState = TestUtility.GetGameState(this.GameStateService, fen);
            var attacks = gameState.Attacks.Where(a => a.AttackingSquare.Name == "d4" && !a.IsProtecting).ToList();
            var squares = new List<int> { 20, 34, 41, 48, 18, 36, 45, 54 };
            TestUtility.ListContainsSquares(attacks, squares, PieceType.Bishop);
            Assert.AreEqual(8, attacks.Count());
        }
    }
}