using chess.v4.engine.enumeration;
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

		public MoveService() {
		}

		public (bool IsValidCoordinate, bool BreakAfterAction, bool CanAttackPiece, Square SquareToAdd) DetermineMoveViability(GameState gameState, int newPosition, bool ignoreKing) {
			if (!GeneralUtility.IsValidCoordinate(newPosition)) {
				return (false, false, false, null);
			}
			var newSquare = gameState.Squares.GetSquare(newPosition);
			if (!newSquare.Occupied) {
				return (true, false, true, newSquare);
			}
			var blockingPiece = newSquare.Piece;
			var canAttackPiece = GeneralUtility.CanAttackPiece(gameState.ActiveColor, blockingPiece);
			if (!canAttackPiece) {
				return (true, true, false, null);
			}
			var breakAfterAction = GeneralUtility.BreakAfterAction(ignoreKing, blockingPiece, newSquare.Piece.Color);
			return (true, breakAfterAction, true, newSquare);
		}

		public Envelope<MoveInfo> GetMoveInfo(GameState gameState, int piecePosition, int newPiecePosition, IEnumerable<AttackedSquare> allAttacks) {
			var newActiveColor = gameState.ActiveColor.Reverse();
			gameState.MoveInfo = new MoveInfo();
			var oldSquare = gameState.Squares.GetSquare(piecePosition);
			var isValidCastleAttempt = this.IsValidCastleAttempt(gameState, oldSquare, newPiecePosition, allAttacks);
			if (isValidCastleAttempt.Success) {
				gameState.MoveInfo.IsCastle = isValidCastleAttempt.Result;
			} else {
				Envelope<MoveInfo>.Error(isValidCastleAttempt.Message);
			}
			var isEnPassant = this.IsEnPassant(oldSquare, newPiecePosition, gameState.EnPassantTargetSquare);
			if (isEnPassant) { //if is en passant, update matrix again
				var pawnPassing = newActiveColor == Color.White ? (newPiecePosition - 8) : (newPiecePosition + 8);
				gameState.Squares.GetSquare(pawnPassing).Piece = null;
			}

			var isResign = false;
			var isDraw = false;
			//todo: i don't think we can get here
			if (isDraw || isResign) {
				if (isDraw) {
					var score = string.Concat(" ", "1/2-1/2");
					gameState.PGN += score;
				}
				if (isResign) {
					var score = string.Concat(" ", newActiveColor == Color.White ? "1-0" : "0-1");
					gameState.PGN += score;
				}
			}

			return Envelope<MoveInfo>.Ok(gameState.MoveInfo);

			////doesn't test anything, just applies the move
			//NotationService.ApplyMoveToSquares(gameState.Squares, piecePosition, newPiecePosition);
			//var piece = oldSquare.Piece;

			////post move application examination

			//var isEnPassant = this.IsEnPassant(piece.Identity, piecePosition, newPiecePosition, gameState.EnPassantTargetSquare);
			//if (isEnPassant) { //if is en passant, update matrix again
			//	var pawnPassing = gameState.ActiveColor == Color.White ? (newPiecePosition - 8) : (newPiecePosition + 8);
			//	oldSquares.GetSquare(pawnPassing).Piece = null;
			//}

			//var isPawnPromotion = pgnMove.Contains(PGNService.PawnPromotionIndicator);
			//if (isPawnPromotion) { //if is a pawn promotion, update matrix again
			//	var piecePromotedTo = pgnMove.Substring(pgnMove.IndexOf(PGNService.PawnPromotionIndicator) + 1, 1)[0];
			//	NotationService.UpdateMatrix_PromotePiece(oldSquares, newPiecePosition, gameState.ActiveColor, piecePromotedTo);
			//}

			//if (oldSquare.Piece.PieceType != PieceType.Pawn) {
			//	var _isValidPawnMove = this.IsValidPawnMove(oldSquare, oldSquares, gameState.ActiveColor, piecePosition, newPiecePosition, isEnPassant);
			//	if (!_isValidPawnMove) {
			//		var errorMessage = "Invalid move.";
			//		var invalidGameState = getNewGameState(gameState, gameState.PGN, gameState.MoveInfo.HasThreefoldRepition, string.Empty, errorMessage);
			//		return invalidGameState;
			//	}
			//}

			//gameState.FEN_Records.Add(new FEN_Record(gameState.ToString()));
		}

		/// <summary>
		/// In chess, in order for a position to be considered the same, each player must have the same set of legal moves each time,
		/// including the possible rights to castle and capture en passant. Positions are considered the same if the same type of piece
		/// is on a given square. So, for instance, if a player has two knights and the knights are on the same squares, it does not
		/// matter if the positions of the two knights have been exchanged. The game is not automatically drawn if a position occurs
		/// for the third time – one of the players, on their move turn, must claim the draw with the arbiter.
		/// </summary>
		/// <returns></returns>
		public bool HasThreefoldRepition(GameState gameState) {
			return gameState.FEN_Records
					.GroupBy(a => new { a.PiecePlacement, a.CastlingAvailability, a.EnPassantTargetPosition })
					.Where(a => a.Count() >= 3)
					.Any();
		}

		//public bool IsCheckmate(GameState gameState, Square enemyKingPosition, IEnumerable<AttackedSquare> whiteAttacks, IEnumerable<AttackedSquare> blackAttacks) {
		//	var activeColor = gameState.ActiveColor;
		//	var squares = gameState.Squares;
		//	var checkedColor = gameState.ActiveColor == Color.White ? Color.White : Color.Black; //trust me this is right
		//	var kingIsBeingAttacked = whiteAttacks.Any(a => a.Index == enemyKingPosition.Index) || blackAttacks.Any(a => a.Index == enemyKingPosition.Index);
		//	if (!kingIsBeingAttacked) {
		//		return false;
		//	}
		//	//make sure that he cannot move
		//	var kingHasEscape = false;

		//	var friendlyAttacks = (activeColor == Color.White ? whiteAttacks : blackAttacks);
		//	var opponentAttacks = (activeColor == Color.White ? blackAttacks : whiteAttacks);

		//	//fix enemyKingAttacks. trying to figure out the moves that the king can make
		//	var enemyKingAttacks = squares;

		//	var remainingKingAttacks = enemyKingAttacks.Except(opponentAttacks);
		//	if (remainingKingAttacks.Any()) {
		//		kingHasEscape = true;
		//	}
		//	if (kingHasEscape) {
		//		return false;
		//	}
		//	//make sure that interposition is not possible
		//	var attackers = opponentAttacks.Where(a => a.Index == enemyKingPosition.Index);
		//	//if there are no attackers there cannot be a single interposition that saves the king
		//	if (attackers == null || !attackers.Any() || attackers.Count() > 1) {
		//		return true;
		//	}
		//	var attacker = attackers.FirstOrDefault();
		//	var theAttack = this.AttackService.GetKingAttack(attacker, gameState, enemyKingPosition);
		//	var interposers = friendlyAttacks.ToList().Intersect(theAttack);
		//	if (interposers.Any()) {
		//		return false;
		//	}
		//	//there were no friendlies to save the king, checkmate is true
		//	return true;
		//}

		public bool IsDiagonalMove(int startPosition, int endPosition) {
			var startMod = startPosition % 8;
			var endMod = endPosition % 8;
			var modDiff = Math.Abs(startMod - endMod);

			var startRow = NotationUtility.PositionToRankInt(startPosition);
			var endRow = NotationUtility.PositionToRankInt(endPosition);
			var rowDiff = Math.Abs(startRow - endRow);
			if (modDiff == rowDiff) {
				return true;
			}
			return false;
		}

		public bool IsEnPassant(Square square, int newPiecePosition, string enPassantTargetSquare) {
			var piece = square.Piece;
			if (piece.PieceType != PieceType.Pawn) { return false; } //only pawns can perform en passant
			var enPassantPosition = NotationUtility.CoordinateToPosition(enPassantTargetSquare);
			if (enPassantPosition != newPiecePosition) { return false; } //if we're not moving to the en passant position, this is not en passant
			var moveDistance = Math.Abs(square.Index - newPiecePosition);
			if (!new List<int> { 7, 9 }.Contains(moveDistance)) { return false; } //is this a diagonal move?
			if (piece.Color == Color.Black && square.Index < newPiecePosition) { return false; } //black can't move up
			if (piece.Color == Color.White && square.Index > newPiecePosition) { return false; } //white can't move down
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

		public Envelope<bool> IsValidCastleAttempt(GameState gameState, Square square, int destination, IEnumerable<AttackedSquare> attackedSquares) {
			var piece = square.Piece;

			var isCastleAttempt =
				(square.Index == 4 || square.Index == 60)
				&& piece.PieceType == PieceType.King
				&& Math.Abs(square.Index - destination) == 2;

			//if not a castle, then no validation needed.
			if (!isCastleAttempt) {
				return Envelope<bool>.Ok(isCastleAttempt);
			}

			var castlingAvailability = checkCastleAvailability(gameState, destination, piece);
			if (!castlingAvailability) {
				return Envelope<bool>.Error("Castling is not available.");
			}

			//validate the move
			if (gameState.MoveInfo.IsCheck) {
				return Envelope<bool>.Error("Can't castle out of check.");
			}

			var kingTravelSquareIndexes = getKingCastleCoordinates(square, destination);
			var reverseColor = gameState.ActiveColor.Reverse();
			var enemyAttacks = from a in attackedSquares
							   join k in kingTravelSquareIndexes on a.Index equals k
							   where a.AttackerSquare.Piece.Color == reverseColor
							   select a;

			if (enemyAttacks.Any()) {
				return Envelope<bool>.Error("Can't castle through check.");
			}

			return Envelope<bool>.Ok(isCastleAttempt);
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