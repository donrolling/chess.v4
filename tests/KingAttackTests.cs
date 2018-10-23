using chess.v4.engine.enumeration;
using chess.v4.engine.interfaces;
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
		public void KingAttack() {
			var gameState = TestUtility.GetGameState(this.GameStateService);
			var kingAttacks = AttackService.GetPieceAttacks(FEN.StartingPosition, new KeyValuePair<int, char>(4, 'K'));
			Assert.AreEqual(0, kingAttacks.Count());

			//artifically placing the white king on square 44 to see his attacks
			var kingAttacks2 = AttackService.GetPieceAttacks(FEN.StartingPosition, new KeyValuePair<int, char>(44, 'K'));
			Assert.AreEqual(3, kingAttacks2.Count());

			//test white king move availability from g1
			var kingAttacks3 = AttackService.GetPieceAttacks("3q1rk1/5pbp/5Qp1/8/8/2B5/5PPP/6K1 w - - 0 1", new KeyValuePair<int, char>(6, 'K'));
			Assert.AreEqual(2, kingAttacks3.Count());

			//it actually should be white's move here, but not for this test
			//move queen down to checkmate the king and move sure that the king has no attacks
			var kingAttacks4 = AttackService.GetPieceAttacks("3q1rk1/5pbp/5Qp1/8/8/2B5/5PPP/6K1 b - - 0 1", new KeyValuePair<int, char>(6, 'K'));
			Assert.IsTrue(kingAttacks4.Contains(5));
			Assert.IsTrue(kingAttacks4.Contains(7));
			Assert.AreEqual(2, kingAttacks4.Count());

			var kingAttacks5 = AttackService.GetPieceAttacks("r2qkbnr/p2ppppp/b1n5/1pp5/4P3/BPN5/P1PPQPPP/R3KBNR w KQkq - 3 5", new KeyValuePair<int, char>(4, 'K'));
			Assert.IsTrue(kingAttacks5.Contains(2));
			Assert.IsTrue(kingAttacks5.Contains(3));
			Assert.AreEqual(2, kingAttacks5.Count());
		}

		[TestMethod]
		public void KingAttacks2() {
			//king can't move in front of a pawn, this is wrong, let's fix it
			//var kingAttacks6 = AttackService.GetPieceAttacks("rnbqkbnr/1pp2ppp/4p3/p2p4/4P3/4K3/PPPP1PPP/RNBQ1BNR w kq a6 0 4", new KeyValuePair<int, char>(20, 'K'));
			//Assert.AreEqual(5, kingAttacks6.Count());
			string fen = "rnbqkbnr/1pp2ppp/4p3/p2p4/4P3/4K3/PPPP1PPP/RNBQ1BNR w kq a6 0 4";
			Game game = new Game(fen);
			game.Move(Color.White, "Kd4");
			Assert.IsTrue(game.Board.MoveSuccess);
		}

		[TestMethod]
		public void KingAttacks3() {
			var positions = CoordinateService.GetKingPositionsDuringCastle(60, 63);
			Assert.AreEqual(2, positions.Count());
			Assert.AreEqual(61, positions[0]);
			Assert.AreEqual(62, positions[1]);

			var positions2 = CoordinateService.GetKingPositionsDuringCastle(60, 56);
			Assert.AreEqual(2, positions2.Count());
			Assert.AreEqual(59, positions2[0]);
			Assert.AreEqual(58, positions2[1]);

			var positions3 = CoordinateService.GetKingPositionsDuringCastle(4, 7);
			Assert.AreEqual(2, positions3.Count());
			Assert.AreEqual(5, positions3[0]);
			Assert.AreEqual(6, positions3[1]);

			var positions4 = CoordinateService.GetKingPositionsDuringCastle(4, 0);
			Assert.AreEqual(2, positions4.Count());
			Assert.AreEqual(3, positions4[0]);
			Assert.AreEqual(2, positions4[1]);
		}

		[TestMethod]
		public void KingAttacks4() {
			string fen = "r1bqk2r/ppp2ppp/8/3P4/1Q2P3/6P1/PPP2P1P/R1B1KB1R b KQkq - 0 9";
			Game game = new Game(fen);

			bool isCastleThroughCheck = CoordinateService.DetermineCastleThroughCheck(game.Board.Matrix, game.Board.FEN, game.Board.ActiveChessTypeColor, 60, 63);
			Assert.IsTrue(isCastleThroughCheck);

			game.Move(Color.Black, "Kg8");
			Assert.IsFalse(game.Board.MoveSuccess);
		}

		[TestMethod]
		public void KingAttacks5() {
			string fen = "r3k2r/pp3ppp/2p2q2/3P3b/1Q2PB2/1P6/P1P1BPPP/R3K2R w KQkq - 3 13";
			Game game = new Game(fen);

			bool isCastleThroughCheck = CoordinateService.DetermineCastleThroughCheck(game.Board.Matrix, game.Board.FEN, game.Board.ActiveChessTypeColor, 4, 0);
			Assert.IsFalse(isCastleThroughCheck);

			game.Move(Color.White, "Kc1");
			Assert.IsTrue(game.Board.MoveSuccess);
		}

		[TestMethod]
		public void KingAttacks6() {
			string fen = "2r5/8/2n1Q3/5R1k/3Pp1p1/p1P3Pp/P6P/R1BK4 b - - 1 41";
			//Game game = new Game(fen);
			//game.Move(Color.White, "Rf5");
			//Assert.IsTrue(game.Board.MoveSuccess);

			var kingAttacks2 = AttackService.GetPieceAttacks(fen, new KeyValuePair<int, char>(39, 'k'));
			Assert.AreEqual(0, kingAttacks2.Count());
		}
	}
}