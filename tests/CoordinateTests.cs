using chess.v4.engine.interfaces;
using common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using tests.setup;

namespace tests {

	[TestClass]
	public class CoordinateTests {
		public IAttackService AttackService { get; }
		
		public IGameStateService GameStateService { get; }
		public IMoveService MoveService { get; }
		public IOrthogonalService OrthogonalService { get; }

		public CoordinateTests() {
			var serviceProvider = new TestSetup().Setup();
			
			this.OrthogonalService = serviceProvider.GetService<IOrthogonalService>();
			this.AttackService = serviceProvider.GetService<IAttackService>();
			this.GameStateService = serviceProvider.GetService<IGameStateService>();
			this.MoveService = serviceProvider.GetService<IMoveService>();
		}

		[TestMethod]
		public void RankFileTests() {
			var a1RookAttackRank = OrthogonalService.GetEntireRank(0);
			Assert.IsTrue(a1RookAttackRank.Contains(0));
			Assert.IsTrue(a1RookAttackRank.Contains(1));
			Assert.IsTrue(a1RookAttackRank.Contains(2));
			Assert.IsTrue(a1RookAttackRank.Contains(3));
			Assert.IsTrue(a1RookAttackRank.Contains(4));
			Assert.IsTrue(a1RookAttackRank.Contains(5));
			Assert.IsTrue(a1RookAttackRank.Contains(6));
			Assert.IsTrue(a1RookAttackRank.Contains(7));
			var a1RookAttackFile = OrthogonalService.GetEntireFile(0);
			Assert.IsTrue(a1RookAttackFile.Contains(0));
			Assert.IsTrue(a1RookAttackFile.Contains(8));
			Assert.IsTrue(a1RookAttackFile.Contains(16));
			Assert.IsTrue(a1RookAttackFile.Contains(24));
			Assert.IsTrue(a1RookAttackFile.Contains(32));
			Assert.IsTrue(a1RookAttackFile.Contains(40));
			Assert.IsTrue(a1RookAttackFile.Contains(48));
			Assert.IsTrue(a1RookAttackFile.Contains(56));
		}
	}
}