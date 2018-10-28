using chess.v4.engine.enumeration;
using chess.v4.engine.extensions;
using chess.v4.engine.interfaces;
using chess.v4.engine.utility;
using common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using tests.setup;
using tests.utility;

namespace tests {

	[TestClass]
	public class KingAttackTests {
		public IAttackService AttackService { get; }
		
		public IGameStateService GameStateService { get; }
		public IMoveService MoveService { get; }

		public KingAttackTests() {
			var serviceProvider = new TestSetup().Setup();			
			this.AttackService = serviceProvider.GetService<IAttackService>();
			this.GameStateService = serviceProvider.GetService<IGameStateService>();
			this.MoveService = serviceProvider.GetService<IMoveService>();
		}

		[TestMethod]
		public void WhiteKingHasTwoValidMoves() {
			var fen = "3q1rk1/5pbp/5Qp1/8/8/2B5/5PPP/6K1 w - - 0 1";
			var gameState = TestUtility.GetGameState(this.GameStateService, fen);
			var whiteKingAttacks = gameState.Attacks.Where(a => a.AttackerSquare.Name == "g1").ToList();
			Assert.AreEqual(2, whiteKingAttacks.Count());
		}

		[TestMethod]
		public void WhiteKingIsInCheckmateAndHasNoValidMoves() {
			var fen = "5rk1/5pbp/5Qp1/8/8/8/5PPP/3q2K1 w - - 0 1";
			var gameState = TestUtility.GetGameState(this.GameStateService, fen);
			var whiteKingAttacks = gameState.Attacks.Where(a => a.AttackerSquare.Name == "g1").ToList();
			//king checkmate make sure that the king has no attacks
			//Assert.AreEqual(0, whiteKingAttacks.Count());
			Assert.IsTrue(gameState.StateInfo.IsCheck);
			Assert.IsTrue(gameState.StateInfo.IsWhiteCheck);
			Assert.IsFalse(gameState.StateInfo.IsBlackCheck);
			Assert.IsTrue(gameState.StateInfo.IsCheckmate);
		}

		[TestMethod]
		public void WhiteKingMayCastle() {
			var fen = "r2qkbnr/p2ppppp/b1n5/1pp5/4P3/BPN5/P1PPQPPP/R3KBNR w KQkq - 3 5";
			var gameState = TestUtility.GetGameState(this.GameStateService, fen);
			var whiteKingAttacks = gameState.Attacks.Where(a => a.AttackerSquare.Name == "e1").ToList();
			var allSquareIndexs = new int[] { 2, 3 };
			foreach (var x in allSquareIndexs) {
				Assert.IsNotNull(whiteKingAttacks.GetSquare(x), $"King should be able to attack square: { x }");
			}
			Assert.AreEqual(2, whiteKingAttacks.Count());
		}

		[TestMethod]
		public void KingMusntMoveIntoPawnAttack() {
			var fen = "rnbqkbnr/1pp2ppp/4p3/p2p4/8/4K3/PPPP1PPP/RNBQ1BNR w kq - 0 4";
			var gameState = TestUtility.GetGameState(this.GameStateService, fen);
			var whiteKingAttacks = gameState.Attacks.Where(a => a.AttackerSquare.Name == "e3").ToList();
			Assert.IsFalse(whiteKingAttacks.Select(a => a.Name).Contains("e4"));
		}

		[TestMethod]
		public void KingAttacks3() {
			var positions = CastleUtility.GetKingPositionsDuringCastle(60, 63);
			Assert.AreEqual(2, positions.Count());
			Assert.AreEqual(61, positions[0]);
			Assert.AreEqual(62, positions[1]);

			var positions2 = CastleUtility.GetKingPositionsDuringCastle(60, 56);
			Assert.AreEqual(2, positions2.Count());
			Assert.AreEqual(59, positions2[0]);
			Assert.AreEqual(58, positions2[1]);

			var positions3 = CastleUtility.GetKingPositionsDuringCastle(4, 7);
			Assert.AreEqual(2, positions3.Count());
			Assert.AreEqual(5, positions3[0]);
			Assert.AreEqual(6, positions3[1]);

			var positions4 = CastleUtility.GetKingPositionsDuringCastle(4, 0);
			Assert.AreEqual(2, positions4.Count());
			Assert.AreEqual(3, positions4[0]);
			Assert.AreEqual(2, positions4[1]);
		}

