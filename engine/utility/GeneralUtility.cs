using chess.v4.models.enumeration;
using chess.v4.engine.extensions;
using chess.v4.models;
using System;
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

		public static bool IsDiagonal(int p1, int p2) {
			var positions = new List<int> { 7, 9, -7, -9 };
			return positions.Contains(p1 - p2);
		}

		public static bool IsOrthogonal(int p1, int p2) {
			var positions = new List<int> { 8, -8, 1, -1 };
			return positions.Contains(p1 - p2);
		}

		public static bool GivenOrthogonalMove_IsItARankMove(int p1, int p2) {			
			var positions = new List<int> { 8, -8 };
			return positions.Contains(p1 - p2);
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
	}
}