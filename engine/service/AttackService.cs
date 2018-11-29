using chess.v4.engine.extensions;
using chess.v4.engine.interfaces;
using chess.v4.engine.utility;
using chess.v4.engine.Utility;
using chess.v4.models;
using chess.v4.models.enumeration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace chess.v4.engine.service {
	public class AttackService : IAttackService {
		public INotationService NotationService { get; }
		public IOrthogonalService OrthogonalService { get; }

		public AttackService(INotationService notationService, IOrthogonalService orthogonalService) {
			NotationService = notationService;
			OrthogonalService = orthogonalService;
		}

		public IEnumerable<AttackedSquare> GetAttacks(GameState gameState, bool ignoreKing = false) {
			var accumulator = new List<AttackedSquare>();
			foreach (var square in gameState.Squares.Where(a => a.Occupied).OrderBy(a => a.Piece.OrderOfOperation)) {
				this.getPieceAttacks(gameState, square, accumulator, ignoreKing);
			}
			trimKingMoves(accumulator);
			return accumulator;
		}

		public void GetKingAttacks(GameState gameState, Square square, List<AttackedSquare> accumulator) {
			var attacks = new List<AttackedSquare>();
			var squares = gameState.Squares;
			var offsets = getKingOffsets(square.Index);
			var pieceColor = square.Piece.Color;
			foreach (var offset in offsets) {
				var tempPos = square.Index + offset;
				var isValidCoordinate = GeneralUtility.IsValidCoordinate(tempPos);
				if (!isValidCoordinate) {
					continue;
				}
				var _isValidMove = isValidMove(squares, tempPos, pieceColor);
				if (!_isValidMove.IsValid) {
					continue;
				}
				attacks.Add(
					new AttackedSquare(square, squares.GetSquare(tempPos), isProtecting: !_isValidMove.CanAttackOccupyingPiece)
				);
			}

			//*********************
			//castling stuff
			//*********************
			var castleAvailability = gameState.CastlingAvailability;
			if (pieceColor == Color.White) {
				if (castleAvailability.Contains("K")) {
					addCastleAttackIfPossible(
						square,
						attacks,
						squares,
						7,
						new List<int> { 5, 6 },
						6
					);
				}
				if (castleAvailability.Contains("Q")) {
					addCastleAttackIfPossible(
						square,
						attacks,
						squares,
						0,
						new List<int> { 1, 2, 3 },
						2
					);
				}
			} else if (pieceColor == Color.Black) {
				if (castleAvailability.Contains("k")) {
					addCastleAttackIfPossible(
						square,
						attacks,
						squares,
						63,
						new List<int> { 61, 62 },
						62
					);
				}
				if (castleAvailability.Contains("q")) {
					addCastleAttackIfPossible(
						square,
						attacks,
						squares,
						56,
						new List<int> { 57, 58, 59 },
						58
					);
				}
			}
			//*********************
			//end castling stuff
			//*********************

			if (!attacks.Any()) {
				return;
			}
			accumulator.AddRange(attacks);
		}

		private static void addCastleAttackIfPossible(Square square, List<AttackedSquare> attacks, List<Square> squares, int rookPos, List<int> castlingClearPositions, int attackToAdd) {
			var areSquaresClear = squares.Count(
									a => castlingClearPositions.Contains(a.Index)
									&& !a.Occupied
								) == castlingClearPositions.Count();
			if (areSquaresClear) {
				var rookSquare = squares.GetSquare(rookPos);
				if (rookSquare.Occupied && rookSquare.Piece.PieceType == PieceType.Rook) {
					attacks.Add(new AttackedSquare(square, squares.GetSquare(attackToAdd)));
				}
			}
		}

		private static List<int> getKingOffsets(int position) {
			var offsets = new List<int> { -9, -8, -7, -1, 1, 7, 8, 9 };
			if (position % 8 == 0) {
				offsets.Remove(-1);
				offsets.Remove(-9);
				offsets.Remove(7);
			}
			if (position % 8 == 7) {
				offsets.Remove(1);
				offsets.Remove(9);
				offsets.Remove(-7);
			}

			return offsets;
		}

		private static void trimKingMoves(List<AttackedSquare> accumulator) {
			//trim king moves now that all moves have been calculated.
			var removeableAttacks = new List<AttackedSquare>();
			foreach (var color in new List<Color> { Color.Black, Color.White }) {
				var kingAttacks = accumulator
									.Where(a =>
										a.AttackingSquare.Piece.PieceType == PieceType.King
										&& a.AttackingSquare.Piece.Color == color
									);
				var conflictingAttacks = from a in accumulator
										 join k in kingAttacks on a.Index equals k.Index
										 where
											a.AttackingSquare.Piece.Color == color.Reverse()
											&& !a.IsPassiveAttack
										 select a;
				if (conflictingAttacks.Any()) {
					var conflictingIndexes = conflictingAttacks.Select(a => a.Index);
					var rs = kingAttacks.Where(a => conflictingIndexes.Contains(a.Index));
					removeableAttacks.AddRange(rs);
				}
			}
			foreach (var removeableAttack in removeableAttacks) {
				accumulator.Remove(removeableAttack);
			}
		}

		private bool determineCheck(List<Square> squares, List<int> proposedAttacks, Color pieceColor) {
			//can't be more than one king
			//has to be at least two kings
			var king = squares.FindPiece(PieceType.King, pieceColor);
			return proposedAttacks.Contains(king.Index);
		}

		private void getKnightAttacks(GameState gameState, Square square, List<AttackedSquare> accumulator) {
			var squares = gameState.Squares;
			var currentPosition = square.Index;
			var pieceColor = square.Piece.Color;
			var attacks = new List<AttackedSquare>();
			var coord = NotationUtility.PositionToCoordinate(currentPosition);
			var file = NotationUtility.FileToInt(coord[0]);
			var rank = (int)coord[1];
			var potentialPositions = new List<int> { 6, 10, 15, 17, -6, -10, -15, -17 };
			foreach (var potentialPosition in potentialPositions) {
				var position = currentPosition + potentialPosition;
				var _isValidKnightMove = isValidKnightMove(currentPosition, position, file, rank);
				var _isValidMove = isValidMove(squares, position, pieceColor);
				var _isValidCoordinate = GeneralUtility.IsValidCoordinate(position);

				if (!_isValidKnightMove || !_isValidMove.IsValid || !_isValidCoordinate) { continue; }

				var attackedSquare = squares.GetSquare(position);
				if (!attackedSquare.Occupied) {
					attacks.Add(new AttackedSquare(square, attackedSquare));
				} else {
					attacks.Add(new AttackedSquare(square, attackedSquare, isProtecting: attackedSquare.Piece.Color == pieceColor));
				}
			}
			if (attacks.Any()) {
				accumulator.AddRange(attacks);
			}
		}

		private IEnumerable<Square> getOccupiedSquaresOfOneColor(Color color, List<Square> squares, bool ignoreKing = false) {
			if (ignoreKing) {
				return squares.Where(a => a.Piece != null && a.Piece.Color == color && a.Piece.PieceType != PieceType.King);
			} else {
				return squares.Where(a => a.Piece != null && a.Piece.Color == color); ;
			}
		}

		private void getPawnAttacks(GameState gameState, Square square, List<AttackedSquare> accumulator) {
			var squares = gameState.Squares;
			var position = square.Index;
			var pieceColor = square.Piece.Color;
			var coord = NotationUtility.PositionToCoordinate(position);
			int file = NotationUtility.FileToInt(coord[0]);
			int rank = NotationUtility.PositionToRankInt(position);

			var directionIndicator = pieceColor == Color.White ? 1 : -1;
			var homeRankIndicator = pieceColor == Color.White ? 2 : 7;

			var nextRank = (rank + directionIndicator);
			var aheadOneRankPosition = NotationUtility.CoordinatePairToPosition(file, nextRank);
			var aheadOneRankSquare = squares.GetSquare(aheadOneRankPosition);
			var attacks = new List<AttackedSquare>();
			if (!aheadOneRankSquare.Occupied) {
				//can't attack going forward
				attacks.Add(new AttackedSquare(square, aheadOneRankSquare, true));
			}

			managePawnAttacks(squares, square, pieceColor, file, rank, directionIndicator, homeRankIndicator, nextRank, attacks, aheadOneRankSquare.Occupied);

			//add en passant position: -1 indicates null here
			if (gameState.EnPassantTargetPosition > -1) {
				var leftPos = NotationUtility.CoordinatePairToPosition(file - 1, nextRank);
				var rightPos = NotationUtility.CoordinatePairToPosition(file + 1, nextRank);
				if (gameState.EnPassantTargetPosition == leftPos || gameState.EnPassantTargetPosition == rightPos) {
					var enPassantSquare = squares.GetSquare(gameState.EnPassantTargetPosition);
					attacks.Add(new AttackedSquare(square, enPassantSquare));
				}
			}
			if (attacks.Any()) {
				accumulator.AddRange(attacks);
			}
		}

		private void getPawnDiagonalAttack(List<Square> squares, Square square, Color pieceColor, int fileIndicator, int nextRank, List<AttackedSquare> attacks) {
			var pos = NotationUtility.CoordinatePairToPosition(fileIndicator, nextRank);
			var attackedSquare = squares.GetSquare(pos);
			if (attackedSquare.Occupied && attackedSquare.Piece.Color != pieceColor) {
				attacks.Add(new AttackedSquare(square, attackedSquare, false, true));
			} else {
				attacks.Add(new AttackedSquare(square, attackedSquare, false, true, true));
			}
		}

		private void getPieceAttacks(GameState gameState, Square square, List<AttackedSquare> accumulator, bool ignoreKing = false) {
			switch (square.Piece.PieceType) {
				case PieceType.Pawn:
					getPawnAttacks(gameState, square, accumulator);
					break;

				case PieceType.Knight:
					getKnightAttacks(gameState, square, accumulator);
					break;

				case PieceType.Bishop:
					DiagonalUtility.GetDiagonals(gameState, square, accumulator, ignoreKing);
					break;

				case PieceType.Rook:
					this.OrthogonalService.GetOrthogonals(gameState, square, accumulator, ignoreKing);
					break;

				case PieceType.Queen:
					this.OrthogonalService.GetOrthogonals(gameState, square, accumulator, ignoreKing);
					DiagonalUtility.GetDiagonals(gameState, square, accumulator, ignoreKing);
					break;

				case PieceType.King:
					if (ignoreKing) {
						return;
					}
					GetKingAttacks(gameState, square, accumulator);
					break;

				default:
					throw new Exception("Mismatched Enum!");
			}
		}

		private bool isValidKnightMove(int position, int tempPosition, int file, int rank) {
			var isValidCoordinate = GeneralUtility.IsValidCoordinate(tempPosition);
			if (!isValidCoordinate) {
				return false;
			}

			var tempCoord = NotationUtility.PositionToCoordinate(tempPosition);
			var tempFile = NotationUtility.FileToInt(tempCoord[0]);
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

		private (bool IsValid, bool CanAttackOccupyingPiece) isValidMove(List<Square> squares, int position, Color pieceColor) {
			var isValidCoordinate = GeneralUtility.IsValidCoordinate(position);
			if (!isValidCoordinate) {
				return (false, false);
			}
			if (!squares.Any(a => a.Index == position)) {
				return (false, false);
			}
			var square = squares.GetSquare(position);
			if (!square.Occupied) {
				return (true, true);
			}
			var blockingPiece = square.Piece;
			if (GeneralUtility.IsTeamPiece(pieceColor, blockingPiece)) {
				return (true, true);
			} else {
				return (true, false);
			}
		}

		private void managePawnAttacks(List<Square> squares, Square square, Color pieceColor, int file, int rank, int directionIndicator, int homeRankIndicator, int nextRank, List<AttackedSquare> attacks, bool aheadOneRankSquareOccupied) {
			var notOnFarLeftFile = file - 1 >= 0;
			var notOnFarRightFile = file + 1 <= 7;
			if (notOnFarLeftFile) {
				//get attack square on left
				var fileIndicator = file - 1;
				getPawnDiagonalAttack(squares, square, pieceColor, fileIndicator, nextRank, attacks);
			}
			if (notOnFarRightFile) {
				//get attack square on right
				var fileIndicator = file + 1;
				getPawnDiagonalAttack(squares, square, pieceColor, fileIndicator, nextRank, attacks);
			}
			//can't move ahead two if the one in front of you is blocked.
			if (aheadOneRankSquareOccupied) {
				return;
			}
			//have to plus one here because rank is zero based and coordinate is base 1
			//if this pawn is on it's home rank, then add a second attack square.
			var isOnHomeRank = rank + 1 == homeRankIndicator;
			if (isOnHomeRank) {
				var forwardOne = nextRank + directionIndicator;
				var rankForwardPosition = NotationUtility.CoordinatePairToPosition(file, forwardOne);
				var rankForwardSquare = squares.GetSquare(rankForwardPosition);
				//pawns don't attack forward, so we don't have attacks when people occupy ahead of us
				if (!rankForwardSquare.Occupied) {
					attacks.Add(new AttackedSquare(square, rankForwardSquare, true));
				}
			}
		}
	}
}