using Chess.v4.Engine.Interfaces;
using Chess.v4.Engine.Reference;
using Chess.v4.Engine.Utility;
using Chess.v4.Models;
using Chess.v4.Models.Enums;
using EngineTests.Models;
using EngineTests.Utility;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace EngineTests.Tests.EngineTests
{
    [TestClass]
    public class DiagonalTests : TestBase
    {
        private readonly GameState _gameState;
        private readonly IGameStateService _gameStateService;

        public DiagonalTests()
        {
            _gameStateService = ServiceProvider.GetService<IGameStateService>();
            _gameState = TestUtility.GetGameState(_gameStateService, GeneralReference.Starting_FEN_Position);
        }

        [TestMethod]
        public void File0_GoingLeft_DiagonalLineContains_TheCorrectSquares()
        {
            var squares = new List<int> { 0 };
            testDiagonalLine(0, DiagonalDirectionFromFileNumber.Left, squares);
        }

        [TestMethod]
        public void File0_GoingRight_DiagonalLineContains_TheCorrectSquares()
        {
            var squares = new List<int> { 0, 9, 18, 27, 36, 45, 54, 63 };
            testDiagonalLine(0, DiagonalDirectionFromFileNumber.Right, squares);
        }

        [TestMethod]
        public void File1_GoingLeft_DiagonalLineContains_TheCorrectSquares()
        {
            var squares = new List<int> { 1, 8 };
            testDiagonalLine(1, DiagonalDirectionFromFileNumber.Left, squares);
        }

        [TestMethod]
        public void File1_GoingRight_DiagonalLineContains_TheCorrectSquares()
        {
            var squares = new List<int> { 1, 10, 19, 28, 37, 46, 55 };
            testDiagonalLine(1, DiagonalDirectionFromFileNumber.Right, squares);
        }

        [TestMethod]
        public void File2_GoingLeft_DiagonalLineContains_TheCorrectSquares()
        {
            var squares = new List<int> { 2, 9, 16 };
            testDiagonalLine(2, DiagonalDirectionFromFileNumber.Left, squares);
        }

        [TestMethod]
        public void File2_GoingRight_DiagonalLineContains_TheCorrectSquares()
        {
            var squares = new List<int> { 2, 11, 20, 29, 38, 47 };
            testDiagonalLine(2, DiagonalDirectionFromFileNumber.Right, squares);
        }

        [TestMethod]
        public void File3_GoingLeft_DiagonalLineContains_TheCorrectSquares()
        {
            var squares = new List<int> { 3, 10, 17, 24 };
            testDiagonalLine(3, DiagonalDirectionFromFileNumber.Left, squares);
        }

        [TestMethod]
        public void File3_GoingRight_DiagonalLineContains_TheCorrectSquares()
        {
            var squares = new List<int> { 3, 12, 21, 30, 39 };
            testDiagonalLine(3, DiagonalDirectionFromFileNumber.Right, squares);
        }

        [TestMethod]
        public void File4_GoingLeft_DiagonalLineContains_TheCorrectSquares()
        {
            var squares = new List<int> { 4, 11, 18, 25, 32 };
            testDiagonalLine(4, DiagonalDirectionFromFileNumber.Left, squares);
        }

        [TestMethod]
        public void File4_GoingRight_DiagonalLineContains_TheCorrectSquares()
        {
            var squares = new List<int> { 4, 13, 22, 31 };
            testDiagonalLine(4, DiagonalDirectionFromFileNumber.Right, squares);
        }

        [TestMethod]
        public void File5_GoingLeft_DiagonalLineContains_TheCorrectSquares()
        {
            var squares = new List<int> { 5, 12, 19, 26, 33, 40 };
            testDiagonalLine(5, DiagonalDirectionFromFileNumber.Left, squares);
        }

        [TestMethod]
        public void File5_GoingRight_DiagonalLineContains_TheCorrectSquares()
        {
            var squares = new List<int> { 5, 14, 23 };
            testDiagonalLine(5, DiagonalDirectionFromFileNumber.Right, squares);
        }

        [TestMethod]
        public void File6_GoingLeft_DiagonalLineContains_TheCorrectSquares()
        {
            var squares = new List<int> { 6, 13, 20, 27, 34, 41, 48 };
            testDiagonalLine(6, DiagonalDirectionFromFileNumber.Left, squares);
        }

        [TestMethod]
        public void File6_GoingRight_DiagonalLineContains_TheCorrectSquares()
        {
            var squares = new List<int> { 6, 15 };
            testDiagonalLine(6, DiagonalDirectionFromFileNumber.Right, squares);
        }

        [TestMethod]
        public void File7_GoingLeft_DiagonalLineContains_TheCorrectSquares()
        {
            var squares = new List<int> { 7, 14, 21, 28, 35, 42, 49, 56 };
            testDiagonalLine(7, DiagonalDirectionFromFileNumber.Left, squares);
        }

        [TestMethod]
        public void File7_GoingRight_DiagonalLineContains_TheCorrectSquares()
        {
            var squares = new List<int> { 7 };
            testDiagonalLine(7, DiagonalDirectionFromFileNumber.Right, squares);
        }

        private void testDiagonalLine(int file, DiagonalDirectionFromFileNumber direction, List<int> squares)
        {
            var diagonalLine = DiagonalEngine.GetEntireDiagonalByFile(_gameState, file, direction);
            TestUtility.ListContainsSquares(diagonalLine, squares);
        }
    }
}