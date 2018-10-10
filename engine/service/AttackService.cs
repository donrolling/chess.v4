using chess.v4.engine.enumeration;
using chess.v4.engine.extensions;
using chess.v4.engine.interfaces;
using chess.v4.engine.model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace chess.v4.engine.service {

	public class AttackService : IAttackService {
		public INotationService NotationService { get; }
		public ICoordinateService CoordinateService { get; }

		public AttackService(INotationService notationService, ICoordinateService coordinateService) {
			NotationService = notationService;
			CoordinateService = coordinateService;
		}

		public (PieceType, Color) GetChessTypeFromChar(char piece) {
			var pieceType = CoordinateService.GetPieceTypeFromChar(piece);
			var pieceColor = CoordinateService.GetColorFromChar(piece);
			return (pieceType, pieceColor);
		}

		public IEnumerable<Square> GetPieceAttacks(string fen, Square square, bool ignoreKing = false) {
			var position = square.Index;
			var piece = square.Piece;
			var squares = NotationService.CreateMatrixFromFEN(fen);
			switch (piece.PieceType) {
				case PieceType.Pawn:
					var enPassantPosition = CoordinateService.CoordinateToPosition(fen.Split(' ')[3]);
					return GetPawnAttacks(squares, position, piece.Color, enPassantPosition);

				case PieceType.Knight:
					return GetKnightAttacks(squares, position, piece.Color);

				case PieceType.Bishop:
					return GetBishopAttacks(squares, position, piece.Color, ignoreKing);

				case PieceType.Rook:
					return GetRookAttacks(squares, position, piece.Color, ignoreKing);

				case PieceType.Queen:
					return GetQueenAttacks(squares, position, piece.Color, ignoreKing);

				case PieceType.King:
					var castleAvailability = fen.Split(' ')[2];
					return GetKingAttacks(fen, position, piece.Color, castleAvailability);

				case PieceType.Invalid:
				default:
					return new List<Square>();
			}
		}

		public IEnumerable<Square> GetKingAttacks(string fen, int position, Color pieceColor, string castleAvailability) {
			var attacks = new List<Square>();
			var squares = NotationService.CreateMatrixFromFEN(fen);

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
					var square = squares.GetSquare(tempPos);
					attacks.Add(square);
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
						var square = squares.GetSquare(tempPos);
						attacks.Add(square);;
					}
				}
			}

			this.removeKingChecksFromKingMoves(fen, attacks, pieceColor, squares);
			return attacks;
		}

		public IEnumerable<Square> GetQueenAttacks(List<Square> squares, int position, Color pieceColor, bool ignoreKing) {
			var attacks = new List<Square>();
			attacks.AddRange(CoordinateService.GetOrthogonals(squares, position, pieceColor, ignoreKing));
			attacks.AddRange(CoordinateService.GetDiagonals(squares, position, pieceColor, ignoreKing));
			return attacks;
		}

		public IEnumerable<Square> GetRookAttacks(List<Square> squares, int position, Color pieceColor, bool ignoreKing) {
			return CoordinateService.GetOrthogonals(squares, position, pieceColor, ignoreKing);
		}

		public IEnumerable<Square> GetBishopAttacks(List<Square> matrix, int position, Color pieceColor, bool ignoreKing) {
			return CoordinateService.GetDiagonals(matrix, position, pieceColor, ignoreKing);
		}

		public IEnumerable<Square> GetKnightAttacks(List<Square> squares, int currentPosition, Color pieceColor) {
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

				var square = squares.GetSquare(position);
				if (!square.Occupied) {
					attacks.Add(square);
				} else if (square.Piece.Color != pieceColor) {
					attacks.Add(square);
				}
			}
			return attacks;
		}

		public IEnumerable<Square> GetPawnAttacks(List<Square> squares, int position, Color pieceColor, int enPassantPosition) {
			var attacks = new List<Square>();
			var coord = CoordinateService.PositionToCoordinate(position);
			int file = CoordinateService.FileToInt(coord[0]);
			int rank = CoordinateService.PositionToRankInt(position);

			var directionIndicator = pieceColor == Color.White ? 1 : -1;
			var rankIndicator = pieceColor == Color.White ? 2 : 7;

			var nextRank = (rank + directionIndicator);
			var newPosition = CoordinateService.CoordinatePairToPosition(file, nextRank);
			var square = squares.GetSquare(newPosition);
			attacks.Add(square);

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
			if (enPassantPosition > -1) {
				var leftPos = CoordinateService.CoordinatePairToPosition(file - 1, nextRank);
				var rightPos = CoordinateService.CoordinatePairToPosition(file + 1, nextRank);
				if (enPassantPosition == leftPos || enPassantPosition == rightPos) {
					attacks.Add(squares.GetSquare(enPassantPosition));
				}
			}

			return attacks;
		}

		public IEnumerable<Square> GetAttacks(Color color, string fen, bool ignoreKing = false) {
			var allAttacks = new List<Square>();
			var squares = NotationService.CreateMatrixFromFEN(fen);
			var queriedPieces = getMatrixOfOneColor(color, squares, ignoreKing);
			foreach (var square in queriedPieces) {
				var list = this.GetPieceAttacks(fen, square, ignoreKing);
				allAttacks.AddRange(list);
			}
			return allAttacks;
		}

		private IEnumerable<Square> getMatrixOfOneColor(Color color, List<Square> squares, bool ignoreKing = false) {
			if (ignoreKing) {
				return squares.Where(a => a.Piece != null && a.Piece.Color == color && a.Piece.PieceType != PieceType.King);
			} else {
				return squares.Where(a => a.Piece != null && a.Piece.Color == color); ;
			}
		}

		private void removeKingChecksFromKingMoves(string fen, List<Square> kingAttacks, Color pieceColor, List<Square> squares) {
			var oppositePieceColor = CoordinateService.GetOppositeColor(pieceColor);
			var pieceAttacks = GetAttacks(oppositePieceColor, fen, true);

			var conflictingAttacks = from p in pieceAttacks
									 join k in kingAttacks on p.Index equals k.Index
									 select p;

			foreach (var conflictingAttack in conflictingAttacks) {
				bool removeAttack = true;
				var attackedSquares = pieceAttacks.Where(a => a.Index == conflictingAttack.Index);
				if (attackedSquares != null && attackedSquares.Any()) {
					if (attackedSquares.Count() == 1) {
						//if there are more, it's not possible that we'd need to keep the attack
						var square = attackedSquares.First();
						if (square.Occupied) {
							var piece = square.Piece;
							//this code is here to remove the possibility that the king is said to be in check by an enemy pawn when he is directly in front of the pawn
							if (piece.PieceType == PieceType.Pawn) {
								var directionIndicator = pieceColor == Color.White ? -1 : 1; //make this backwards of normal
								var onSameFile = square.Index + (directionIndicator * 8) == conflictingAttack.Index ? true : false;
								if (onSameFile) {
									removeAttack = false;
								}
							}
						}
					}
				}
				if (removeAttack) {
					kingAttacks.Remove(conflictingAttack);
				}
			}
		}

		private bool determineCheck(List<Square> squares, List<int> proposedAttacks, Color pieceColor) {
			//can't be more than one king
			//has to be at least two kings
			var king = CoordinateService.FindPiece(squares, PieceType.King, pieceColor).First();
			return proposedAttacks.Contains(king.Index);
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
			if (CoordinateService.CanAttackPiece(pieceColor, blockingPiece)) {
				return true;
			} else {
				return false;
			}
		}
	}
}