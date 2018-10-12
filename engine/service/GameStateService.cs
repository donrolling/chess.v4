using chess.v4.engine.enumeration;
using chess.v4.engine.extensions;
using chess.v4.engine.interfaces;
using chess.v4.engine.model;
using common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace chess.v4.engine.service {

	public class GameStateService : IGameStateService {
		public INotationService NotationService { get; }
		public ICoordinateService CoordinateService { get; }
		public IMoveService MoveService { get; }
		public IAttackService AttackService { get; }

		public GameStateService(INotationService notationService, ICoordinateService coordinateService, IMoveService moveService, IAttackService attackService) {
			NotationService = notationService;
			CoordinateService = coordinateService;
			MoveService = moveService;
			AttackService = attackService;
		}

		public ResultOuput<GameState> SetStartPosition(string fen) {
			return getNewGameState(fen, string.Empty, false, string.Empty);
		}

		//Positions should be numbered 0-63 where a1 is 0
		public ResultOuput<GameState> UpdateGameState(GameState gameState, Color color, int piecePosition, int newPiecePosition, string pgnMove) {
			var newSquares = NotationService.ApplyMoveToSquares(gameState.Squares, piecePosition, newPiecePosition);
			var oldSquares = gameState.Squares;
			var square = oldSquares.GetSquare(piecePosition);
			var piece = square.Piece;

			var isCastle = MoveService.IsCastle(square, newPiecePosition);
			if (isCastle) { //if is castle, update matrix again
				if (gameState.IsCheck) {
					return ResultOuput<GameState>.Error("Can't castle out of check.");
				}

				var rookPosition = getRookPositionsForCastle(color, piecePosition, newPiecePosition);
				//todo: enemyAttacks
				var enemyAttacks = new List<Square>();
				bool isCastleThroughCheck = CoordinateService.DetermineCastleThroughCheck(gameState.Squares, enemyAttacks, gameState.FEN, color, piecePosition, rookPosition.Item1);
				if (!isCastleThroughCheck) {
					//make the second move here
					newSquares = NotationService.ApplyMoveToSquares(newSquares, rookPosition.Item1, rookPosition.Item2);
				} else {
					return ResultOuput<GameState>.Error("Can't castle through check.");
				}
			}

			bool isEnPassant = this.isEnPassant(piece.Identity, piecePosition, newPiecePosition, gameState.EnPassantTargetSquare);
			if (isEnPassant) { //if is en passant, update matrix again
				var pawnPassing = color == Color.White ? (newPiecePosition - 8) : (newPiecePosition + 8);
				oldSquares.GetSquare(pawnPassing).Piece = null;
			}

			bool isPawnPromotion = pgnMove.Contains(PGNService.PawnPromotionIndicator);
			if (isPawnPromotion) { //if is a pawn promotion, update matrix again
				var piecePromotedTo = pgnMove.Substring(pgnMove.IndexOf(PGNService.PawnPromotionIndicator) + 1, 1)[0];
				NotationService.UpdateMatrix_PromotePiece(oldSquares, newPiecePosition, color, piecePromotedTo);
			}

			if (square.Piece.PieceType != PieceType.Pawn) {
				var _isValidPawnMove = isValidPawnMove(square, oldSquares, color, piecePosition, newPiecePosition, isEnPassant);
				if (!_isValidPawnMove) {
					var errorMessage = "Invalid move.";
					var invalidGameState = getNewGameState(gameState.FEN, gameState.PGN, gameState.HasThreefoldRepition, string.Empty, errorMessage);
					return invalidGameState;
				}
			}

			gameState.History.Add(gameState);
			var newFEN = NotationService.CreateNewFENFromGameState(gameState, newSquares, piecePosition, newPiecePosition);
			var newGameState = getNewGameState(newFEN, gameState.PGN, gameState.HasThreefoldRepition, string.Empty);
			if (newGameState.Failure) {
				return newGameState;
			}
			var hasThreefoldRepition = this.hasThreefoldRepition(gameState);
			var putsOwnKingInCheck = (
					gameState.ActiveColor == Color.White
					&& newGameState.Output.IsWhiteCheck
				) || (
					gameState.ActiveColor == Color.Black
					&& newGameState.Output.IsBlackCheck
				);
			if (putsOwnKingInCheck) {
				var checkedOwnKingGameState = getNewGameState(gameState.FEN, gameState.PGN, gameState.HasThreefoldRepition, string.Empty, "You must move out of check, or at the very least, not move into check.");
				return checkedOwnKingGameState;
			}
			return newGameState;
		}

		public ResultOuput<GameState> UpdateGameStateWithError(GameState gameState, string errorMessage) {
			var newGameState = getNewGameState(gameState.FEN, gameState.PGN, gameState.HasThreefoldRepition, string.Empty, errorMessage);
			return newGameState;
		}

		private ResultOuput<GameState> getNewGameState(string fen, string pgn, bool hasThreefoldRepition, string pgnMove, string errorMessage = null) {
			if (!string.IsNullOrEmpty(errorMessage)) {
				return ResultOuput<GameState>.Error(errorMessage);
			}

			var gameState = new GameState();
			gameState.FEN = fen;
			gameState.HasThreefoldRepition = hasThreefoldRepition;
			applyGameData(gameState);

			gameState.Squares = NotationService.CreateMatrixFromFEN(gameState.FEN);

			//having problems on the 2nd time through
			var allAttacks = AttackService.GetAttacks(gameState.Squares, fen);
			var whiteAttacks = allAttacks.Where(a => a.AttackerSquare.Piece.Color == Color.White);
			var blackAttacks = allAttacks.Where(a => a.AttackerSquare.Piece.Color == Color.Black);

			var whiteKingSquare = gameState.Squares.Where(a => a.Piece != null && a.Piece.PieceType == PieceType.King && a.Piece.Color == Color.White).Single();
			var blackKingSquare = gameState.Squares.Where(a => a.Piece != null && a.Piece.PieceType == PieceType.King && a.Piece.Color == Color.Black).Single();

			//todo: refactor this so that the piece contains its own attacks?
			var attacksThatCheckWhite = blackAttacks.Where(a => a.Index == whiteKingSquare.Index);
			var attacksThatCheckBlack = whiteAttacks.Where(a => a.Index == blackKingSquare.Index);

			var isCheck = false;
			gameState.IsWhiteCheck = isRealCheck(gameState.Squares, attacksThatCheckWhite, gameState.ActiveColor, whiteKingSquare.Index);
			gameState.IsBlackCheck = isRealCheck(gameState.Squares, attacksThatCheckBlack, gameState.ActiveColor, blackKingSquare.Index);

			if (!string.IsNullOrEmpty(pgnMove)) {
				bool isPawnPromotion = pgnMove.Contains(PGNService.PawnPromotionIndicator);
				if (isPawnPromotion && isCheck) {
					pgnMove = string.Concat(pgnMove, '#');
				}
				var pgnNumbering = (gameState.ActiveColor == Color.Black ? gameState.FullmoveNumber.ToString() + ". " : string.Empty);
				var nextPGNMove = string.Concat(pgnNumbering, pgnMove, ' ');
				gameState.PGN = pgn + nextPGNMove;
			} else { gameState.PGN = pgn; }

			if (gameState.IsCheck) {
				var checkedKing = gameState.ActiveColor == Color.White ? whiteKingSquare : blackKingSquare; //trust me this is right
				var checkedColor = gameState.ActiveColor == Color.White ? Color.White : Color.Black; //trust me this is right
				gameState.IsCheckmate = isCheckmate(checkedColor, gameState.Squares, checkedKing.Index, whiteAttacks, blackAttacks);
				if (gameState.IsCheckmate) {
					var score = string.Concat(" ", gameState.ActiveColor == Color.White ? "1-0" : "0-1");
					gameState.PGN += score;
				}
			} else {
				var isResign = false;
				var isDraw = false;
				//todo: i don't think we can get here
				if (isDraw || isResign) {
					if (isDraw) {
						var score = string.Concat(" ", "1/2-1/2");
						gameState.PGN += score;
					}
					if (isResign) {
						var score = string.Concat(" ", gameState.ActiveColor == Color.White ? "1-0" : "0-1");
						gameState.PGN += score;
					}
				}
			}
			return ResultOuput<GameState>.Ok(gameState);
		}

		private static void applyGameData(GameState gameState) {
			string[] gameData = gameState.FEN.Split(' ');
			gameState.FEN = gameData[0];
			gameState.ActiveColor = gameData[1][0] == 'w' ? Color.White : Color.Black;
			gameState.CastlingAvailability = gameData[2];
			gameState.EnPassantTargetSquare = gameData[3];
			int halfmoveClock = 0;
			int fullmoveNumber = 0;
			Int32.TryParse(gameData[4], out halfmoveClock);
			Int32.TryParse(gameData[5], out fullmoveNumber);
			gameState.HalfmoveClock = halfmoveClock;
			gameState.FullmoveNumber = fullmoveNumber;
		}

		private bool isRealCheck(List<Square> squares, IEnumerable<AttackedSquare> attacksThatCheck, Color ActiveColor, int kingSquare) {
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

		private bool isValidPawnMove(Square currentSquare, List<Square> squares, Color color, int currentPosition, int newPiecePosition, bool isEnPassant) {
			var isDiagonalMove = CoordinateService.IsDiagonalMove(currentSquare.Index, newPiecePosition);
			if (!isDiagonalMove) {
				return true;
			}
			var pieceToCapture = squares.GetSquare(newPiecePosition).Piece;
			var isCapture = pieceToCapture != null;
			return isCapture || isEnPassant;
		}

		private bool isEnPassant(char piece, int piecePosition, int newPiecePosition, string enPassantTargetSquare) {
			if (char.ToUpper(piece) != 'P') { return false; } //only pawns can perform en passant
			var enPassantPosition = CoordinateService.CoordinateToPosition(enPassantTargetSquare);
			if (enPassantPosition != newPiecePosition) { return false; } //if we're not moving to the en passant position, this is not en passant
			var moveDistance = Math.Abs(piecePosition - newPiecePosition);
			if (!new List<int> { 7, 9 }.Contains(moveDistance)) { return false; } //is this a diagonal move?
			if (char.IsLower(piece) && piecePosition < newPiecePosition) { return false; } //black can't move up
			if (char.IsUpper(piece) && piecePosition > newPiecePosition) { return false; } //black can't move down
			return true;
		}

		private Tuple<int, int> getRookPositionsForCastle(Color color, int piecePosition, int newPiecePosition) {
			//manage the castle
			var rookRank = color == Color.White ? 1 : 8; //intentionally not zero based
			var rookFile = CoordinateService.IntToFile(piecePosition - newPiecePosition > 0 ? 0 : 7);
			var rookPos = CoordinateService.CoordinateToPosition(string.Concat(rookFile, rookRank.ToString()));

			var newRookFile = CoordinateService.IntToFile(piecePosition - newPiecePosition > 0 ? 3 : 5);
			var newRookPos = CoordinateService.CoordinateToPosition(string.Concat(newRookFile, rookRank.ToString()));

			return Tuple.Create<int, int>(rookPos, newRookPos);
		}

		/// <summary>
		/// In chess, in order for a position to be considered the same, each player must have the same set of legal moves each time,
		/// including the possible rights to castle and capture en passant. Positions are considered the same if the same type of piece
		/// is on a given square. So, for instance, if a player has two knights and the knights are on the same squares, it does not
		/// matter if the positions of the two knights have been exchanged. The game is not automatically drawn if a position occurs
		/// for the third time – one of the players, on their move turn, must claim the draw with the arbiter.
		/// </summary>
		/// <returns></returns>
		private bool hasThreefoldRepition(GameState gameState) {
			if (gameState.HasThreefoldRepition) { return true; }
			if (gameState.History.Count() < 5) {
				return false;
			}
			var threeIdentical = gameState.History
										.GroupBy(a => new { a.FEN, a.CastlingAvailability, a.EnPassantTargetSquare })
										.Where(a => a.Count() >= 3)
										.Select(a => new { a.Key.FEN, a.Key.CastlingAvailability, a.Key.EnPassantTargetSquare });
			return threeIdentical != null && threeIdentical.Any();
		}

		private bool isCheckmate(Color activeColor, List<Square> squares, int enemyKingPosition, IEnumerable<Square> whiteAttacks, IEnumerable<Square> blackAttacks) {
			var kingIsBeingAttacked = whiteAttacks.Any(a => a.Index == enemyKingPosition) || blackAttacks.Any(a => a.Index == enemyKingPosition);
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
			var attackers = opponentAttacks.Where(a => a.Index == enemyKingPosition);
			//if there are no attackers there cannot be a single interposition that saves the king
			if (attackers == null || !attackers.Any() || attackers.Count() > 1) {
				return true;
			}
			var attacker = attackers.FirstOrDefault();
			var attackerPiece = squares.GetPiece(attacker.Index);
			var theAttack = getAttack(attackerPiece.Color, squares, attacker.Index, enemyKingPosition, attackerPiece.PieceType);
			var interposers = friendlyAttacks.ToList().Intersect(theAttack);
			if (interposers.Any()) {
				return false;
			}
			//there were no friendlies to save the king, checkmate is true
			return true;
		}

		private List<Square> getAttack(Color attackerPieceColor, List<Square> squares, int attackerPosition, int enemyKingPosition, PieceType attackerPieceType) {
			var theAttack = new List<Square>();
			switch (attackerPieceType) {
				case PieceType.Pawn | PieceType.Knight | PieceType.King: //you can't interpose a pawn or a knight attack, also a king cannot attack a king
					break;

				case PieceType.Bishop:
					foreach (var direction in CoordinateService.DiagonalLines) {
						var potentialAttack = CoordinateService.GetDiagonalLine(squares, attackerPosition, direction, attackerPieceColor, true);
						if (potentialAttack.Any(a => a.Index == enemyKingPosition)) {
							theAttack.AddRange(potentialAttack);
							break;
						}
					}
					break;

				case PieceType.Rook:
					foreach (var direction in CoordinateService.OrthogonalLines) {
						var potentialAttack = CoordinateService.GetOrthogonalLine(squares, attackerPosition, direction, attackerPieceColor, true);
						if (potentialAttack.Any(a => a.Index == enemyKingPosition)) {
							theAttack.AddRange(potentialAttack);
							break;
						}
					}
					break;

				case PieceType.Queen:
					foreach (var direction in CoordinateService.DiagonalLines) {
						var potentialAttack = CoordinateService.GetDiagonalLine(squares, attackerPosition, direction, attackerPieceColor, true);
						if (potentialAttack.Any(a => a.Index == enemyKingPosition)) {
							theAttack.AddRange(potentialAttack);
							break;
						}
					}
					foreach (var direction in CoordinateService.OrthogonalLines) {
						var potentialAttack = CoordinateService.GetOrthogonalLine(squares, attackerPosition, direction, attackerPieceColor, true);
						if (potentialAttack.Any(a => a.Index == enemyKingPosition)) {
							theAttack.AddRange(potentialAttack);
							break;
						}
					}
					break;
			}
			return theAttack;
		}
	}
}