using Chess.v4.Engine.Interfaces;
using EngineTests.Models;
using EngineTests.Utility;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EngineTests.Tests.EngineTests
{
	[TestClass]
	public class CheckTests : TestBase
	{
		private IGameStateService _gameStateService;

		public CheckTests()
		{
			_gameStateService = ServiceProvider.GetService<IGameStateService>();
		}

		[TestMethod]
		public void Verify_CheckIsDetected()
		{
			var fen = "7k/1p3Q2/5N2/p5p1/P6p/1Pb4P/2n3P1/6K1 w - - 13 49";
			var gameState = TestUtility.GetGameState(_gameStateService, fen);
			var gsr = _gameStateService.MakeMove(gameState, "Qh7");
			Assert.IsTrue(gsr.Success, "Shoud have been a valid move.");
			Assert.IsTrue(gsr.Result.StateInfo.IsCheckmate, "Should be white checkmate.");
		}
	}
}