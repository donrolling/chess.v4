using chess.v4.engine.enumeration;
using chess.v4.engine.model;
using System;
using System.Collections.Generic;
using System.Text;

namespace chess.v4.engine.service {
	public class AttackService {
		public static List<int> GetAttacks(Square square) {
			if (!square.Occupied) {
				return new List<int>();
			}
			switch (square.Piece.PieceType) {
				case enumeration.PieceType.Invalid:
					return new List<int>();
				case enumeration.PieceType.Pawn:
					break;
				case enumeration.PieceType.Knight:
					break;
				case enumeration.PieceType.Bishop:
					break;
				case enumeration.PieceType.Rook:
					break;
				case enumeration.PieceType.Queen:
					break;
				case enumeration.PieceType.King:
					break;
				default:
					break;
			}
			return new List<int>();
		}

		public static List<int> GetKingAttacks(string fen, int position, Color pieceColor, string castleAvailability) {
			List<int> attacks = new List<int>();
			var matrix = NotationService.CreateMatrixFromFEN(fen);

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
				if (CoordinateService.IsValidCoordinate(tempPos)) {
					if (isValidMove(matrix, tempPos, pieceColor)) {
						var isCastle = Math.Abs(positionShim) == 2; //are we trying to move two squares? if so, this is a castle attempt
						if (!isCastle) {
							attacks.Add(tempPos);
						} else {
							var direction = positionShim > 0 ? 1 : -1;
							int clearPathPos = tempPos;
							int clearPathFile = 4;
							do { //make sure the path is clear
								clearPathPos += direction;
								clearPathFile = CoordinateService.PositionToFile(clearPathPos);
							} while (isValidMove(matrix, clearPathPos, pieceColor) && clearPathFile > 0 && clearPathFile < 8);
							if (
								(pieceColor == Color.White && (positionShim == -2 && clearPathPos == 0) || (positionShim == 2 && clearPathPos == 7))
								|| (pieceColor == Color.Black && (positionShim == -2 && clearPathPos == 56) || (positionShim == 2 && clearPathPos == 63))
							) {
								if (matrix.ContainsKey(clearPathPos)) {
									var edgePiece = matrix[clearPathPos];
									if (
										(pieceColor == Color.White && edgePiece == 'R')
										|| (pieceColor == Color.Black && edgePiece == 'r')
									) {
										attacks.Add(tempPos);
									}
								}
							}
						}
					}
				}
			}

			attacks = removeKingChecksFromKingMoves(fen, attacks, pieceColor, matrix);

			return attacks;
		}
		public static List<int> GetQueenAttacks(Dictionary<int, char> matrix, int position, Color pieceColor, bool ignoreKing) {
			List<int> attacks = new List<int>();
			attacks.AddRange(CoordinateService.GetOrthogonals(matrix, position, pieceColor, ignoreKing));
			attacks.AddRange(CoordinateService.GetDiagonals(matrix, position, pieceColor, ignoreKing));
			return attacks;
		}
		public static List<int> GetRookAttacks(Dictionary<int, char> matrix, int position, Color pieceColor, bool ignoreKing) {
			List<int> attacks = new List<int>();
			attacks.AddRange(CoordinateService.GetOrthogonals(matrix, position, pieceColor, ignoreKing));
			return attacks;
		}
		public static List<int> GetBishopAttacks(Dictionary<int, char> matrix, int position, Color pieceColor, bool ignoreKing) {
			List<int> attacks = new List<int>();
			attacks.AddRange(CoordinateService.GetDiagonals(matrix, position, pieceColor, ignoreKing));
			return attacks;
		}
		public static List<int> GetKnightAttacks(Dictionary<int, char> matrix, int position, Color pieceColor) {
			List<int> attacks = new List<int>();

			var coord = CoordinateService.PositionToCoordinate(position);
			var file = CoordinateService.FileToInt(coord[0]);
			var rank = (int)coord[1];

			var positions = new List<int> { 6, 10, 15, 17, -6, -10, -15, -17 };
			foreach (var pos in positions) {
				var tempPosition = position + pos;
				bool isValid = isValidKnightMove(position, tempPosition, file, rank);
				bool isGoodMove = isValidMove(matrix, tempPosition, pieceColor);
				if (isValid && isGoodMove) {
					attacks.Add(tempPosition);
				}
			}

			attacks = removeInvalidPieces(matrix, attacks, pieceColor);
			return attacks;
		}
		public static List<int> GetPawnAttacks(Dictionary<int, char> matrix, int position, Color pieceColor, int enPassantPosition) {
			List<int> attacks = new List<int>();
			var coord = CoordinateService.PositionToCoordinate(position);
			int file = CoordinateService.FileToInt(coord[0]);
			int rank = CoordinateService.PositionToRankInt(position);

			var directionIndicator = pieceColor == Color.White ? 1 : -1;
			var rankIndicator = pieceColor == Color.White ? 2 : 7;

			var nextRank = (rank + directionIndicator);
			attacks.Add(CoordinateService.CoordinatePairToPosition(file, nextRank));

			if (file - 1 >= 0) {
				//get attack square on left
				var leftPos = CoordinateService.CoordinatePairToPosition(file - 1, nextRank);
				if (isValidPawnAttack(matrix, leftPos, pieceColor)) {
					attacks.Add(leftPos);
				}
			}
			if (file + 1 <= 7) {
				//get attack square on right
				var rightPos = CoordinateService.CoordinatePairToPosition(file + 1, nextRank);
				if (isValidPawnAttack(matrix, rightPos, pieceColor)) {
					attacks.Add(rightPos);
				}
			}
			//have to plus one here because rank is zero based and coordinate is base 1
			if ((rank + 1) == rankIndicator) {
				attacks.Add(CoordinateService.CoordinatePairToPosition(file, nextRank + directionIndicator));
			}

			//add en passant position
			if (enPassantPosition > -1) {
				var leftPos = CoordinateService.CoordinatePairToPosition(file - 1, nextRank);
				var rightPos = CoordinateService.CoordinatePairToPosition(file + 1, nextRank);
				if (enPassantPosition == leftPos || enPassantPosition == rightPos) {
					attacks.Add(enPassantPosition);
				}
			}

			return attacks;
		}
	}
}
