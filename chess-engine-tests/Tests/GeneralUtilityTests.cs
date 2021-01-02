using chess_engine.Engine.Interfaces;
using chess_engine.Engine.Utility;
using chess_engine_tests.Application;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace chess_engine_tests.Tests
{
    [TestClass]
    public class GeneralUtilityTests : TestBase
    {
        public GeneralUtilityTests()
        {
        }

        [TestMethod]
        public void GetEntireDiagonalByPosition()
        {
            var list = DiagonalEngine.GetEntireDiagonalByPosition(35);
            var accepted = new List<int> { 8, 17, 26, 35, 44, 53, 62, 56, 49, 42, 28, 21, 14, 7 };
            var remaining = accepted.Except(list);
            Assert.IsFalse(remaining.Any());
        }

        [TestMethod]
        public void IsDiagonal()
        {
            Assert.IsTrue(DiagonalEngine.IsDiagonal(9, 27));
            Assert.IsTrue(DiagonalEngine.IsDiagonal(27, 9));
            Assert.IsTrue(DiagonalEngine.IsDiagonal(49, 35));
            Assert.IsTrue(DiagonalEngine.IsDiagonal(35, 49));
            Assert.IsFalse(DiagonalEngine.IsDiagonal(0, 7));
            Assert.IsFalse(DiagonalEngine.IsDiagonal(7, 0));
        }

        [TestMethod]
        public void IsOrthogonal()
        {
            Assert.IsTrue(GeneralEngine.IsOrthogonal(0, 7));
            Assert.IsTrue(GeneralEngine.IsOrthogonal(7, 0));
            Assert.IsTrue(GeneralEngine.IsOrthogonal(56, 0));
            Assert.IsTrue(GeneralEngine.IsOrthogonal(0, 56));
            Assert.IsFalse(GeneralEngine.IsOrthogonal(0, 9));
        }
    }
}