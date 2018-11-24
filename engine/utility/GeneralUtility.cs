using chess.v4.engine.extensions;
using chess.v4.models;
using chess.v4.models.enumeration;
using System.Collections.Generic;

namespace chess.v4.engine.Utility {

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

		public static (bool IsValidCoordinate, bool BreakAfterAction, bool CanAttackPiece, Square SquareToAdd) DetermineMoveViability(GameState gameState, Piece attackingPiece, int newPosition, bool ignoreKing) {
			if (!GeneralUtility.IsValidCoordinate(newPosition)) {
				return (false, false, false, null);
			}
			var newSquare = gameState.Squares.GetSquare(newPosition);
			if (!newSquare.Occupied) {
				return (true, false, true, newSquare);
			}
			var blockingPiece = newSquare.Piece;
			var canAttackPiece = GeneralUtility.CanAttackPiece(attackingPiece.Color, blockingPiece);
			if (!canAttackPiece) {
				return (true, true, false, null);
			}
			var breakAfterAction = GeneralUtility.BreakAfterAction(ignoreKing, blockingPiece, newSquare.Piece.Color);
			return (true, breakAfterAction, true, newSquare);
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

		public static List<int> GetEntireFile(int file) {
			var attacks = new List<int>();

			var ind = file % 8;
			attacks.Add(ind);
			for (var i = 1; i < 8; i++) {
				attacks.Add((i * 8) + ind);
			}

			return attacks;
		}

		public static List<int> GetEntireRank(int rank) {
			var attacks = new List<int>();

			var ind = (rank % 8) * 8;
			attacks.Add(ind);
			for (var i = 1; i < 8; i++) {
				attacks.Add(ind + i);
			}

			return attacks;
		}

		public static bool GivenOrthogonalMove_IsItARankMove(int p1, int p2) {
			var positions = new List<int> { 8, -8 };
			return positions.Contains(p1 - p2);
		}

		public static bool IsOrthogonal(int p1, int p2) {
			var rank = GetEntireRank(NotationUtility.PositionToRankInt(p1));
			var file = GetEntireFile(p1);
			return rank.Contains(p2) || file.Contains(p2);
		}

		public static bool IsValidCoordinate(int position) {
			return position >= 0 && position <= 63;
		}
	}
}