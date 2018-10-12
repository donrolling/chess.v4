using System;
using chess.v4.engine.enumeration;
using chess.v4.engine.model;

namespace chess.v4.engine.utility {

	public static class NotationUtility {

		public static Piece GetPieceFromCharacter(char c) {
			return new Piece { 
				Identity = c,
				Color = GetColorFromCharacter(c),
				PieceType = GetPieceTypeFromCharacter(c)
			};
		}

		public static Color GetColorFromCharacter(char c) {
			return char.IsUpper(c) ? Color.White : Color.Black;
		}

		public static PieceType GetPieceTypeFromCharacter(char c) {
			var identity = char.ToLower(c);
			switch (identity) {
				case 'p':
					return PieceType.Pawn;
				case 'n':
					return PieceType.Knight;
				case 'b':
					return PieceType.Bishop;
				case 'r':
					return PieceType.Rook;
				case 'q':
					return PieceType.Queen;
				case 'k':
					return PieceType.King;
				default:
					throw new Exception("Incorrect notation!");
			}
		}
	}
}