using chess.v4.models.enumeration;
using chess.v4.models;
using System;

namespace chess.v4.engine.utility {

	public static class NotationUtility {

		public static int CoordinatePairToPosition(int file, int rank) {
			var fileChar = IntToFile(file);
			var coord = fileChar + (rank + 1).ToString();
			return CoordinateToPosition(coord);
		}

		public static int CoordinateToPosition(string coordinate) {
			if (coordinate == "-") {
				throw new Exception("Invalid Coordinate");
			}

			var coord = coordinate.Substring(coordinate.Length - 2, 2);
			var file = FileToInt(coord[0]);

			int rank = 0;
			Int32.TryParse(coord[1].ToString(), out rank);

			return file + (8 * (rank - 1));
		}

		public static int FileToInt(char file) {
			return (int)(file - 97);
		}

		public static Color GetColorFromCharacter(char c) {
			return char.IsUpper(c) ? Color.White : Color.Black;
		}

		public static Piece GetPieceFromCharacter(char c) {
			return new Piece (GetPieceTypeFromCharacter(c),	GetColorFromCharacter(c));
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

		public static char IntToFile(int file) {
			return (char)(file + 97);
		}

		public static string PositionToCoordinate(int position) {
			var file = PositionToFileChar(position);
			var rank = (PositionToRankInt(position) + 1).ToString();
			return string.Concat(file, rank);
		}

		public static int PositionToFile(int position) {
			var file = (position % 8);
			return file;
		}
		
		public static int PositionToRank(int position) {
			var rank = (position / 8);
			return rank;
		}

		public static char PositionToFileChar(int position) {
			var file = (char)((position % 8) + 97);
			return file;
		}

		public static int PositionToFileInt(int position) {
			var file = (position % 8);
			return file;
		}

		public static int PositionToRankInt(int position) {
			var rank = (int)(position / 8);
			return rank;
		}
	}
}