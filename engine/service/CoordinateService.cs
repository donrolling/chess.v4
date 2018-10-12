using chess.v4.engine.enumeration;
using chess.v4.engine.extensions;
using chess.v4.engine.interfaces;
using chess.v4.engine.model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Chess.ServiceLayer {

	public class CoordinateService : ICoordinateService {
		public const string Files = "abcdefgh";
		public List<DiagonalDirection> DiagonalLines { get; } = new List<DiagonalDirection> { DiagonalDirection.UpLeft, DiagonalDirection.UpRight, DiagonalDirection.DownLeft, DiagonalDirection.DownRight };
		public List<Direction> OrthogonalLines { get; } = new List<Direction> { Direction.RowUp, Direction.RowDown, Direction.FileUp, Direction.FileDown };

		public CoordinateService() {}

		public int CoordinatePairToPosition(int file, int rank) {
			var fileChar = IntToFile(file);
			var coord = fileChar + (rank + 1).ToString();
			return CoordinateToPosition(coord);
		}

		public string PositionToCoordinate(int position) {
			var file = PositionToFileChar(position);
			var rank = (PositionToRankInt(position) + 1).ToString();
			return string.Concat(file, rank);
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

		public char GetOppositeColor(char activeColor) {
			return activeColor == 'w' ? 'b' : 'w';
		}

		public Color Reverse(Color pieceColor) {
			return pieceColor == Color.White ? Color.Black : Color.White;
		}

		public int FileToInt(char file) {
			return (int)(file - 97);
		}

		public char IntToFile(int file) {
			return (char)(file + 97);
		}

		public int AbsDiff(int piecePos, int newPiecePos) {
			return Math.Abs(piecePos - newPiecePos);
		}

		public char PositionToFileChar(int position) {
			var file = (char)((position % 8) + 97);
			return file;
		}

		public int PositionToFileInt(int position) {
			var file = (position % 8);
			return file;
		}

		public int PositionToFile(int position) {
			var file = (position % 8);
			return file;
		}

		public int PositionToRankInt(int position) {
			var rank = (int)(position / 8);
			return rank;
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

		public List<Square> GetDiagonals(List<Square> squares, int position, Color pieceColor, bool ignoreKing = false) {
			var attacks = new List<Square>();
			foreach (var direction in DiagonalLines) {
				attacks.AddRange(GetDiagonalLine(squares, position, direction, pieceColor, ignoreKing));
			}
			return attacks;
		}

		public List<Square> GetDiagonalLine(List<Square> squares, int position, DiagonalDirection direction, Color pieceColor, bool ignoreKing) {
			var attacks = new List<Square>();
			int diagonalLine = getIteratorByDirectionEnum(direction);
			var newPosition = position;
			do {
				if (CanDoDiagonalsFromStartPosition(position, diagonalLine)) {
					newPosition = position + diagonalLine;
					if (!IsValidCoordinate(newPosition)) {
						break;
					}
					var square = squares.GetSquare(newPosition);
					if (square.Occupied) {
						var blockingPiece = square.Piece;
						if (CanAttackPiece(pieceColor, blockingPiece)) {
							attacks.Add(square);
						}
						var breakAfterAction = BreakAfterAction(ignoreKing, blockingPiece.Identity, pieceColor);
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

		public List<Square> GetOrthogonals(List<Square> squares, int position, Color pieceColor, bool ignoreKing = false) {
			var attacks = new List<Square>();
			foreach (var orthogonalLine in OrthogonalLines) {
				var line = GetOrthogonalLine(squares, position, orthogonalLine, pieceColor, ignoreKing);
				if (line != null && line.Any()) {
					attacks.AddRange(line);
				}
			}
			return attacks;
		}

		public List<Square> GetOrthogonalLine(List<Square> squares, int currentPosition, Direction direction, Color pieceColor, bool ignoreKing) {
			int endCondition = getEndCondition(direction, currentPosition);
			var attacks = new List<Square>();
			var iterator = getIteratorByDirectionEnum(direction);
			for (var position = currentPosition + iterator; position != endCondition + iterator; position = position + iterator) {
				if (!IsValidCoordinate(position)) { break; }
				var square = squares.GetSquare(position);
				if (square.Occupied) {
					var blockingPiece = squares.GetPiece(position);
					if (CanAttackPiece(pieceColor, blockingPiece.Identity)) {
						attacks.Add(square);
					}
					bool breakAfterAction = BreakAfterAction(ignoreKing, blockingPiece.Identity, pieceColor);
					if (breakAfterAction) {
						break;
					}
				} else {
					attacks.Add(square);
				}
			}
			return attacks;
		}

		public bool IsValidCoordinate(int position) {
			return position >= 0 && position <= 63;
		}

		public bool CanAttackPiece(Color pieceColor, char attackedPiece) {
			if (pieceColor == Color.White && char.IsLower(attackedPiece)) {
				return true;
			}
			if (pieceColor == Color.Black && char.IsUpper(attackedPiece)) {
				return true;
			}
			return false;
		}

		public bool CanAttackPiece(Color pieceColor, Piece attackedPiece) {
			if (attackedPiece == null) {
				return true;
			}
			return attackedPiece.Color != pieceColor;
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

		private bool CanDoDiagonalsFromStartPosition(int startPosition, int direction) {
			bool isLeftSide = startPosition % 8 == 0;
			bool isRightSide = startPosition % 8 == 7;

			if (isLeftSide && (direction == 7 || direction == -9)) { return false; }
			if (isRightSide && (direction == -7 || direction == 9)) { return false; }
			return true;
		}

		private bool IsValidDiagonalCoordinate(int position) {
			if (!IsValidCoordinate(position)) { return false; }
			if (position % 8 == 0 || position % 8 == 7) { return false; }
			if (position < 7 || position > 56) { return false; }
			return true;
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

		public bool BreakAfterAction(bool ignoreKing, char blockingPiece, Color pieceColor) {
			//if ignoreKing is true, then we won't break after we hit the king
			//because we're trying to determine if the king will be in check if he moves to one of these squares.
			bool breakAfterAction = false;
			if (ignoreKing) {
				bool isOpposingKing = IsOpposingKing(blockingPiece, pieceColor);
				if (!isOpposingKing) {
					breakAfterAction = true;
				}
			} else {
				breakAfterAction = true;
			}
			return breakAfterAction;
		}

		/// <summary>
		/// Determines if the char passed in is the king for the color opposite of the color passed in.
		/// </summary>
		/// <param name="piece">The piece that might be your opponent's king.</param>
		/// <param name="pieceColor">The color of the current player.</param>
		/// <returns></returns>
		public bool IsOpposingKing(char piece, Color pieceColor) {
			return pieceColor == Color.White ? (piece == 'k' ? true : false) : (piece == 'K' ? true : false);
		}

		public IEnumerable<Square> FindPiece(List<Square> squares, PieceType pieceType, Color color) {
			var pieceChar = GetCharFromPieceType(pieceType, color);
			return squares.Where(a => a.Piece != null && a.Piece.Identity == pieceChar);
		}

		public char GetCharFromPieceType(PieceType pieceType, Color color) {
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

		

		public Color GetColorFromChar(char piece) {
			if (char.IsLower(piece)) {
				return Color.Black;
			}
			return Color.White;
		}

		public bool DetermineCastleThroughCheck(List<Square> matrix, List<Square> enemyAttacks, string fen, Color color, int kingPos, int rookPos) {
			var oppositeColor = color == Color.White ? Color.Black : Color.White;
			//var enemyAttacks = this.PieceService.GetAttacks(oppositeColor, fen).SelectMany(a => a.Value);
			var positions = this.GetKingPositionsDuringCastle(kingPos, rookPos);
			var arePositionsAttacked = positions.Intersect<int>(enemyAttacks.Select(a => a.Index)).Any();
			return arePositionsAttacked;
		}

		public int[] GetKingPositionsDuringCastle(int kingPos, int rookPos) {
			int direction = kingPos < rookPos ? 1 : -1;
			int[] result = new int[2];
			for (int i = 0; i < 2; i++) {
				result[i] = kingPos + (direction * (i + 1));
			}
			return result;
		}
	}
}