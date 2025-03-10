using chess_engine.Engine.Interfaces;
using chess_engine_tests.Application;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace chess_engine_tests.Tests
{
	[TestClass]
	public class CoordinateTests : TestBase
	{
		private readonly IAttackService _attackService;
		private readonly IGameStateService _gameStateService;
		private readonly IMoveService _moveService;
		private readonly IOrthogonalService _orthogonalService;

		public CoordinateTests()
		{
			_orthogonalService = ServiceProvider.GetService<IOrthogonalService>();
			_attackService = ServiceProvider.GetService<IAttackService>();
			_gameStateService = ServiceProvider.GetService<IGameStateService>();
			_moveService = ServiceProvider.GetService<IMoveService>();
		}

		[TestMethod]
		public void RankFileTests()
		{
			var a1RookAttackRank = _orthogonalService.GetEntireRank(0);
			Assert.IsTrue(a1RookAttackRank.Contains(0));
			Assert.IsTrue(a1RookAttackRank.Contains(1));
			Assert.IsTrue(a1RookAttackRank.Contains(2));
			Assert.IsTrue(a1RookAttackRank.Contains(3));
			Assert.IsTrue(a1RookAttackRank.Contains(4));
			Assert.IsTrue(a1RookAttackRank.Contains(5));
			Assert.IsTrue(a1RookAttackRank.Contains(6));
			Assert.IsTrue(a1RookAttackRank.Contains(7));
			var a1RookAttackFile = _orthogonalService.GetEntireFile(0);
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