﻿using chess.v4.engine.enumeration;
using chess.v4.engine.extensions;
using chess.v4.engine.interfaces;
using chess.v4.engine.model;
using chess.v4.engine.utility;
using common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace chess.v4.engine.service {

	public class MoveService : IMoveService {
		public IAttackService AttackService { get; }
		public ICoordinateService CoordinateService { get; }

		public MoveService(ICoordinateService coordinateService, IAttackService attackService) {
			CoordinateService = coordinateService;
			AttackService = attackService;
		}

		public ResultOuput<bool> IsValidCastleAttempt(GameState gameState, Square square, int destination, IEnumerable<AttackedSquare> attackedSquares) {
			var piece = square.Piece;

			var isCastleAttempt =
				(square.Index == 4 || square.Index == 60)
				&& piece.PieceType == PieceType.King
				&& Math.Abs(square.Index - destination) == 2;

			//if not a castle, then no validation needed.
			if (!isCastleAttempt) {
				return ResultOuput<bool>.Ok(isCastleAttempt);
			}

			var castlingAvailability = checkCastleAvailability(gameState, destination, piece);
			if (!castlingAvailability) {
				return ResultOuput<bool>.Error("Castling is not available.");
			}

			//validate the move
			if (gameState.MoveInfo.IsCheck) {
				return ResultOuput<bool>.Error("Can't castle out of check.");
			}

			var kingTravelSquareIndexes = getKingCastleCoordinates(square, destination);
			var reverseColor = gameState.ActiveColor.Reverse();
			var enemyAttacks = from a in attackedSquares
							   join k in kingTravelSquareIndexes on a.Index equals k
							   where a.AttackerSquare.Piece.Color == reverseColor
							   select a;

			if (enemyAttacks.Any()) {
				return ResultOuput<bool>.Error("Can't castle through check.");
			}

			return ResultOuput<bool>.Ok(isCastleAttempt);
		}

		public bool IsCheckmate(GameState gameState, Square enemyKingPosition, IEnumerable<AttackedSquare> whiteAttacks, IEnumerable<AttackedSquare> blackAttacks) {
			var activeColor = gameState.ActiveColor;
			var squares = gameState.Squares;
			var checkedColor = gameState.ActiveColor == Color.White ? Color.White : Color.Black; //trust me this is right
			var kingIsBeingAttacked = whiteAttacks.Any(a => a.Index == enemyKingPosition.Index) || blackAttacks.Any(a => a.Index == enemyKingPosition.Index);
			if (!kingIsBeingAttacked) {
				return false;
			}
			//make sure that he cannot move
			var kingHasEscape = false;

			var friendlyAttacks = (activeColor == Color.White ? whiteAttacks : blackAttacks);
			var opponentAttacks = (activeColor == Color.White ? blackAttacks : whiteAttacks);

			//fix enemyKingAttacks. trying to figure out the moves that the king can make
			var enemyKingAttacks = squares;

			var remainingKingAttacks = enemyKingAttacks.Except(opponentAttacks);
			if (remainingKingAttacks.Any()) {
				kingHasEscape = true;
			}
			if (kingHasEscape) {
				return false;
			}
			//make sure that interposition is not possible
			var attackers = opponentAttacks.Where(a => a.Index == enemyKingPosition.Index);
			//if there are no attackers there cannot be a single interposition that saves the king
			if (attackers == null || !attackers.Any() || attackers.Count() > 1) {
				return true;
			}
			var attacker = attackers.FirstOrDefault();
			var theAttack = this.AttackService.GetKingAttack(attacker, gameState, enemyKingPosition);
			var interposers = friendlyAttacks.ToList().Intersect(theAttack);
			if (interposers.Any()) {
				return false;
			}
			//there were no friendlies to save the king, checkmate is true
			return true;
		}

		public bool IsDiagonalMove(int startPosition, int endPosition) {
			var startMod = startPosition % 8;
			var endMod = endPosition % 8;
			var modDiff = Math.Abs(startMod - endMod);

			var startRow = this.CoordinateService.PositionToRankInt(startPosition);
			var endRow = this.CoordinateService.PositionToRankInt(endPosition);
			var rowDiff = Math.Abs(startRow - endRow);
			if (modDiff == rowDiff) {
				return true;
			}
			return false;
		}

		public bool IsEnPassant(char piece, int piecePosition, int newPiecePosition, string enPassantTargetSquare) {
			if (char.ToUpper(piece) != 'P') { return false; } //only pawns can perform en passant
			var enPassantPosition = CoordinateService.CoordinateToPosition(enPassantTargetSquare);
			if (enPassantPosition != newPiecePosition) { return false; } //if we're not moving to the en passant position, this is not en passant
			var moveDistance = Math.Abs(piecePosition - newPiecePosition);
			if (!new List<int> { 7, 9 }.Contains(moveDistance)) { return false; } //is this a diagonal move?
			if (char.IsLower(piece) && piecePosition < newPiecePosition) { return false; } //black can't move up
			if (char.IsUpper(piece) && piecePosition > newPiecePosition) { return false; } //black can't move down
			return true;
		}

		public bool IsRealCheck(List<Square> squares, IEnumerable<AttackedSquare> attacksThatCheck, Color activeColor, int kingSquare) {
			if (attacksThatCheck == null || !attacksThatCheck.Any()) {
				return false;
			}
			if (attacksThatCheck.Count() > 1) {
				//if there are more than one, then this is real
				return true;
			}
			var key = attacksThatCheck.First().Index;
			var attackingPiece = squares.GetPiece(key);
			//this code is here to remove the possibility that the king is said to be in check by an enemy pawn when he is directly in front of the pawn
			if (attackingPiece.PieceType == PieceType.Pawn) {
				var onSameFile = (key % 8) == (kingSquare % 8) ? true : false;
				return !onSameFile;
			}
			return true;
		}

		public bool IsValidPawnMove(Square currentSquare, List<Square> squares, Color color, int piecePosition, int newPiecePosition, bool isEnPassant) {
			var isDiagonalMove = this.IsDiagonalMove(currentSquare.Index, newPiecePosition);
			if (!isDiagonalMove) {
				return true;
			}
			var pieceToCapture = squares.GetSquare(newPiecePosition).Piece;
			var isCapture = pieceToCapture != null;
			return isCapture || isEnPassant;
		}

		private static bool checkCastleAvailability(GameState gameState, int destination, Piece piece) {
			if (gameState.CastlingAvailability == "-") {
				return false;
			}
			var castlingAvailability = true;
			switch (piece.Color) {
				case Color.Black:
					switch (destination) {
						case 58:
							castlingAvailability = gameState.CastlingAvailability.Contains("q", StringComparison.CurrentCulture);
							break;

						case 62:
							castlingAvailability = gameState.CastlingAvailability.Contains("k", StringComparison.CurrentCulture);
							break;

						default:
							throw new Exception("Invalid destination.");
					}
					break;

				case Color.White:
					switch (destination) {
						case 2:
							castlingAvailability = gameState.CastlingAvailability.Contains("Q", StringComparison.CurrentCulture);
							break;

						case 6:
							castlingAvailability = gameState.CastlingAvailability.Contains("K", StringComparison.CurrentCulture);
							break;

						default:
							throw new Exception("Invalid destination.");
					}
					break;

				default:
					throw new Exception("Enum not matched.");
			}

			return castlingAvailability;
		}

		private static int[] getKingCastleCoordinates(Square kingSquare, int destination) {
			switch (kingSquare.Piece.Color) {
				case Color.Black:
					switch (destination) {
						case 58:
							return new int[2] { 61, 62 };

						case 62:
							return new int[2] { 58, 59 };

						default:
							throw new Exception("Invalid destination.");
					}

				case Color.White:
					switch (destination) {
						case 2:
							return new int[2] { 2, 3 };

						case 6:
							return new int[2] { 5, 6 };

						default:
							throw new Exception("Invalid destination.");
					}

				default:
					throw new Exception("Enum not matched.");
			}
		}

		private bool determineCastleThroughCheck(GameState gameState, List<Square> enemyAttacks, int kingPos, int rookPos) {
			var oppositeColor = gameState.ActiveColor.Reverse();
			//var enemyAttacks = this.PieceService.GetAttacks(oppositeColor, fen).SelectMany(a => a.Value);
			var positions = this.getKingPositionsDuringCastle(kingPos, rookPos);
			var arePositionsAttacked = positions.Intersect<int>(enemyAttacks.Select(a => a.Index)).Any();
			return arePositionsAttacked;
		}

		private int[] getKingPositionsDuringCastle(int kingPos, int rookPos) {
			int direction = kingPos < rookPos ? 1 : -1;
			int[] result = new int[2];
			for (int i = 0; i < 2; i++) {
				result[i] = kingPos + (direction * (i + 1));
			}
			return result;
		}
	}
}