using chess.v4.engine.enumeration;
using chess.v4.engine.extensions;
using chess.v4.engine.interfaces;
using chess.v4.engine.model;
using chess.v4.engine.reference;
using chess.v4.engine.utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace chess.v4.engine.service {

	public class AttackService : IAttackService {
		public ICoordinateService CoordinateService { get; }
		public IDiagonalService DiagonalService { get; }
		public INotationService NotationService { get; }
		public IOrthogonalService OrthogonalService { get; }

		public AttackService(INotationService notationService, IOrthogonalService orthogonalService, IDiagonalService diagonalService, ICoordinateService coordinateService) {
			NotationService = notationService;
			OrthogonalService = orthogonalService;
			DiagonalService = diagonalService;
			CoordinateService = coordinateService;
		}

		public IEnumerable<AttackedSquare> GetAttacks(GameState gameState, bool ignoreKing = false) {
			var allAttacks = new List<AttackedSquare>();
			foreach (var square in gameState.Squares) {
				var list = this.getPieceAttacks(gameState, square, ignoreKing);
				if (list.Any()) {
					allAttacks.AddRange(list);
				}
			}
			return allAttacks;
		}

		public IEnumerable<Square> GetKingAttack(AttackedSquare attackedSquare, GameState gameState, Square enemyKing) {
			var theAttack = new List<Square>();
			switch (attackedSquare.AttackerSquare.Piece.PieceType) {
				case PieceType.Pawn | PieceType.Knight | PieceType.King: //you can't interpose a pawn or a knight attack, also a king cannot attack a king
					break;

				case PieceType.Bishop:
					foreach (var direction in GeneralReference.DiagonalLines) {
						var potentialAttack = this.DiagonalService.GetDiagonalLine(gameState, attackedSquare.AttackerSquare, direction, true);
						if (potentialAttack.Any(a => a.Index == enemyKing.Index)) {
							theAttack.AddRange(potentialAttack);
							break;
						}
					}
					break;

				case PieceType.Rook:
					foreach (var direction in GeneralReference.OrthogonalLines) {
						var potentialAttack = this.OrthogonalService.GetOrthogonalLine(gameState, attackedSquare.AttackerSquare, direction, true);
						if (potentialAttack.Any(a => a.Index == enemyKing.Index)) {
							theAttack.AddRange(potentialAttack);
							break;
						}
					}
					break;

				case PieceType.Queen:
					foreach (var direction in GeneralReference.DiagonalLines) {
						var potentialAttack = this.DiagonalService.GetDiagonalLine(gameState, attackedSquare.AttackerSquare, direction, true);
						if (potentialAttack.Any(a => a.Index == enemyKing.Index)) {
							theAttack.AddRange(potentialAttack);
							break;
						}
					}
					foreach (var direction in GeneralReference.OrthogonalLines) {
						var potentialAttack = this.OrthogonalService.GetOrthogonalLine(gameState, attackedSquare.AttackerSquare, direction, true);
						if (potentialAttack.Any(a => a.Index == enemyKing.Index)) {
							theAttack.AddRange(potentialAttack);
							break;
						}
					}
					break;
			}
			return theAttack;
		}

		public IEnumerable<AttackedSquare> GetKingAttacks(GameState gameState, Square square) {
			var attacks = new List<AttackedSquare>();
			var squares = gameState.Squares;
			var position = square.Index;
			var pieceColor = square.Piece.Color;
			var castleAvailability = gameState.CastlingAvailability;
			var positionList = new List<int> { -9, -8, -7, -1, 1, 7, 8, 9 };

			if ( //make sure castle is available
				(pieceColor == Color.White && (castleAvailability.Contains("K") || castleAvailability.Contains("Q")))
				|| (pieceColor == Color.Black && (castleAvailability.Contains("k") || castleAvailability.Contains("q")))
			) {
				positionList.Add(2);
				positionList.Add(-2);
			}

			if (position % 8 == 0) {
				positionList.Remove(-1);
				positionList.Remove(-9);
				positionList.Remove(7);
			}
			if (position % 8 == 7) {
				positionList.Remove(1);
				positionList.Remove(9);
				positionList.Remove(-7);
			}

			foreach (var positionShim in positionList) {
				var tempPos = position + positionShim;
				var isValidCoordinate = CoordinateService.IsValidCoordinate(tempPos);
				if (!isValidCoordinate) {
					continue;
				}
				var _isValidMove = isValidMove(squares, tempPos, pieceColor);
				if (!_isValidMove) {
					continue;
				}
				var isCastle = Math.Abs(positionShim) == 2; //are we trying to move two squares? if so, this is a castle attempt
				if (!isCastle) {
					attacks.Add(new AttackedSquare(square, squares.GetSquare(tempPos)));
					continue;
				}
				var direction = positionShim > 0 ? 1 : -1;
				int clearPathPos = tempPos;
				int clearPathFile = 4;

				do { //make sure the path is clear
					clearPathPos += direction;
					clearPathFile = CoordinateService.PositionToFile(clearPathPos);
				} while (isValidMove(squares, clearPathPos, pieceColor) && clearPathFile > 0 && clearPathFile < 8);

				if (
					(pieceColor == Color.White && (positionShim == -2 && clearPathPos == 0) || (positionShim == 2 && clearPathPos == 7))
					|| (pieceColor == Color.Black && (positionShim == -2 && clearPathPos == 56) || (positionShim == 2 && clearPathPos == 63))
				) {
					if (!squares.Intersects(clearPathPos)) {
						continue;
					}
					var edgePiece = squares.GetPiece(clearPathPos);
					if (edgePiece != null && edgePiece.PieceType == PieceType.Rook) {
						attacks.Add(new AttackedSquare(square, squares.GetSquare(tempPos)));
					}
				}
			}

			//this.removeKingChecksFromKingMoves(gameState, attacks, pieceColor, squares);
			return attacks;
		}

		private bool determineCheck(List<Square> squares, List<int> proposedAttacks, Color pieceColor) {
			//can't be more than one king
			//has to be at least two kings
			var king = squares.FindPiece(PieceType.King, pieceColor);
			return proposedAttacks.Contains(king.Index);
		}

		private IEnumerable<AttackedSquare> getKnightAttacks(GameState gameState, Square square) {
			var squares = gameState.Squares;
			var currentPosition = square.Index;
			var pieceColor = square.Piece.Color;
			var attacks = new List<Square>();
			var coord = CoordinateService.PositionToCoordinate(currentPosition);
			var file = CoordinateService.FileToInt(coord[0]);
			var rank = (int)coord[1];
			var potentialPositions = new List<int> { 6, 10, 15, 17, -6, -10, -15, -17 };
			foreach (var potentialPosition in potentialPositions) {
				var position = currentPosition + potentialPosition;
				var _isValidKnightMove = isValidKnightMove(currentPosition, position, file, rank);
				var _isValidMove = isValidMove(squares, position, pieceColor);
				var _isValidCoordinate = CoordinateService.IsValidCoordinate(position);

				if (!_isValidKnightMove || !_isValidMove || !_isValidCoordinate) { continue; }

				var attackedSquare = squares.GetSquare(position);
				if (!attackedSquare.Occupied) {
					attacks.Add(attackedSquare);
					//shouldn't really have to do this logic
					//} else if (attackedSquare.Piece.Color != pieceColor) {
					//because that should already be taken care of
				} else if (attackedSquare.Piece.Color != pieceColor) {
					attacks.Add(attackedSquare);
				}
			}
			return attacks.Select(a => new AttackedSquare(square, a));
		}

		private IEnumerable<Square> getOccupiedSquaresOfOneColor(Color color, List<Square> squares, bool ignoreKing = false) {
			if (ignoreKing) {
				return squares.Where(a => a.Piece != null && a.Piece.Color == color && a.Piece.PieceType != PieceType.King);
			} else {
				return squares.Where(a => a.Piece != null && a.Piece.Color == color); ;
			}
		}

		private IEnumerable<AttackedSquare> getPawnAttacks(GameState gameState, Square square) {
			var squares = gameState.Squares;
			var position = square.Index;
			var pieceColor = square.Piece.Color;
			var attacks = new List<Square>();
			var coord = CoordinateService.PositionToCoordinate(position);
			int file = CoordinateService.FileToInt(coord[0]);
			int rank = CoordinateService.PositionToRankInt(position);

			var directionIndicator = pieceColor == Color.White ? 1 : -1;
			var rankIndicator = pieceColor == Color.White ? 2 : 7;

			var nextRank = (rank + directionIndicator);
			var newPosition = CoordinateService.CoordinatePairToPosition(file, nextRank);
			var attackedSquare = squares.GetSquare(newPosition);
			attacks.Add(attackedSquare);

			if (file - 1 >= 0) {
				//get attack square on left
				var leftPos = CoordinateService.CoordinatePairToPosition(file - 1, nextRank);
				if (isValidPawnAttack(squares, leftPos, pieceColor)) {
					attacks.Add(squares.GetSquare(leftPos));
				}
			}
			if (file + 1 <= 7) {
				//get attack square on right
				var rightPos = CoordinateService.CoordinatePairToPosition(file + 1, nextRank);
				if (isValidPawnAttack(squares, rightPos, pieceColor)) {
					attacks.Add(squares.GetSquare(rightPos));
				}
			}
			//have to plus one here because rank is zero based and coordinate is base 1
			if ((rank + 1) == rankIndicator) {
				var rankUpPosition = CoordinateService.CoordinatePairToPosition(file, nextRank + directionIndicator);
				attacks.Add(squares.GetSquare(rankUpPosition));
			}

			//add en passant position
			if (gameState.EnPassantTargetPosition > -1) {
				var leftPos = CoordinateService.CoordinatePairToPosition(file - 1, nextRank);
				var rightPos = CoordinateService.CoordinatePairToPosition(file + 1, nextRank);
				if (gameState.EnPassantTargetPosition == leftPos || gameState.EnPassantTargetPosition == rightPos) {
					attacks.Add(squares.GetSquare(gameState.EnPassantTargetPosition));
				}
			}

			return attacks.Select(a => new AttackedSquare(square, a));
		}

		private IEnumerable<AttackedSquare> getPieceAttacks(GameState gameState, Square square, bool ignoreKing = false) {
			if (!square.Occupied) {
				return new List<AttackedSquare>();
			}

			switch (square.Piece.PieceType) {
				case PieceType.Pawn:
					return getPawnAttacks(gameState, square);

				case PieceType.Knight:
					return getKnightAttacks(gameState, square);

				case PieceType.Bishop:
					return this.DiagonalService.GetDiagonals(gameState, square, ignoreKing).Select(a => new AttackedSquare(square, a));

				case PieceType.Rook:
					return this.OrthogonalService.GetOrthogonals(gameState, square, ignoreKing).Select(a => new AttackedSquare(square, a));

				case PieceType.Queen:
					var attacks =
							this.OrthogonalService.GetOrthogonals(gameState, square, ignoreKing)
							.Concat(
								this.DiagonalService.GetDiagonals(gameState, square, ignoreKing)
							);
					return attacks.Select(a => new AttackedSquare(square, a));

				case PieceType.King:
					if (ignoreKing) {
						return new List<AttackedSquare>();
					}
					return GetKingAttacks(gameState, square);

				default:
					throw new Exception("Mismatched Enum!");
			}
		}

		private bool isValidKnightMove(int position, int tempPosition, int file, int rank) {
			var tempCoord = CoordinateService.PositionToCoordinate(tempPosition);
			var tempFile = CoordinateService.FileToInt(tempCoord[0]);
			var tempRank = (int)tempCoord[1];

			var fileDiff = Math.Abs(tempFile - file);
			var rankDiff = Math.Abs(tempRank - rank);

			if (fileDiff > 2 || fileDiff < 1) {
				return false;
			}
			if (rankDiff > 2 || rankDiff < 1) {
				return false;
			}

			return true;
		}

		private bool isValidMove(List<Square> squares, int position, Color pieceColor) {
			var isValidCoordinate = CoordinateService.IsValidCoordinate(position);
			if (!isValidCoordinate) {
				return false;
			}
			if (!squares.Intersects(position)) {
				return false;
			}
			var square = squares.GetSquare(position);
			if (!square.Occupied) {
				return true;
			}
			var blockingPiece = square.Piece;
			if (GeneralUtility.CanAttackPiece(pieceColor, blockingPiece)) {
				return true;
			} else {
				return false;
			}
		}

		private bool isValidPawnAttack(List<Square> matrix, int position, Color pieceColor) {
			if (CoordinateService.IsValidCoordinate(position)) {
				return true;
				//if (matrix.Select(a => a.Key).Contains(position)) {
				//var blockingPiece = matrix.Where(a => a.Key == position).First();
				//if (CoordinateService.CanAttackPiece(pieceColor, blockingPiece.Value)) {
				//	return true;
				//}
				//}
			}
			return false;
		}

		private void removeKingChecksFromKingMoves(GameState gameState, List<AttackedSquare> kingAttacks, Color color, List<Square> squares) {
			var oppositePieceColor = color.Reverse();
			//var allAttacks = GetAttacks(oppositePieceColor, fen, true).Where(a => a.Square.Occupied && a.Square.Piece.PieceType == PieceType.King);
			var allAttacksExceptKing = GetAttacks(gameState, true);
			var conflictingAttacks = from a in allAttacksExceptKing
									 join k in kingAttacks on a.Index equals k.Index
									 select a;

			foreach (var conflictingAttack in conflictingAttacks) {
				var attackedSquares = allAttacksExceptKing.Where(a => a.Index == conflictingAttack.Index);
				if (attackedSquares != null && attackedSquares.Any() && attackedSquares.Count() < 2) {
					//if there are more than two square attacking here, then it's not possible that we'd need to keep the attack
					var attackedSquare = attackedSquares.First();
					if (!attackedSquare.Occupied) {
						throw new Exception("This is the square that is supposed to have a king on it, why is it empty?");
					}
					var piece = attackedSquare.Piece;
					//this code is here to remove the possibility that the king is said to be in check by
					//an enemy pawn when he is directly in front of the pawn
					if (piece.PieceType == PieceType.Pawn) {
						var directionIndicator = color == Color.White ? -1 : 1; //make this backwards of normal
						var onSameFile = attackedSquare.Index + (directionIndicator * 8) == conflictingAttack.Index ? true : false;
						if (!onSameFile) {
							kingAttacks.Remove(conflictingAttack);
						}
					} else {
						kingAttacks.Remove(conflictingAttack);
					}
				} else {
					kingAttacks.Remove(conflictingAttack);
				}
			}
		}
	}
}