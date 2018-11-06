using chess.v4.engine.interfaces;
using Common.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Tests.Models;

namespace Tests {

	[TestClass]
	public class CoordinateTests : TestBase{
		public IAttackService AttackService { get; }
		
		public IGameStateService GameStateService { get; }
		public IMoveService MoveService { get; }
		public IOrthogonalService OrthogonalService { get; }

		public CoordinateTests() {
			this.OrthogonalService = this.ServiceProvider.GetService<IOrthogonalService>();
			this.AttackService = this.ServiceProvider.GetService<IAttackService>();
			this.GameStateService = this.ServiceProvider.GetService<IGameStateService>();
			this.MoveService = this.ServiceProvider.GetService<IMoveService>();
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