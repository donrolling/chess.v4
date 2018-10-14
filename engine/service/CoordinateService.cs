using chess.v4.engine.enumeration;
using chess.v4.engine.extensions;
using chess.v4.engine.interfaces;
using chess.v4.engine.model;
using chess.v4.engine.reference;
using chess.v4.engine.utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Chess.ServiceLayer {

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

		public List<Square> GetDiagonalLine(GameState gameState, Square square, DiagonalDirection direction, bool ignoreKing) {
			var attacks = new List<Square>();
			var diagonalLine = getIteratorByDirectionEnum(direction);
			var position = square.Index;
			var newPosition = square.Index;
			do {
				if (canDoDiagonalsFromStartPosition(position, diagonalLine)) {
					newPosition = position + diagonalLine;
					if (!IsValidCoordinate(newPosition)) {
						break;
					}
					var newSquare = gameState.Squares.GetSquare(newPosition);
					if (newSquare.Occupied) {
						var blockingPiece = newSquare.Piece;
						if (GeneralUtility.CanAttackPiece(newSquare.Piece.Color, blockingPiece)) {
							attacks.Add(square);
						}
						var breakAfterAction = GeneralUtility.BreakAfterAction(ignoreKing, blockingPiece.Identity, newSquare.Piece.Color);
						if (breakAfterAction) {
							break;
						}
					} else {
						attacks.Add(square);
					}
				}
			} while (IsValidDiagonalCoordinate(newPosition));
			return attacks;
		}

		public List<Square> GetDiagonals(GameState gameState, Square square, bool ignoreKing = false) {
			var attacks = new List<Square>();
			foreach (var direction in GeneralReference.DiagonalLines) {
				attacks.AddRange(GetDiagonalLine(gameState, square, direction, ignoreKing));
			}
			return attacks;
		}

		public List<int> GetEntireFile(int file) {
			List<int> attacks = new List<int>();

			var ind = file % 8;
			attacks.Add(ind);
			for (int i = 1; i < 8; i++) {
				attacks.Add((i * 8) + ind);
			}

			return attacks;
		}

		public List<int> GetEntireRank(int rank) {
			List<int> attacks = new List<int>();

			var ind = (rank % 8) * 8;
			attacks.Add(ind);
			for (int i = 1; i < 8; i++) {
				attacks.Add(ind + i);
			}

			return attacks;
		}

		public List<Square> GetOrthogonalLine(GameState gameState, Square square, Direction direction, bool ignoreKing = false) {
			var currentPosition = square.Index;
			var endCondition = getEndCondition(direction, currentPosition);
			var attacks = new List<Square>();
			var iterator = getIteratorByDirectionEnum(direction);
			for (var position = currentPosition + iterator; position != endCondition + iterator; position = position + iterator) {
				if (!IsValidCoordinate(position)) { break; }
				if (square.Occupied) {
					var blockingPiece = gameState.Squares.GetPiece(position);
					if (GeneralUtility.CanAttackPiece(square.Piece.Color, blockingPiece.Identity)) {
						attacks.Add(square);
					}
					var breakAfterAction = GeneralUtility.BreakAfterAction(ignoreKing, blockingPiece.Identity, square.Piece.Color);
					if (breakAfterAction) {
						break;
					}
				} else {
					attacks.Add(square);
				}
			}
			return attacks;
		}

		public List<Square> GetOrthogonals(GameState gameState, Square square, bool ignoreKing = false) {
			var attacks = new List<Square>();
			foreach (var orthogonalLine in GeneralReference.OrthogonalLines) {
				var line = GetOrthogonalLine(gameState, square, orthogonalLine, ignoreKing);
				if (line != null && line.Any()) {
					attacks.AddRange(line);
				}
			}
			return attacks;
		}

		public char IntToFile(int file) {
			return (char)(file + 97);
		}

		public bool IsDiagonalMove(int startPosition, int endPosition) {
			var startMod = startPosition % 8;
			var endMod = endPosition % 8;
			var modDiff = Math.Abs(startMod - endMod);

			var startRow = this.PositionToRankInt(startPosition);
			var endRow = this.PositionToRankInt(endPosition);
			var rowDiff = Math.Abs(startRow - endRow);
			if (modDiff == rowDiff) {
				return true;
			}
			return false;
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

		private bool canDoDiagonalsFromStartPosition(int startPosition, int direction) {
			bool isLeftSide = startPosition % 8 == 0;
			bool isRightSide = startPosition % 8 == 7;

			if (isLeftSide && (direction == 7 || direction == -9)) { return false; }
			if (isRightSide && (direction == -7 || direction == 9)) { return false; }
			return true;
		}

		private int getEndCondition(Direction direction, int position) {
			int file = this.PositionToFileInt(position);
			int rank = this.PositionToRankInt(position);

			switch (direction) {
				case Direction.RowUp:
					return this.GetEntireFile(file).Max();

				case Direction.RowDown:
					return this.GetEntireFile(file).Min();

				case Direction.FileUp:
					return this.GetEntireRank(rank).Max();

				case Direction.FileDown:
					return this.GetEntireRank(rank).Min();
			}

			return 0;
		}

		private int getIteratorByDirectionEnum(Direction direction) {
			switch (direction) {
				case Direction.RowUp:
					return 8;

				case Direction.RowDown:
					return -8;

				case Direction.FileUp:
					return 1;

				case Direction.FileDown:
					return -1;
			}
			return 0;
		}

		private int getIteratorByDirectionEnum(DiagonalDirection direction) {
			switch (direction) {
				case DiagonalDirection.UpLeft:
					return 7;

				case DiagonalDirection.UpRight:
					return 9;

				case DiagonalDirection.DownLeft:
					return -9;

				case DiagonalDirection.DownRight:
					return -7;
			}
			return 0;
		}

		private bool IsValidDiagonalCoordinate(int position) {
			if (!IsValidCoordinate(position)) { return false; }
			if (position % 8 == 0 || position % 8 == 7) { return false; }
			if (position < 7 || position > 56) { return false; }
			return true;
		}
	}
}