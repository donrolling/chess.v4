﻿using chess.v4.engine.interfaces;
using chess.v4.models.enumeration;
using Common.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Tests.Models;
using Tests.Utility;

namespace Tests {
	[TestClass]
	public class RookAttackTests : TestBase {
		public IAttackService AttackService { get; }

		public IGameStateService GameStateService { get; }
		public IMoveService MoveService { get; }

		public RookAttackTests() {
			this.AttackService = this.ServiceProvider.GetService<IAttackService>();
			this.GameStateService = this.ServiceProvider.GetService<IGameStateService>();
			this.MoveService = this.ServiceProvider.GetService<IMoveService>();
		}

		[TestMethod]
		public void WhiteRookAttacksEmptyBoardFromD4() {
			var fen = "7k/8/8/8/3R4/8/8/7K b - - 0 32";
			var gameState = TestUtility.GetGameState(this.GameStateService, fen);
			var whiteRookAttacks = gameState.Attacks.Where(a => a.AttackingSquare.Index == 27).ToList();
			var allSquareIndexs = new int[] { 24, 25, 26, 28, 29, 30, 31, 59, 51, 43, 35, 19, 11, 3 };
			foreach (var x in allSquareIndexs) {
				TestUtility.ListContainsSquare(whiteRookAttacks, PieceType.Rook, x);
			}
		}
	}
}