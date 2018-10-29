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
		public IDiagonalService DiagonalService { get; }

		public IOrthogonalService OrthogonalService { get; }

		//kings don't count here
		private List<PieceType> diagonalAttackers = new List<PieceType> { PieceType.Queen, PieceType.Pawn, PieceType.Bishop };

		//kings don't count here
		private List<PieceType> orthogonalAttackers = new List<PieceType> { PieceType.Queen, PieceType.Rook };

		public MoveService(IOrthogonalService orthogonalService, IDiagonalService diagonalService) {
			OrthogonalService = orthogonalService;
			DiagonalService = diagonalService;
		}

		public Envelope<StateInfo> GetStateInfo(GameState gameState, int piecePosition, int newPiecePosition) {
			var newActiveColor = gameState.ActiveColor.Reverse();
			var stateInfo = new StateInfo();
			var oldSquare = gameState.Squares.GetSquare(piecePosition);

			var isValidCastleAttempt = this.IsValidCastleAttempt(gameState, oldSquare, newPiecePosition, gameState.Attacks);
			if (isValidCastleAttempt.Success) {
				stateInfo.IsCastle = isValidCastleAttempt.Result;
			} else {
				return Envelope<StateInfo>.Error(isValidCastleAttempt.Message);
			}

			stateInfo.IsPawnPromotion = this.isPawnPromotion(oldSquare, newPiecePosition);

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

			return Envelope<StateInfo>.Ok(stateInfo);
		}

		public StateInfo GetStateInfo(GameState gameState) {
			var stateInfo = new StateInfo();
			var whiteKingAttacks = getAttacksOnKing(gameState, Color.White);
			stateInfo.IsWhiteCheck = whiteKingAttacks.Any();
			if (stateInfo.IsWhiteCheck) {
				var isCheckMate = this.isCheckMate(gameState, Color.White, whiteKingAttacks);
				if (isCheckMate) {
					stateInfo.IsCheckmate = true;
				}
			} else {
				var blackKingAttacks = getAttacksOnKing(gameState, Color.Black);
				stateInfo.IsBlackCheck = blackKingAttacks.Any();
				if (stateInfo.IsBlackCheck) {
					var isCheckMate = this.isCheckMate(gameState, Color.Black, blackKingAttacks);
					if (isCheckMate) {
						stateInfo.IsCheckmate = true;
					}
				}
			}
			stateInfo.HasThreefoldRepition = this.HasThreefoldRepition(gameState);
			return stateInfo;
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
		public bool IsEnPassant(Square square, int newPiecePosition, string enPassantTargetSquare) {
			if (enPassantTargetSquare == "-") {
				return false;
			}
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

		//	//fix enemyKingAttacks. trying to figure out the moves that the king can make
		//	var enemyKingAttacks = squares;
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

		//	var friendlyAttacks = (activeColor == Color.White ? whiteAttacks : blackAttacks);
		//	var opponentAttacks = (activeColor == Color.White ? blackAttacks : whiteAttacks);
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

			var castleInfo = checkCastleAvailability(gameState, destination, piece);
			if (!castleInfo.CastleAvailability) {
				return Envelope<bool>.Error("Castling is not available.");
			}

			//validate the move
			if (gameState.StateInfo.IsCheck) {
				return Envelope<bool>.Error("Can't castle out of check.");
			}

			var castleThroughCheck = CastleUtility.DetermineCastleThroughCheck(gameState, square.Index, castleInfo.RookPosition);
			if (castleThroughCheck) {
				return Envelope<bool>.Error("Can't castle through check.");
			}

			return Envelope<bool>.Ok(isCastleAttempt);
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
		public bool IsValidPawnMove(Square currentSquare, List<Square> squares, Color color, int piecePosition, int newPiecePosition, bool isEnPassant) {
			var isDiagonalMove = this.IsDiagonalMove(currentSquare.Index, newPiecePosition);
			if (!isDiagonalMove) {
				return true;
			}
			var pieceToCapture = squares.GetSquare(newPiecePosition).Piece;
			var isCapture = pieceToCapture != null;
			return isCapture || isEnPassant;
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

		private (bool CastleAvailability, int RookPosition) checkCastleAvailability(GameState gameState, int destination, Piece piece) {
			if (gameState.CastlingAvailability == "-") {
				return (false, 0);
			}
			var castlingAvailability = true;
			switch (piece.Color) {
				case Color.Black:
					switch (destination) {
						case 58:
							return (gameState.CastlingAvailability.Contains("q", StringComparison.CurrentCulture), 56);

						case 62:
							return (gameState.CastlingAvailability.Contains("k", StringComparison.CurrentCulture), 63);

						default:
							throw new Exception("Invalid destination.");
					}

				case Color.White:
					switch (destination) {
						case 2:
							return (gameState.CastlingAvailability.Contains("Q", StringComparison.CurrentCulture), 0);

						case 6:
							return (gameState.CastlingAvailability.Contains("K", StringComparison.CurrentCulture), 7);

						default:
							throw new Exception("Invalid destination.");
					}
					break;

				default:
					throw new Exception("Enum not matched.");
			}
		}

		private IEnumerable<AttackedSquare> getAttacksOnKing(GameState gameState, Color color) {
			return gameState.Attacks.Where(a => a.Occupied && a.Piece.Color == color && a.Piece.PieceType == PieceType.King);
		}

		private Square getKing(GameState gameState, Color color) {
			return gameState.Squares.Where(a => a.Occupied && a.Piece.Color == color && a.Piece.PieceType == PieceType.King).First();
		}

		private bool isCheckMate(GameState gameState, Color kingColor, IEnumerable<AttackedSquare> attacksOnKing) {
			var opponentAttacks = gameState.Attacks.Where(a => a.AttackerSquare.Piece.Color == kingColor.Reverse()).ToList();
			var kingMoves = gameState.Attacks.Where(a => a.AttackerSquare.Piece.PieceType == PieceType.King && a.AttackerSquare.Piece.Color == kingColor).ToList();
			var clearMoves = kingMoves.Select(a => a.Index).Except(opponentAttacks.Select(b => b.Index));
			if (clearMoves.Any()) {
				//todo: there is a bug here: when the king is checked like so (5rk1/5pbp/5Qp1/8/8/8/5PPP/3q2K1 w - - 0 1)
				//he is boxed in, but h1 looks a valid move for him because the Queen doesn't currently have it as an attack square
				//because the King is blocking it.
				//fix this by examining if the king is moving orthoganally or diagonally and determine if any attackers are on that line as well
				var clearMoveCount = clearMoves.Count();
				foreach (var clearMoveIndex in clearMoves) {
					var clearMove = kingMoves.GetSquare(clearMoveIndex);
					//clearMove.AttackerSquare is the king here
					//clearMove.Index is where he is going
					var isOrthogonal = GeneralUtility.IsOrthogonal(clearMove.AttackerSquare.Index, clearMove.Index);
					if (isOrthogonal) {
						var isRankMove = GeneralUtility.GivenOrthogonalMove_IsItARankMove(clearMove.AttackerSquare.Index, clearMove.Index);
						//find all attackers who attack orthogonally and determine if they are on the same line
						var orthogonalAttacksOnKing = attacksOnKing.Where(a => orthogonalAttackers.Contains(a.AttackerSquare.Piece.PieceType));
						if (!orthogonalAttacksOnKing.Any()) { continue; }
						foreach (var x in orthogonalAttacksOnKing) {
							if (isRankMove) {
								var rank = NotationUtility.PositionToRank(x.Index);
								var entireRank = this.OrthogonalService.GetEntireRank(rank);
								//is clearMove going to be on this rank?
								//if so, we're still in check
								var sameRank = entireRank.Contains(clearMoveIndex);
								if (sameRank) {
									clearMoveCount--;
								}
							} else {
								var file = NotationUtility.PositionToFile(x.Index);
								var entireFile = this.OrthogonalService.GetEntireFile(file);
								//is clearMove going to be on this file?
								//if so, we're still in check
								var sameFile = entireFile.Contains(clearMoveIndex);
								if (sameFile) {
									clearMoveCount--;
								}
							}
						}
					} else {
						//has to be diagonal
						var diagonalAttacksOnKing = attacksOnKing.Where(a => diagonalAttackers.Contains(a.AttackerSquare.Piece.PieceType));
						if (!diagonalAttacksOnKing.Any()) { continue; }
						foreach (var x in diagonalAttacksOnKing) {
							//diagonal moves: rise LtoR /  or RtoL \ 
							var isLtoRMove = GeneralUtility.GivenOrthogonalMove_IsItARankMove(clearMove.AttackerSquare.Index, clearMove.Index);

						}
					}
				}
				return clearMoveCount > 0;
			}
			//find interpositions
			var teamAttacks = gameState.Attacks.Where(a => a.AttackerSquare.Piece.Color == kingColor);
			var interpositions = attacksOnKing.Select(a => a.Index).Intersect(teamAttacks.Select(a => a.Index));
			if (!interpositions.Any()) { return true; }
			//now see if any of the interpositions stop all attacks
			foreach (var interposition in interpositions) {
				//todo:
			}
			return false;
		}

		private bool isPawnPromotion(Square square, int newPiecePosition) {
			if (square.Piece.PieceType != PieceType.Pawn) {
				return false;
			}
			if (square.Piece.Color == Color.White && newPiecePosition >= 56 && newPiecePosition <= 63) {
				return true;
			}
			if (square.Piece.Color == Color.Black && newPiecePosition >= 0 && newPiecePosition <= 7) {
				return true;
			}
			return false;
		}
	}
}