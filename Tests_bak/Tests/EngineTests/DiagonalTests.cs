using Chess.v4.Engine.Interfaces;
using Chess.v4.Engine.Reference;
using Chess.v4.Engine.Utility;
using Chess.v4.Models;
using Chess.v4.Models.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Tests.Models;
using Tests.Utility;

namespace Tests
{
    [TestClass]
    public class DiagonalTests : TestBase
    {
        public GameState GameState { get; }
        public IGameStateService GameStateService { get; }

        public DiagonalTests()
        {
            this.GameStateService = this.ServiceProvider.GetService<IGameStateService>();
            this.GameState = TestUtility.GetGameState(this.GameStateService, GeneralReference.Starting_FEN_Position);
        }

        [TestMethod]
        public void File0_GoingLeft_DiagonalLineContains_TheCorrectSquares()
        {
            var squares = new List<int> { 0 };
            this.testDiagonalLine(0, DiagonalDirectionFromFileNumber.Left, squares);
        }

        [TestMethod]
        public void File0_GoingRight_DiagonalLineContains_TheCorrectSquares()
        {
            var squares = new List<int> { 0, 9, 18, 27, 36, 45, 54, 63 };
            this.testDiagonalLine(0, DiagonalDirectionFromFileNumber.Right, squares);
        }

        [TestMethod]
        public void File1_GoingLeft_DiagonalLineContains_TheCorrectSquares()
        {
            var squares = new List<int> { 1, 8 };
            this.testDiagonalLine(1, DiagonalDirectionFromFileNumber.Left, squares);
        }

        [TestMethod]
        public void File1_GoingRight_DiagonalLineContains_TheCorrectSquares()
        {
            var squares = new List<int> { 1, 10, 19, 28, 37, 46, 55 };
            this.testDiagonalLine(1, DiagonalDirectionFromFileNumber.Right, squares);
        }

        [TestMethod]
        public void File2_GoingLeft_DiagonalLineContains_TheCorrectSquares()
        {
            var squares = new List<int> { 2, 9, 16 };
            this.testDiagonalLine(2, DiagonalDirectionFromFileNumber.Left, squares);
        }

        [TestMethod]
        public void File2_GoingRight_DiagonalLineContains_TheCorrectSquares()
        {
            var squares = new List<int> { 2, 11, 20, 29, 38, 47 };
            this.testDiagonalLine(2, DiagonalDirectionFromFileNumber.Right, squares);
        }

        [TestMethod]
        public void File3_GoingLeft_DiagonalLineContains_TheCorrectSquares()
        {
            var squares = new List<int> { 3, 10, 17, 24 };
            this.testDiagonalLine(3, DiagonalDirectionFromFileNumber.Left, squares);
        }

        [TestMethod]
        public void File3_GoingRight_DiagonalLineContains_TheCorrectSquares()
        {
            var squares = new List<int> { 3, 12, 21, 30, 39 };
            this.testDiagonalLine(3, DiagonalDirectionFromFileNumber.Right, squares);
        }

        [TestMethod]
        public void File4_GoingLeft_DiagonalLineContains_TheCorrectSquares()
        {
            var squares = new List<int> { 4, 11, 18, 25, 32 };
            this.testDiagonalLine(4, DiagonalDirectionFromFileNumber.Left, squares);
        }

        [TestMethod]
        public void File4_GoingRight_DiagonalLineContains_TheCorrectSquares()
        {
            var squares = new List<int> { 4, 13, 22, 31 };
            this.testDiagonalLine(4, DiagonalDirectionFromFileNumber.Right, squares);
        }

        [TestMethod]
        public void File5_GoingLeft_DiagonalLineContains_TheCorrectSquares()
        {
            var squares = new List<int> { 5, 12, 19, 26, 33, 40 };
            this.testDiagonalLine(5, DiagonalDirectionFromFileNumber.Left, squares);
        }

        [TestMethod]
        public void File5_GoingRight_DiagonalLineContains_TheCorrectSquares()
        {
            var squares = new List<int> { 5, 14, 23 };
            this.testDiagonalLine(5, DiagonalDirectionFromFileNumber.Right, squares);
        }

        [TestMethod]
        public void File6_GoingLeft_DiagonalLineContains_TheCorrectSquares()
        {
            var squares = new List<int> { 6, 13, 20, 27, 34, 41, 48 };
            this.testDiagonalLine(6, DiagonalDirectionFromFileNumber.Left, squares);
        }

        [TestMethod]
        public void File6_GoingRight_DiagonalLineContains_TheCorrectSquares()
        {
            var squares = new List<int> { 6, 15 };
            this.testDiagonalLine(6, DiagonalDirectionFromFileNumber.Right, squares);
        }

        [TestMethod]
        public void File7_GoingLeft_DiagonalLineContains_TheCorrectSquares()
        {
            var squares = new List<int> { 7, 14, 21, 28, 35, 42, 49, 56 };
            this.testDiagonalLine(7, DiagonalDirectionFromFileNumber.Left, squares);
        }

        [TestMethod]
        public void File7_GoingRight_DiagonalLineContains_TheCorrectSquares()
        {
            var squares = new List<int> { 7 };
            this.testDiagonalLine(7, DiagonalDirectionFromFileNumber.Right, squares);
        }

        private void testDiagonalLine(int file, DiagonalDirectionFromFileNumber direction, List<int> squares)
        {
            var diagonalLine = DiagonalUtility.GetEntireDiagonalByFile(GameState, file, direction);
            TestUtility.ListContainsSquares(diagonalLine, squares);
        }
    }
}