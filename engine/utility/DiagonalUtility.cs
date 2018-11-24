using chess.v4.engine.extensions;
using chess.v4.engine.reference;
using chess.v4.engine.Utility;
using chess.v4.models;
using chess.v4.models.enumeration;
using System.Collections.Generic;
using System.Linq;

namespace chess.v4.engine.utility {

	public static class DiagonalUtility {
		private static List<int> HFile = GeneralUtility.GetEntireFile(7);

		public static List<Square> GetDiagonalLine(GameState gameState, Square square, Piece attackingPiece, DiagonalDirection direction, bool ignoreKing) {
			var attacks = new List<Square>();
			var diagonalLine = getIteratorByDirectionEnum(direction);
			var position = square.Index;
			var attackPosition = square.Index;
			do {
				if (!canDoDiagonalsFromStartPosition(position, diagonalLine)) {
					break;
				}
				attackPosition = attackPosition + diagonalLine;
				var moveViability = GeneralUtility.DetermineMoveViability(gameState, attackingPiece, attackPosition, ignoreKing);
				if (!moveViability.IsValidCoordinate) {
					continue;
				}
				if (moveViability.CanAttackPiece && moveViability.SquareToAdd != null) {
					var attackSquare = gameState.Squares.GetSquare(attackPosition);
					attacks.Add(attackSquare);
				}
				if (moveViability.BreakAfterAction) {
					break;
				}
			} while (isValidDiagonalCoordinate(attackPosition));
			return attacks;
		}

		public static void GetDiagonals(GameState gameState, Square square, List<AttackedSquare> accumulator, bool ignoreKing = false) {
			var attacks = new List<Square>();
			foreach (var direction in GeneralReference.DiagonalLines) {
				attacks.AddRange(GetDiagonalLine(gameState, square, square.Piece, direction, ignoreKing));
			}
			if (attacks.Any()) {
				accumulator.AddRange(attacks.Select(a => new AttackedSquare(square, a)));
			}
		}

		public static List<int> GetEntireDiagonalByFile(int file, DiagonalDirectionFromFileNumber direction) {
			var list = new List<int>();
			var increment = getDiagonalIncrement(direction);
			var numberOfSquares = howFarToGo(file, direction);
			var index = file;
			for (int i = 0; i < numberOfSquares; i++) {
				list.Add(index);
				index = index + increment;
			}
			return list;
		}

		public static List<Square> GetEntireDiagonalByFile(GameState gameState, int file, DiagonalDirectionFromFileNumber direction) {
			var indexes = GetEntireDiagonalByFile(file, direction);
			var list = new List<Square>();
			indexes.ForEach(a => list.Add(gameState.Squares.GetSquare(a)));
			return list;
		}

		public static List<int> GetEntireDiagonalByPosition(int position) {
			var list = new List<int>();
			foreach (var direction in GeneralReference.DiagonalLines) {
				var result = getEntireDiagonalLine(direction, position);
				list.AddRange(result);
			}
			return list;
		}

		public static bool IsDiagonal(int p1, int p2) {
			var list = GetEntireDiagonalByPosition(p1);
			return list.Contains(p2);
		}

		private static bool canDoDiagonalsFromStartPosition(int startPosition, int direction) {
			var isLeftSide = startPosition % 8 == 0;
			var isRightSide = startPosition % 8 == 7;
			if (isLeftSide && (direction == 7 || direction == -9)) { return false; }
			if (isRightSide && (direction == -7 || direction == 9)) { return false; }
			return true;
		}

		private static int getDiagonalIncrement(DiagonalDirectionFromFileNumber direction) {
			return direction == DiagonalDirectionFromFileNumber.Left ? 7 : 9;
		}

		private static List<int> getEntireDiagonalLine(DiagonalDirection direction, int position) {
			var list = new List<int> { position };
			var increment = 0;
			var currentPosition = position;
			switch (direction) {
				case DiagonalDirection.UpLeft:
					if (position % 8 == 0) { return list; }
					if (position > 56) { return list; }
					increment = 7;
					while (currentPosition <= 63) {
						currentPosition = currentPosition + increment;
						if (currentPosition <= 63) {
							list.Add(currentPosition);
						}
						if (currentPosition % 8 == 0) { break; }
					}
					break;

				case DiagonalDirection.UpRight:
					if (HFile.Contains(position)) { return list; }
					if (position > 56) { return list; }
					increment = 9;
					while (currentPosition <= 63) {
						currentPosition = currentPosition + increment;
						if (currentPosition <= 63) {
							list.Add(currentPosition);
						}
						if (HFile.Contains(currentPosition)) { break; }
					}
					break;

				case DiagonalDirection.DownLeft:
					if (position % 8 == 0) { return list; }
					if (position < 7) { return list; }
					increment = -9;
					while (currentPosition >= 0) {
						currentPosition = currentPosition + increment;
						if (currentPosition >= 0) {
							list.Add(currentPosition);
						}
						if (currentPosition % 8 == 0) { break; }
					}
					break;

				case DiagonalDirection.DownRight:
					if (HFile.Contains(position)) { return list; }
					if (position < 7) { return list; }
					increment = -7;
					while (currentPosition >= 0) {
						currentPosition = currentPosition + increment;
						if (currentPosition >= 0) {
							list.Add(currentPosition);
						}
						if (HFile.Contains(currentPosition)) { break; }
					}
					break;

				case DiagonalDirection.Invalid:
				default:
					break;
			}
			return list;
		}

		private static int getIteratorByDirectionEnum(DiagonalDirection direction) {
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

		private static object howFarToGo(int rank, int file, DiagonalDirectionFromFileNumber direction) {
			var f1 = howFarToGo(file, direction);

			var result = 0;
			return result;
		}

		private static int howFarToGo(int rank, DiagonalDirectionFromRankNumber direction) {
			return direction == DiagonalDirectionFromRankNumber.Up ? 8 - rank : rank - 1;
		}

		private static int howFarToGo(int file, DiagonalDirectionFromFileNumber direction) {
			return direction == DiagonalDirectionFromFileNumber.Left ? file + 1 : 8 - file;
		}

		private static bool isValidDiagonalCoordinate(int position) {
			if (!GeneralUtility.IsValidCoordinate(position)) { return false; }
			if (position % 8 == 0 || position % 8 == 7) { return false; }
			if (position < 7 || position > 56) { return false; }
			return true;
		}
	}
}