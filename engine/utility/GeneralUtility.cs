using chess.v4.engine.enumeration;
using chess.v4.engine.model;

namespace chess.v4.engine.utility {

	public static class GeneralUtility {

		public static bool BreakAfterAction(bool ignoreKing, Piece piece, Color activeColor) {
			//if ignoreKing is true, then we won't break after we hit the king
			//because we're trying to determine if the king will be in check if he moves to one of these squares.
			if (!ignoreKing) { return true; }
			if (piece.PieceType != PieceType.King) {
				return false;
			}
			return !(activeColor == piece.Color);
		}

		public static bool CanAttackPiece(Color pieceColor, char attackedPiece) {
			if (pieceColor == Color.White && char.IsLower(attackedPiece)) {
				return true;
			}
			if (pieceColor == Color.Black && char.IsUpper(attackedPiece)) {
				return true;
			}
			return false;
		}

		public static bool CanAttackPiece(Color pieceColor, Piece attackedPiece) {
			if (attackedPiece == null) {
				return true;
			}
			return attackedPiece.Color != pieceColor;
		}

		public static char GetCharFromPieceType(PieceType pieceType, Color color) {
			switch (pieceType) {
				case PieceType.King:
					return color == Color.White ? 'K' : 'k';

				case PieceType.Queen:
					return color == Color.White ? 'Q' : 'q';

				case PieceType.Bishop:
					return color == Color.White ? 'B' : 'b';

				case PieceType.Knight:
					return color == Color.White ? 'N' : 'n';

				case PieceType.Rook:
					return color == Color.White ? 'R' : 'r';

				case PieceType.Pawn:
					return color == Color.White ? 'P' : 'p';
			}
			return 'I';
		}

		public static Color GetColorFromChar(char piece) {
			if (char.IsLower(piece)) {
				return Color.Black;
			}
			return Color.White;
		}

		public static bool IsValidCoordinate(int position) {
			return position >= 0 && position <= 63;
		}
	}
}