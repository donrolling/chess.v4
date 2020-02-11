using Chess.v4.Engine.Extensions;
using Chess.v4.Engine.Models;
using Chess.v4.Models;
using Chess.v4.Models.Enums;
using System;
using System.Collections.Generic;

namespace Chess.v4.Engine.Utility
{
	public static class GeneralUtility
	{
		public static bool BreakAfterAction(Piece piece, Color activeColor)
		{
			//if (!ignoreKing) { return true; }
			//if ignoreKing is true, then we won't break after we hit the king
			//because we're trying to determine if the king will be in check if he moves to one of these squares.
			if (piece.PieceType != PieceType.King)
			{
				return false;
			}
			var isEnemyPiece = activeColor != piece.Color;
			return !isEnemyPiece;
		}

		public static bool CanAttackPiece(Color pieceColor, char attackedPiece)
		{
			if (pieceColor == Color.White && char.IsLower(attackedPiece))
			{
				return true;
			}
			if (pieceColor == Color.Black && char.IsUpper(attackedPiece))
			{
				return true;
			}
			return false;
		}

		public static MoveViability DetermineMoveViability(GameState gameState, Piece attacker, int position)
		{
			if (!GeneralUtility.IsValidCoordinate(position))
			{
				return new MoveViability(false, false, false, null);
			}
			var square = gameState.Squares.GetSquare(position);
			if (!square.Occupied)
			{
				return new MoveViability(true, false, false, square);
			}
			var blockingPiece = square.Piece;
			var isTeamPiece = GeneralUtility.IsTeamPiece(attacker.Color, blockingPiece);
			if (!isTeamPiece)
			{
				return new MoveViability(true, true, true, square);
			}
			var breakAfterAction = GeneralUtility.BreakAfterAction(blockingPiece, attacker.Color);
			return new MoveViability(true, breakAfterAction, false, square);
		}

		public static char GetCharFromPieceType(PieceType pieceType, Color color)
		{
			switch (pieceType)
			{
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

		public static Color GetColorFromChar(char piece)
		{
			if (char.IsLower(piece))
			{
				return Color.Black;
			}
			return Color.White;
		}

		public static List<int> GetEntireFile(int file)
		{
			var attacks = new List<int>();

			var ind = file % 8;
			attacks.Add(ind);
			for (var i = 1; i < 8; i++)
			{
				attacks.Add((i * 8) + ind);
			}

			return attacks;
		}

		public static List<int> GetEntireRank(int rank)
		{
			var attacks = new List<int>();

			var ind = (rank % 8) * 8;
			attacks.Add(ind);
			for (var i = 1; i < 8; i++)
			{
				attacks.Add(ind + i);
			}

			return attacks;
		}

		public static bool GivenOrthogonalMove_IsItARankMove(int p1, int p2)
		{
			var positions = new List<int> { 8, -8 };
			return positions.Contains(p1 - p2);
		}

		public static bool IsDiagonal(int p1, int p2)
		{
			var diff = Math.Abs(p1 - p2);
			var ltr = diff % 9 == 0;
			var rtl = diff % 7 == 0;
			if (!ltr && !rtl)
			{
				return false;
			}
			return true;
		}

		public static bool IsOrthogonal(int p1, int p2)
		{
			var rank = GetEntireRank(NotationUtility.PositionToRankInt(p1));
			var file = GetEntireFile(p1);
			return rank.Contains(p2) || file.Contains(p2);
		}

		public static bool IsTeamPiece(Color pieceColor, Piece attackedPiece)
		{
			if (attackedPiece == null)
			{
				return true;
			}
			return attackedPiece.Color != pieceColor;
		}

		public static bool IsValidCoordinate(int position)
		{
			return position >= 0 && position <= 63;
		}
	}
}