		[TestMethod]
		public void BlackKingAttemptsToCastleThroughCheck_Fails() {
			var fen = "r1bqk2r/ppp2ppp/8/3P4/1Q2P3/6P1/PPP2P1P/R1B1KB1R b KQkq - 0 9";
			var gameState = TestUtility.GetGameState(this.GameStateService, fen);

			var isCastleThroughCheck = CastleUtility.DetermineCastleThroughCheck(gameState, 60, 63);
			Assert.IsTrue(isCastleThroughCheck);

			var newGameStateResult = this.GameStateService.MakeMove(gameState, 60, 62);
			Assert.IsFalse(newGameStateResult.Success);
		}

		[TestMethod]
		public void BlackKingCastles_Succeeds() {
			var fen = "r1bqk2r/ppp2ppp/8/3P4/2Q1P3/6P1/PPP2P1P/R1B1KB1R b KQkq - 0 9";
			var gameState = TestUtility.GetGameState(this.GameStateService, fen);

			var isCastleThroughCheck = CastleUtility.DetermineCastleThroughCheck(gameState, 60, 63);
			Assert.IsFalse(isCastleThroughCheck);

			var newGameStateResult = this.GameStateService.MakeMove(gameState, 60, 62);
			Assert.IsTrue(newGameStateResult.Success);

			var newFEN = newGameStateResult.Result.ToString();
			Assert.AreEqual("r1bq1rk1/ppp2ppp/8/3P4/2Q1P3/6P1/PPP2P1P/R1B1KB1R w KQ - 1 10", newFEN);
		}

		[TestMethod]
		public void WhiteKingCastles_Succeeds() {
			var fen = "r1bqk2r/ppp2ppp/8/3P4/2Q1P3/6P1/PPP2P1P/R3KB1R w KQkq - 0 9";
			var gameState = TestUtility.GetGameState(this.GameStateService, fen);

			var isCastleThroughCheck = CastleUtility.DetermineCastleThroughCheck(gameState, 4, 0);
			Assert.IsFalse(isCastleThroughCheck);

			var newGameStateResult = this.GameStateService.MakeMove(gameState, 4, 2);
			Assert.IsTrue(newGameStateResult.Success);
				
			var newFEN = newGameStateResult.Result.ToString();
			Assert.AreEqual("r1bqk2r/ppp2ppp/8/3P4/2Q1P3/6P1/PPP2P1P/2KR1B1R b kq - 1 9", newFEN);
		}

		[TestMethod]
		public void WhiteKingAttemptsToCastleThroughCheck_Fails() {
			var fen = "r3k2r/pp3ppp/2pq4/7b/1Q2PB2/1P6/P1P1BPPP/R3K2R w KQkq - 3 13";
			var gameState = TestUtility.GetGameState(this.GameStateService, fen);

			var isCastleThroughCheck = CastleUtility.DetermineCastleThroughCheck(gameState, 4, 0);
			Assert.IsTrue(isCastleThroughCheck);

			var newGameStateResult = this.GameStateService.MakeMove(gameState, 4, 2);
			Assert.IsFalse(newGameStateResult.Success);
		}

		[TestMethod]
		public void BlackKingIsInCheckmate_HasNoValidAttacks() {
			var fen = "2r5/8/2n1Q3/5R1k/3Pp1p1/p1P3Pp/P6P/R1BK4 b - - 1 41";
			var gameState = TestUtility.GetGameState(this.GameStateService, fen);
			var blackKingAttacks = gameState.Attacks.Where(a => a.AttackerSquare.Name == "g1").ToList();
			Assert.AreEqual(0, blackKingAttacks.Count());
			Assert.IsTrue(gameState.StateInfo.IsCheck);
			Assert.IsTrue(gameState.StateInfo.IsBlackCheck);
			Assert.IsFalse(gameState.StateInfo.IsWhiteCheck);
			Assert.IsTrue(gameState.StateInfo.IsCheckmate);
		}
	}
}