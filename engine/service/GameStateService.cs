using chess.v4.engine.enumeration;
using chess.v4.engine.extensions;
using chess.v4.engine.interfaces;
using chess.v4.engine.model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace chess.v4.engine.service {

	public class GameStateService : IGameStateService {
		public INotationService NotationService { get; }
		public ICoordinateService CoordinateService { get; }
		public IMoveService MoveService { get; }

		public GameStateService(INotationService notationService, ICoordinateService coordinateService, IMoveService moveService) {
			NotationService = notationService;
			CoordinateService = coordinateService;
			MoveService = moveService;
		}

		public GameState SetStartPosition(string fen) {
			return getNewGameState(fen, string.Empty, false, string.Empty);
		}

		//Positions should be numbered 0-63 where a1 is 0
		public GameState UpdateGameState(GameState gameState, Color color, int piecePosition, int newPiecePosition, string pgnMove, List<History> History) {
			NotationService.UpdateMatrix(gameState.Squares, piecePosition, newPiecePosition);
			var square = gameState.Squares.GetSquare(newPiecePosition);
			var piece = square.Piece;

			var isCastle = MoveService.IsCastle(piece, piecePosition, newPiecePosition);
			if (isCastle) { //if is castle, update matrix again
				if (gameState.IsCheck) {
					var errorMessage = "Can't castle out of check.";
					var invalidGameState = getNewGameState(gameState.FEN, gameState.PGN, gameState.HasThreefoldRepition, string.Empty, errorMessage);
					return invalidGameState;
				}

				var rookPosition = getRookPositionsForCastle(color, piecePosition, newPiecePosition);
				//todo: enemyAttacks
				var enemyAttacks = new List<Square>();
				bool isCastleThroughCheck = CoordinateService.DetermineCastleThroughCheck(gameState.Squares, enemyAttacks, gameState.FEN, color, piecePosition, rookPosition.Item1);
				if (!isCastleThroughCheck) {
					squares = NotationService.UpdateMatrix(squares, rookPosition.Item1, rookPosition.Item2);
				} else {
					var errorMessage = "Can't castle through check.";
					var invalidGameState = getNewGameState(gameState.FEN, gameState.PGN, gameState.HasThreefoldRepition, string.Empty, errorMessage);
					return invalidGameState;
				}
			}

			bool isEnPassant = this.isEnPassant(piece, piecePosition, newPiecePosition, gameState.EnPassantTargetSquare);
			if (isEnPassant) { //if is en passant, update matrix again
				var pawnPassing = color == Color.White ? (newPiecePosition - 8) : (newPiecePosition + 8);
				squares.Remove(pawnPassing);
			}

			bool isPawnPromotion = pgnMove.Contains(PGNService.PawnPromotionIndicator);
			if (isPawnPromotion) { //if is a pawn promotion, update matrix again
				var piecePromotedTo = pgnMove.Substring(pgnMove.IndexOf(PGNService.PawnPromotionIndicator) + 1, 1)[0];
				squares = NotationService.UpdateMatrix(squares, newPiecePosition, color, piecePromotedTo);
			}

			var validMove = isValidMove(gameState, color, piece, piecePosition, newPiecePosition, isEnPassant);
			if (!validMove) {
				var errorMessage = "Invalid move.";
				var invalidGameState = getNewGameState(gameState.FEN, gameState.PGN, gameState.HasThreefoldRepition, string.Empty, errorMessage);
				return invalidGameState;
			}

			var newFEN = NotationService.CreateNewFENFromGameState(gameState, squares, piecePosition, newPiecePosition);
			var hasThreefoldRepition = this.hasThreefoldRepition(color, gameState, History, newFEN);
			var newGameState = getNewGameState(newFEN, gameState.PGN, hasThreefoldRepition, pgnMove);

			var putsOwnKingInCheck = (gameState.ActiveChessTypeColor == Color.White && newGameState.IsWhiteCheck) || (gameState.ActiveChessTypeColor == Color.Black && newGameState.IsBlackCheck);
			if (putsOwnKingInCheck) {
				var checkedOwnKingGameState = getNewGameState(gameState.FEN, gameState.PGN, gameState.HasThreefoldRepition, string.Empty, "You must move out of check, or at the very least, not move into check.");
				return checkedOwnKingGameState;
			}

			return newGameState;
		}

		public GameState UpdateGameStateWithError(GameState gameState, string errorMessage) {
			var newGameState = getNewGameState(gameState.FEN, gameState.PGN, gameState.HasThreefoldRepition, string.Empty, errorMessage);
			return newGameState;
		}

		//private GameState getNewGameState(string fen, string pgn, bool hasThreefoldRepition, string pgnMove, string errorMessage = null) {
		//	var gameState = new GameState();

		//	bool isResign = false;
		//	bool isDraw = false;

		//	gameState.FEN = fen;
		//	gameState.HasThreefoldRepition = hasThreefoldRepition;
		//	var gameData = gameState.FEN.Split(' ');
		//	gameState.Position = gameData[0];
		//	gameState.ActiveColor = gameData[1][0];
		//	gameState.CastlingAvailability = gameData[2];
		//	gameState.EnPassantTargetSquare = gameData[3];

		//	int halfmoveClock = 0;
		//	int fullmoveNumber = 0;
		//	Int32.TryParse(gameData[4], out halfmoveClock);
		//	Int32.TryParse(gameData[5], out fullmoveNumber);
		//	gameState.HalfmoveClock = halfmoveClock;
		//	gameState.FullmoveNumber = fullmoveNumber;

		//	gameState.Squares = NotationService.CreateMatrixFromFEN(gameState.FEN);
		//	gameState.WhiteAttacks = PieceService.GetAttacks(Color.White, fen);
		//	gameState.BlackAttacks = PieceService.GetAttacks(Color.Black, fen);

		//	var whiteKingSquare = gameState.Squares.Where(a => a.Value == 'K').Single().Key;
		//	var blackKingSquare = gameState.Squares.Where(a => a.Value == 'k').Single().Key;

		//	var attacksThatCheckWhite = gameState.BlackAttacks.Where(a => a.Value.Contains(whiteKingSquare));
		//	var attacksThatCheckBlack = gameState.WhiteAttacks.Where(a => a.Value.Contains(blackKingSquare));

		//	bool isCheck = false;
		//	gameState.IsWhiteCheck = isRealCheck(gameState.Squares, attacksThatCheckWhite, gameState.ActiveChessTypeColor, whiteKingSquare);
		//	gameState.IsBlackCheck = isRealCheck(gameState.Squares, attacksThatCheckBlack, gameState.ActiveChessTypeColor, blackKingSquare);

		//	if (!string.IsNullOrEmpty(pgnMove)) {
		//		bool isPawnPromotion = pgnMove.Contains(PGNService.PawnPromotionIndicator);
		//		if (isPawnPromotion && isCheck) {
		//			pgnMove = string.Concat(pgnMove, '#');
		//		}
		//		var pgnNumbering = (gameState.ActiveColor == 'b' ? gameState.FullmoveNumber.ToString() + ". " : string.Empty);
		//		var nextPGNMove = string.Concat(pgnNumbering, pgnMove, ' ');
		//		gameState.PGN = pgn + nextPGNMove;
		//	} else { gameState.PGN = pgn; }

		//	if (gameState.IsCheck) {
		//		var checkedKing = gameState.ActiveChessTypeColor == Color.White ? whiteKingSquare : blackKingSquare; //trust me this is right
		//		var checkedColor = gameState.ActiveChessTypeColor == Color.White ? Color.White : Color.Black; //trust me this is right
		//		gameState.IsCheckmate = isCheckmate(checkedColor, gameState.Squares, checkedKing, gameState.WhiteAttacks, gameState.BlackAttacks);
		//		if (gameState.IsCheckmate) {
		//			var score = string.Concat(" ", gameState.ActiveChessTypeColor == Color.White ? "1-0" : "0-1");
		//			gameState.PGN += score;
		//		}
		//	} else {
		//		if (isDraw || isResign) {
		//			if (isDraw) {
		//				var score = string.Concat(" ", "1/2-1/2");
		//				gameState.PGN += score;
		//			}
		//			if (isResign) {
		//				var score = string.Concat(" ", gameState.ActiveChessTypeColor == Color.White ? "1-0" : "0-1");
		//				gameState.PGN += score;
		//			}
		//		}
		//	}

		//	if (!string.IsNullOrEmpty(errorMessage)) {
		//		gameState.MoveFailureMessage = errorMessage;
		//		gameState.MoveSuccess = false;
		//	} else {
		//		gameState.MoveSuccess = true;
		//	}

		//	return gameState;
		//}

		private bool isRealCheck(Dictionary<int, char> matrix, IEnumerable<KeyValuePair<int, List<int>>> attacksThatCheck, Color activeChessTypeColor, int kingSquare) {
			bool isRealCheck = false;
			if (attacksThatCheck != null && attacksThatCheck.Any()) {
				isRealCheck = true;
				if (attacksThatCheck.Count() == 1) { //if there are more, it's not possible that we'd need to remove the attack
					var key = attacksThatCheck.First().Key;
					var attackingPiece = matrix[key];
					//this code is here to remove the possibility that the king is said to be in check by an enemy pawn when he is directly in front of the pawn
					if (char.ToUpper(attackingPiece) == 'P') {
						var onSameFile = (key % 8) == (kingSquare % 8) ? true : false;
						if (onSameFile) {
							isRealCheck = false;
						}
					}
				}
			}
			return isRealCheck;
		}

		private bool isValidMove(GameState gameState, Color color, char piece, int piecePosition, int newPiecePosition, bool isEnPassant) {
			if (char.ToUpper(piece) == 'P') {
				var isDiagonalMove = CoordinateService.IsDiagonalMove(piecePosition, newPiecePosition);
				if (isDiagonalMove) {
					var isCapture = gameState.Squares.ContainsKey(newPiecePosition);
					if (isCapture || isEnPassant) {
						return true;
					} else {
						return false;
					}
				}
			}
			return true;
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
		private bool hasThreefoldRepition(Color color, GameState gameState, List<History> History, string newFEN) {
			if (gameState.HasThreefoldRepition) { return true; }

			var historySize = History.Count();
			if (historySize > 5) {
				var newHistory = new List<History>();
				History.ForEach(b => { newHistory.Add(b); });
				var newGameState = getNewGameState(newFEN, gameState.PGN, gameState.HasThreefoldRepition, string.Empty);
				var newHistory = GetHistory(newGameState);
				newHistory.Add(newHistory);

				var threeIdentical = newHistory
											.GroupBy(a => new { a.Position, a.CastlingAvailability, a.EnPassantTargetSquare })
											.Where(a => a.Count() >= 3)
											.Select(a => new { a.Key.Position, a.Key.CastlingAvailability, a.Key.EnPassantTargetSquare });
				if (threeIdentical != null && threeIdentical.Any()) { return true; }
			}

			return false;
		}

		private bool isCheckmate(Color activeChessTypeColor, Dictionary<int, char> matrix, int enemyKingSquare, Dictionary<int, List<int>> whiteAttacks, Dictionary<int, List<int>> blackAttacks) {
			bool kingIsBeingAttacked = whiteAttacks.ContainsKey(enemyKingSquare) || blackAttacks.ContainsKey(enemyKingSquare);
			if (kingIsBeingAttacked) {
				//make sure that he cannot move
				bool kingHasEscape = false;

				var friendlyAttacks = (activeChessTypeColor == Color.White ? whiteAttacks : blackAttacks);
				var opponentAttacks = (activeChessTypeColor == Color.White ? blackAttacks : whiteAttacks);

				var kingAttacks = friendlyAttacks.Where(a => a.Key == enemyKingSquare).FirstOrDefault().Value;
				var opponentFlatAttacks = opponentAttacks.SelectMany(a => a.Value);

				var remainingKingAttacks = kingAttacks.Except(opponentFlatAttacks);
				if (remainingKingAttacks.Any()) {
					kingHasEscape = true;
				}
				if (!kingHasEscape) { //make sure that interposition is not possible
					var attackers = opponentAttacks.Where(a => a.Value.Contains(enemyKingSquare));
					if (attackers != null && attackers.Count() == 1) { //if there is more than one attacker there cannot be an interposition that saves the king...i think
						var attacker = attackers.FirstOrDefault();
						var attackerPiece = matrix.Where(a => a.Key == attacker.Key).FirstOrDefault().Value;
						var attackerPieceType = CoordinateService.GetPieceTypeFromChar(attackerPiece);
						var attackerPieceColor = CoordinateService.GetColorFromChar(attackerPiece);

						var theAttack = getAttack(attackerPieceColor, matrix, attacker.Key, enemyKingSquare, attackerPieceType);

						var friendlyFlatAttacks = friendlyAttacks.SelectMany(a => a.Value);
						var interposers = friendlyFlatAttacks.Intersect(theAttack);
						if (interposers.Any()) { return false; }
					}
					return true; //there were no friendlies to save the king, checkmate is true
				}
			}
			return false;
		}

		private static List<int> getAttack(Color attackerPieceColor, Dictionary<int, char> matrix, int attackerPosition, int enemyKingSquare, PieceType attackerPieceType) {
			var theAttack = new List<int>();
			switch (attackerPieceType) {
				case PieceType.Pawn | PieceType.Knight | PieceType.King: //you can't interpose a pawn or a knight attack, also a king cannot attack a king
					break;

				case PieceType.Bishop:
					foreach (var direction in CoordinateService.DiagonalLines) {
						var potentialAttack = CoordinateService.GetDiagonalLine(matrix, attackerPosition, direction, attackerPieceColor, true);
						if (potentialAttack.Contains(enemyKingSquare)) {
							theAttack.AddRange(potentialAttack);
							break;
						}
					}
					break;

				case PieceType.Rook:
					foreach (var direction in CoordinateService.OrthogonalLines) {
						var potentialAttack = CoordinateService.GetOrthogonalLine(matrix, attackerPosition, direction, attackerPieceColor, true);
						if (potentialAttack.Contains(enemyKingSquare)) {
							theAttack.AddRange(potentialAttack);
							break;
						}
					}
					break;

				case PieceType.Queen:
					foreach (var direction in CoordinateService.DiagonalLines) {
						var potentialAttack = CoordinateService.GetDiagonalLine(matrix, attackerPosition, direction, attackerPieceColor, true);
						if (potentialAttack.Contains(enemyKingSquare)) {
							theAttack.AddRange(potentialAttack);
							break;
						}
					}
					foreach (var direction in CoordinateService.OrthogonalLines) {
						var potentialAttack = CoordinateService.GetOrthogonalLine(matrix, attackerPosition, direction, attackerPieceColor, true);
						if (potentialAttack.Contains(enemyKingSquare)) {
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