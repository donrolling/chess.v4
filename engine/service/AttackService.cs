﻿using chess.v4.engine.enumeration;
using chess.v4.engine.extensions;
using chess.v4.engine.interfaces;
using chess.v4.engine.model;
using chess.v4.engine.utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace chess.v4.engine.service {
	public class AttackService : IAttackService {
		public IDiagonalService DiagonalService { get; }
		public INotationService NotationService { get; }
		public IOrthogonalService OrthogonalService { get; }

		public AttackService(INotationService notationService, IOrthogonalService orthogonalService, IDiagonalService diagonalService) {
			NotationService = notationService;
			OrthogonalService = orthogonalService;
			DiagonalService = diagonalService;
		}

		public IEnumerable<AttackedSquare> GetAttacks(GameState gameState, bool ignoreKing = false) {
			var accumulator = new List<AttackedSquare>();
			foreach (var square in gameState.Squares.Where(a => a.Occupied).OrderBy(a => a.Piece.OrderOfOperation)) {
				this.getPieceAttacks(gameState, square, accumulator, ignoreKing);
			}
			return accumulator;
		}

		public void GetKingAttacks(GameState gameState, Square square, List<AttackedSquare> accumulator) {
			var attacks = new List<AttackedSquare>();
			var squares = gameState.Squares;
			var position = square.Index;
			var pieceColor = square.Piece.Color;
			var opponentPieceColor = pieceColor.Reverse();
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
				var isValidCoordinate = GeneralUtility.IsValidCoordinate(tempPos);
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
					clearPathFile = NotationUtility.PositionToFile(clearPathPos);
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

			if (attacks.Any()) {
				var conflictingAttacks = from a in accumulator
										 join k in attacks on a.Index equals k.Index
										 where
											a.AttackingSquare.Piece.Color == opponentPieceColor
											&& !a.IsPassiveAttack
										 select a;
				if (conflictingAttacks.Any()) {
					var nonCheckingAttacks = attacks.Select(a => a.Index).Except(conflictingAttacks.Select(a => a.Index));
					var trimmedAttacks = attacks
							.Where(a => nonCheckingAttacks.Contains(a.Index))
							.Select(a => new AttackedSquare(square, a));
					accumulator.AddRange(trimmedAttacks);
				} else {
					accumulator.AddRange(attacks.Select(a => new AttackedSquare(square, a)));
				}
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
			var attacks = new List<Square>();
			var coord = NotationUtility.PositionToCoordinate(currentPosition);
			var file = NotationUtility.FileToInt(coord[0]);
			var rank = (int)coord[1];
			var potentialPositions = new List<int> { 6, 10, 15, 17, -6, -10, -15, -17 };
			foreach (var potentialPosition in potentialPositions) {
				var position = currentPosition + potentialPosition;
				var _isValidKnightMove = isValidKnightMove(currentPosition, position, file, rank);
				var _isValidMove = isValidMove(squares, position, pieceColor);
				var _isValidCoordinate = GeneralUtility.IsValidCoordinate(position);

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
			if (attacks.Any()) {
				accumulator.AddRange(attacks.Select(a => new AttackedSquare(square, a)));
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

			managePawnAttacks(squares, square, pieceColor, file, rank, directionIndicator, homeRankIndicator, nextRank, attacks);

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
			var _isValidPawnAttack = GeneralUtility.CanAttackPiece(pieceColor, attackedSquare.Piece);
			if (_isValidPawnAttack) {
				var s1 = squares.GetSquare(pos);
				attacks.Add(new AttackedSquare(square, s1, false, true));
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
					this.DiagonalService.GetDiagonals(gameState, square, accumulator, ignoreKing);
					break;

				case PieceType.Rook:
					this.OrthogonalService.GetOrthogonals(gameState, square, accumulator, ignoreKing);
					break;

				case PieceType.Queen:
					this.OrthogonalService.GetOrthogonals(gameState, square, accumulator, ignoreKing);
					this.DiagonalService.GetDiagonals(gameState, square, accumulator, ignoreKing);
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

		private bool isValidMove(List<Square> squares, int position, Color pieceColor) {
			var isValidCoordinate = GeneralUtility.IsValidCoordinate(position);
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

		private void managePawnAttacks(List<Square> squares, Square square, Color pieceColor, int file, int rank, int directionIndicator, int homeRankIndicator, int nextRank, List<AttackedSquare> attacks) {
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