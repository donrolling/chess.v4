using chess.v4.engine.enumeration;
using chess.v4.engine.interfaces;
using System;

namespace chess.v4.engine.service {

	public class CoordinateService : ICoordinateService {

		public CoordinateService() {
		}

		public int CoordinatePairToPosition(int file, int rank) {
			var fileChar = IntToFile(file);
			var coord = fileChar + (rank + 1).ToString();
			return CoordinateToPosition(coord);
		}

		public int CoordinateToPosition(string coordinate) {
			if (coordinate == "-") { return -1; }

			var coord = coordinate.Substring(coordinate.Length - 2, 2);
			var file = FileToInt(coord[0]);

			int rank = 0;
			Int32.TryParse(coord[1].ToString(), out rank);

			int position = file + (8 * (rank - 1));
			return position;
		}

		public int FileToInt(char file) {
			return (int)(file - 97);
		}

		public char IntToFile(int file) {
			return (char)(file + 97);
		}

		public bool IsValidCoordinate(int position) {
			return position >= 0 && position <= 63;
		}

		public string PositionToCoordinate(int position) {
			var file = PositionToFileChar(position);
			var rank = (PositionToRankInt(position) + 1).ToString();
			return string.Concat(file, rank);
		}

		public int PositionToFile(int position) {
			var file = (position % 8);
			return file;
		}

		public char PositionToFileChar(int position) {
			var file = (char)((position % 8) + 97);
			return file;
		}

		public int PositionToFileInt(int position) {
			var file = (position % 8);
			return file;
		}

		public int PositionToRankInt(int position) {
			var rank = (int)(position / 8);
			return rank;
		}

		public Color Reverse(Color pieceColor) {
			return pieceColor == Color.White ? Color.Black : Color.White;
		}
	}
}