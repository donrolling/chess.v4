using chess.v4.engine.enumeration;
using chess.v4.engine.extensions;
using chess.v4.engine.interfaces;
using chess.v4.engine.model;
using chess.v4.engine.reference;
using chess.v4.engine.utility;
using common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace chess.v4.engine.service {

	public class GameStateService : IGameStateService {
		public IAttackService AttackService { get; }
		public ICoordinateService CoordinateService { get; }
		public IMoveService MoveService { get; }
		public INotationService NotationService { get; }

		public GameStateService(INotationService notationService, ICoordinateService coordinateService, IMoveService moveService, IAttackService attackService) {
			NotationService = notationService;
			CoordinateService = coordinateService;
			MoveService = moveService;
			AttackService = attackService;
		}

		public ResultOuput<GameState> SetStartPosition(string fen) {
			return getNewGameState(new FEN_Record(fen), string.Empty, false, string.Empty);
		}

		//Positions should be numbered 0-63 where a1 is 0
		public ResultOuput<GameState> MakeMove(GameState gameState, int piecePosition, int newPiecePosition, string pgnMove) {
			var newSquares = NotationService.ApplyMoveToSquares(gameState.Squares, piecePosition, newPiecePosition);
			var oldSquares = gameState.Squares;
			var square = oldSquares.GetSquare(piecePosition);
			var piece = square.Piece;
			var color = gameState.ActiveColor;

			var isCastle = MoveService.IsCastle(square, newPiecePosition);
			if (isCastle) { //if is castle, update matrix again
				if (gameState.IsCheck) {
					return ResultOuput<GameState>.Error("Can't castle out of check.");
				}

				var rookPosition = getRookPositionsForCastle(color, piecePosition, newPiecePosition);
				//todo: enemyAttacks
				var enemyAttacks = new List<Square>();
				var isCastleThroughCheck = MoveService.DetermineCastleThroughCheck(gameState, enemyAttacks, piecePosition, rookPosition.Item1);
				if (!isCastleThroughCheck) {
					//make the second move here
					newSquares = NotationService.ApplyMoveToSquares(newSquares, rookPosition.Item1, rookPosition.Item2);
				} else {
					return ResultOuput<GameState>.Error("Can't castle through check.");
				}
			}

			bool isEnPassant = this.MoveService.IsEnPassant(piece.Identity, piecePosition, newPiecePosition, gameState.EnPassantTargetSquare);
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
				var _isValidPawnMove = this.MoveService.IsValidPawnMove(square, oldSquares, color, piecePosition, newPiecePosition, isEnPassant);
				if (!_isValidPawnMove) {
					var errorMessage = "Invalid move.";
					var invalidGameState = getNewGameState(gameState, gameState.PGN, gameState.HasThreefoldRepition, string.Empty, errorMessage);
					return invalidGameState;
				}
			}

			gameState.FEN_Records.Add(gameState);
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
				var checkedOwnKingGameState = getNewGameState(gameState, gameState.PGN, gameState.HasThreefoldRepition, string.Empty, "You must move out of check, or at the very least, not move into check.");
				return checkedOwnKingGameState;
			}
			return newGameState;
		}

		public ResultOuput<GameState> MakeMove(GameState gameState, string beginning, string destination) {
			var pos1 = CoordinateService.CoordinateToPosition("e2");
			var pos2 = CoordinateService.CoordinateToPosition("e4");
			return this.MakeMove(gameState, pos1, pos2, string.Empty);
		}

		public ResultOuput<GameState> UpdateGameStateWithError(GameState gameState, string errorMessage) {
			var newGameState = getNewGameState(gameState, gameState.PGN, gameState.HasThreefoldRepition, string.Empty, errorMessage);
			return newGameState;
		}

		private ResultOuput<GameState> getNewGameState(FEN_Record fenRecord, string pgn, bool hasThreefoldRepition, string pgnMove, string errorMessage = null) {
			if (!string.IsNullOrEmpty(errorMessage)) {
				return ResultOuput<GameState>.Error(errorMessage);
			}

			var gameState = new GameState(fenRecord);
			gameState.HasThreefoldRepition = hasThreefoldRepition;

			gameState.Squares = NotationService.GetSquaresFromFEN_Record(gameState);

			//having problems on the 2nd time through
			var allAttacks = AttackService.GetAttacks(gameState, false);
			var whiteAttacks = allAttacks.Where(a => a.AttackerSquare.Piece.Color == Color.White);
			var blackAttacks = allAttacks.Where(a => a.AttackerSquare.Piece.Color == Color.Black);

			var whiteKingSquare = gameState.Squares.Where(a => a.Piece != null && a.Piece.PieceType == PieceType.King && a.Piece.Color == Color.White).Single();
			var blackKingSquare = gameState.Squares.Where(a => a.Piece != null && a.Piece.PieceType == PieceType.King && a.Piece.Color == Color.Black).Single();

			//todo: refactor this so that the piece contains its own attacks?
			var attacksThatCheckWhite = blackAttacks.Where(a => a.Index == whiteKingSquare.Index);
			var attacksThatCheckBlack = whiteAttacks.Where(a => a.Index == blackKingSquare.Index);

			var isCheck = false;
			gameState.IsWhiteCheck = this.MoveService.IsRealCheck(gameState.Squares, attacksThatCheckWhite, gameState.ActiveColor, whiteKingSquare.Index);
			gameState.IsBlackCheck = this.MoveService.IsRealCheck(gameState.Squares, attacksThatCheckBlack, gameState.ActiveColor, blackKingSquare.Index);

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
				gameState.IsCheckmate = this.MoveService.IsCheckmate(gameState, checkedKing, allAttacks, blackAttacks);
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
			if (gameState.FEN_Records.Count() < 5) {
				return false;
			}
			return gameState.FEN_Records
					.GroupBy(a => new { a.PiecePlacement, a.CastlingAvailability, a.EnPassantTargetPosition })
					.Where(a => a.Count() >= 3)
					.Any();
		}
	}
}