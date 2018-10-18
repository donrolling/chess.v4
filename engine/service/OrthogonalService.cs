using chess.v4.engine.enumeration;
using chess.v4.engine.extensions;
using chess.v4.engine.interfaces;
using chess.v4.engine.model;
using chess.v4.engine.reference;
using chess.v4.engine.utility;
using System.Collections.Generic;
using System.Linq;

namespace chess.v4.engine.service {

	public class OrthogonalService : IOrthogonalService {
		public ICoordinateService CoordinateService { get; }
		public IMoveService MoveService { get; }

		public OrthogonalService(ICoordinateService coordinateService, IMoveService moveService) {
			CoordinateService = coordinateService;
			MoveService = moveService;
		}

		public List<int> GetEntireFile(int file) {
			var attacks = new List<int>();

			var ind = file % 8;
			attacks.Add(ind);
			for (var i = 1; i < 8; i++) {
				attacks.Add((i * 8) + ind);
			}

			return attacks;
		}

		public List<int> GetEntireRank(int rank) {
			var attacks = new List<int>();

			var ind = (rank % 8) * 8;
			attacks.Add(ind);
			for (var i = 1; i < 8; i++) {
				attacks.Add(ind + i);
			}

			return attacks;
		}

		public List<Square> GetOrthogonalLine(GameState gameState, Square movingSquare, Direction direction, bool ignoreKing = false) {
			var currentPosition = movingSquare.Index;
			var endCondition = getEndCondition(direction, currentPosition);
			var attacks = new List<Square>();
			var iterator = getIteratorByDirectionEnum(direction);
			for (var position = currentPosition + iterator; position != endCondition + iterator; position = position + iterator) {
				var isValidCoordinate = this.CoordinateService.IsValidCoordinate(position);
				if (!isValidCoordinate) { break; }
				var moveViability = this.MoveService.DetermineMoveViability(gameState, position, ignoreKing);
				if (!moveViability.IsValidCoordinate) {
					continue;
				}
				if (moveViability.CanAttackPiece && moveViability.SquareToAdd != null) {
					attacks.Add(square);
				}
				if (moveViability.BreakAfterAction) {
					break;
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

		private int getEndCondition(Direction direction, int position) {
			var file = this.CoordinateService.PositionToFileInt(position);
			var rank = this.CoordinateService.PositionToRankInt(position);

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