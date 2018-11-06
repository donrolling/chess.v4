using chess.v4.models.enumeration;
using chess.v4.engine.extensions;
using chess.v4.models;
using System.Collections.Generic;
using System.Linq;

namespace chess.v4.engine.Utility {

	public static class CastleUtility {
	
		public static bool DetermineCastleThroughCheck(GameState gameState, int kingPos, int rookPos) {
			var oppositeColor = gameState.ActiveColor.Reverse();
			var positions = CastleUtility.GetKingPositionsDuringCastle(kingPos, rookPos);
			var opponentAttacks = gameState.Attacks.Where(a => a.AttackingSquare.Piece.Color == oppositeColor).Select(a => a.Index);
			var arePositionsAttacked = positions.Intersect<int>(opponentAttacks).Any();
			return arePositionsAttacked;
		}

		public static int[] GetKingPositionsDuringCastle(int kingPos, int rookPos) {
			var direction = kingPos < rookPos ? 1 : -1;
			var result = new int[2];
			for (int i = 0; i < 2; i++) {
				result[i] = kingPos + (direction * (i + 1));
			}
			return result;
		}

		public static (int RookPos, int NewRookPos) GetRookPositionsForCastle(Color color, int piecePosition, int newPiecePosition) {
			//manage the castle
			var rookRank = color == Color.White ? 1 : 8; //intentionally not zero based
			var rookFile = NotationUtility.IntToFile(piecePosition - newPiecePosition > 0 ? 0 : 7);
			var rookPos = NotationUtility.CoordinateToPosition(string.Concat(rookFile, rookRank.ToString()));

			var newRookFile = NotationUtility.IntToFile(piecePosition - newPiecePosition > 0 ? 3 : 5);
			var newRookPos = NotationUtility.CoordinateToPosition(string.Concat(newRookFile, rookRank.ToString()));

			return (rookPos, newRookPos);
		}
	}
}