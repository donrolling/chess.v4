﻿using chess.v4.engine.enumeration;
using chess.v4.engine.extensions;
using chess.v4.engine.interfaces;
using chess.v4.engine.model;
using chess.v4.engine.reference;
using chess.v4.engine.utility;
using System.Collections.Generic;

namespace chess.v4.engine.service {

	public class DiagonalService : IDiagonalService {
		public DiagonalService() {

		}

		public List<Square> GetDiagonalLine(GameState gameState, Square square, Piece attackingPiece, DiagonalDirection direction, bool ignoreKing) {
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

		public List<Square> GetDiagonals(GameState gameState, Square square, bool ignoreKing = false) {
			var attacks = new List<Square>();
			foreach (var direction in GeneralReference.DiagonalLines) {
				attacks.AddRange(GetDiagonalLine(gameState, square, square.Piece, direction, ignoreKing));
			}
			return attacks;
		}

		private bool canDoDiagonalsFromStartPosition(int startPosition, int direction) {
			var isLeftSide = startPosition % 8 == 0;
			var isRightSide = startPosition % 8 == 7;
			if (isLeftSide && (direction == 7 || direction == -9)) { return false; }
			if (isRightSide && (direction == -7 || direction == 9)) { return false; }
			return true;
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

		private bool isValidDiagonalCoordinate(int position) {
			if (!GeneralUtility.IsValidCoordinate(position)) { return false; }
			if (position % 8 == 0 || position % 8 == 7) { return false; }
			if (position < 7 || position > 56) { return false; }
			return true;
		}
	}
}