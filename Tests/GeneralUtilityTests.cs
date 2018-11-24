using chess.v4.engine.interfaces;
using chess.v4.engine.reference;
using chess.v4.engine.Utility;
using Common.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Text.RegularExpressions;
using Tests.Models;

namespace Tests {
	[TestClass]
	public class GeneralUtilityTests : TestBase {
		public IGameStateService GameStateService { get; }
		public IPGNService PGNService { get; }

		public GeneralUtilityTests() {
			this.GameStateService = this.ServiceProvider.GetService<IGameStateService>();
			this.PGNService = this.ServiceProvider.GetService<IPGNService>();
		}

		[TestMethod]
		public void IsOrthogonal() {
			Assert.IsTrue(GeneralUtility.IsOrthogonal(0, 7));
			Assert.IsTrue(GeneralUtility.IsOrthogonal(7, 0));
			Assert.IsTrue(GeneralUtility.IsOrthogonal(56, 0));
			Assert.IsTrue(GeneralUtility.IsOrthogonal(0, 56));
			Assert.IsFalse(GeneralUtility.IsOrthogonal(0, 9));
		}

		[TestMethod]
		public void IsDiagonal() {
			Assert.IsTrue(GeneralUtility.IsDiagonal(9, 27));
			Assert.IsTrue(GeneralUtility.IsDiagonal(49, 35));
			Assert.IsFalse(GeneralUtility.IsDiagonal(0, 7));
			Assert.IsFalse(GeneralUtility.IsDiagonal(7, 0));
		}
	}
}