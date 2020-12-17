using Chess.v4.Engine.Interfaces;
using Chess.v4.Engine.Utility;
using EngineTests.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace EngineTests.Tests.EngineTests
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
            var list = DiagonalUtility.GetEntireDiagonalByPosition(35);
            var accepted = new List<int> { 8, 17, 26, 35, 44, 53, 62, 56, 49, 42, 28, 21, 14, 7 };
            var remaining = accepted.Except(list);
            Assert.IsFalse(remaining.Any());
        }

        [TestMethod]
        public void IsDiagonal()
        {
            Assert.IsTrue(DiagonalUtility.IsDiagonal(9, 27));
            Assert.IsTrue(DiagonalUtility.IsDiagonal(27, 9));
            Assert.IsTrue(DiagonalUtility.IsDiagonal(49, 35));
            Assert.IsTrue(DiagonalUtility.IsDiagonal(35, 49));
            Assert.IsFalse(DiagonalUtility.IsDiagonal(0, 7));
            Assert.IsFalse(DiagonalUtility.IsDiagonal(7, 0));
        }

        [TestMethod]
        public void IsOrthogonal()
        {
            Assert.IsTrue(GeneralUtility.IsOrthogonal(0, 7));
            Assert.IsTrue(GeneralUtility.IsOrthogonal(7, 0));
            Assert.IsTrue(GeneralUtility.IsOrthogonal(56, 0));
            Assert.IsTrue(GeneralUtility.IsOrthogonal(0, 56));
            Assert.IsFalse(GeneralUtility.IsOrthogonal(0, 9));
        }
    }
}