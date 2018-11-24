using chess.v4.models.enumeration;
using chess.v4.engine.interfaces;
using chess.v4.models;
using chess.v4.engine.reference;
using chess.v4.engine.Utility;
using System.Collections.Generic;
using System.Linq;

namespace chess.v4.engine.service {
	public class OrthogonalService : IOrthogonalService {

		public OrthogonalService() {
		}

		public List<int> GetEntireFile(int file) {
			return GeneralUtility.GetEntireFile(file);
		}

		public List<int> GetEntireRank(int rank) {
			return GeneralUtility.GetEntireRank(rank);
		}

		public List<Square> GetOrthogonalLine(GameState gameState, Square movingSquare, Direction direction, bool ignoreKing = false) {
			var currentPosition = movingSquare.Index;
			var endCondition = getEndCondition(direction, currentPosition);
			var attacks = new List<Square>();
			var iterator = getIteratorByDirectionEnum(direction);
			for (var position = currentPosition + iterator; position != endCondition + iterator; position = position + iterator) {
				var isValidCoordinate = GeneralUtility.IsValidCoordinate(position);
				if (!isValidCoordinate) { break; }
				var moveViability = GeneralUtility.DetermineMoveViability(gameState, movingSquare.Piece, position, ignoreKing);
				if (!moveViability.IsValidCoordinate) {
					continue;
				}
				if (moveViability.CanAttackPiece && moveViability.SquareToAdd != null) {
					attacks.Add(moveViability.SquareToAdd);
				}
				if (moveViability.BreakAfterAction) {
					break;
				}
			}
			return attacks;
		}

		public void GetOrthogonals(GameState gameState, Square square, List<AttackedSquare> accumulator, bool ignoreKing = false) {
			var attacks = new List<Square>();
			foreach (var orthogonalLine in GeneralReference.OrthogonalLines) {
				var line = GetOrthogonalLine(gameState, square, orthogonalLine, ignoreKing);
				if (line != null && line.Any()) {
					attacks.AddRange(line);
				}
			}
			if (attacks.Any()) {
				accumulator.AddRange(attacks.Select(a => new AttackedSquare(square, a)));
			}
		}

		private int getEndCondition(Direction direction, int position) {
			var file = NotationUtility.PositionToFileInt(position);
			var rank = NotationUtility.PositionToRankInt(position);

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
	}
}