using chess.v4.engine.enumeration;

namespace chess.v4.engine.utility {

	public static class CastleUtility {

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