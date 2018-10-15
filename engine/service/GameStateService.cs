using chess.v4.engine.enumeration;
using chess.v4.engine.extensions;
using chess.v4.engine.interfaces;
using chess.v4.engine.model;
using chess.v4.engine.utility;
using common;
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

		public ResultOuput<GameState> Initialize(string fen) {
			return getNewGameState(new FEN_Record(fen), string.Empty, false, string.Empty);
		}

		/// <summary>
		/// Examine the move for validity return game state with error if invalid.
		/// Copy the game state.
		/// Apply the move to new game state.
		/// Examine the move for issues such as king check return old game state with error if invalid.
		/// If no issues, return new game state.
		/// </summary>
		/// <param name="gameState"></param>
		/// <param name="piecePosition">Positions should be numbered 0-63 where a1 is 0</param>
		/// <param name="newPiecePosition">Positions should be numbered 0-63 where a1 is 0</param>
		/// <param name="pgnMove"></param>
		/// <returns></returns>
		public ResultOuput<GameState> MakeMove(GameState gameState, int piecePosition, int newPiecePosition, string pgnMove) {
			var square = gameState.Squares.GetSquare(piecePosition);
			if (!square.Occupied) {
				return ResultOuput<GameState>.Error("Square was empty.");
			}
			var newGameState = gameState.DeepCopy();
			var allAttacks = this.AttackService.GetAttacks(gameState, false);
			this.getMoveInfo(newGameState, piecePosition, newPiecePosition, allAttacks);

			var oldSquares = gameState.Squares;
			var newSquares = newGameState.Squares;
			var piece = square.Piece;
			var color = gameState.ActiveColor;

			
			//var newFEN = NotationService.CreateNewFENFromGameState(gameState, newSquares, piecePosition, newPiecePosition);
			//var newGameState = getNewGameState(newFEN, gameState.PGN, gameState.MoveInfo.HasThreefoldRepition, string.Empty);
			//if (newGameState.Failure) {
			//	return newGameState;
			//}
			var hasThreefoldRepition = this.hasThreefoldRepition(gameState);
			var putsOwnKingInCheck = (
					gameState.ActiveColor == Color.White
					&& newGameState.MoveInfo.IsWhiteCheck
				) || (
					gameState.ActiveColor == Color.Black
					&& newGameState.MoveInfo.IsBlackCheck
				);
			if (putsOwnKingInCheck) {
				var checkedOwnKingGameState = getNewGameState(gameState, gameState.PGN, gameState.MoveInfo.HasThreefoldRepition, string.Empty, "You must move out of check, or at the very least, not move into check.");
				return checkedOwnKingGameState;
			}
			return ResultOuput<GameState>.Ok(newGameState);
		}

		public ResultOuput<GameState> MakeMove(GameState gameState, string beginning, string destination) {
			var pos1 = CoordinateService.CoordinateToPosition("e2");
			var pos2 = CoordinateService.CoordinateToPosition("e4");
			return this.MakeMove(gameState, pos1, pos2, string.Empty);
		}

		private ResultOuput<MoveInfo> getMoveInfo(GameState gameState, int piecePosition, int newPiecePosition, IEnumerable<AttackedSquare> allAttacks) {
			gameState.ActiveColor = gameState.ActiveColor.Reverse();
			gameState.MoveInfo = new MoveInfo();
			var oldSquare = gameState.Squares.GetSquare(piecePosition);
			var isValidCastleAttempt = this.MoveService.IsValidCastleAttempt(gameState, oldSquare, newPiecePosition, allAttacks);
			if (isValidCastleAttempt.Sucess) {
				gameState.MoveInfo.IsCastle = isValidCastleAttempt.Output;
			} else {
				ResultOuput<MoveInfo>.Error(isValidCastleAttempt.Message);
			}
			return ResultOuput<MoveInfo>.Ok(gameState.MoveInfo);

			////doesn't test anything, just applies the move
			//NotationService.ApplyMoveToSquares(gameState.Squares, piecePosition, newPiecePosition);
			//var piece = oldSquare.Piece;

			////post move application examination

			//var isEnPassant = this.MoveService.IsEnPassant(piece.Identity, piecePosition, newPiecePosition, gameState.EnPassantTargetSquare);
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
			//	var _isValidPawnMove = this.MoveService.IsValidPawnMove(oldSquare, oldSquares, gameState.ActiveColor, piecePosition, newPiecePosition, isEnPassant);
			//	if (!_isValidPawnMove) {
			//		var errorMessage = "Invalid move.";
			//		var invalidGameState = getNewGameState(gameState, gameState.PGN, gameState.MoveInfo.HasThreefoldRepition, string.Empty, errorMessage);
			//		return invalidGameState;
			//	}
			//}

			//gameState.FEN_Records.Add(new FEN_Record(gameState.ToString()));
		}

		private ResultOuput<GameState> getNewGameState(FEN_Record fenRecord, string pgn, bool hasThreefoldRepition, string pgnMove, string errorMessage = null) {
			if (!string.IsNullOrEmpty(errorMessage)) {
				return ResultOuput<GameState>.Error(errorMessage);
			}

			var gameState = new GameState(fenRecord);
			gameState.MoveInfo.HasThreefoldRepition = hasThreefoldRepition;

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
			gameState.MoveInfo.IsWhiteCheck = this.MoveService.IsRealCheck(gameState.Squares, attacksThatCheckWhite, gameState.ActiveColor, whiteKingSquare.Index);
			gameState.MoveInfo.IsBlackCheck = this.MoveService.IsRealCheck(gameState.Squares, attacksThatCheckBlack, gameState.ActiveColor, blackKingSquare.Index);

			if (!string.IsNullOrEmpty(pgnMove)) {
				bool isPawnPromotion = pgnMove.Contains(PGNService.PawnPromotionIndicator);
				if (isPawnPromotion && isCheck) {
					pgnMove = string.Concat(pgnMove, '#');
				}
				var pgnNumbering = (gameState.ActiveColor == Color.Black ? gameState.FullmoveNumber.ToString() + ". " : string.Empty);
				var nextPGNMove = string.Concat(pgnNumbering, pgnMove, ' ');
				gameState.PGN = pgn + nextPGNMove;
			} else { gameState.PGN = pgn; }

			if (gameState.MoveInfo.IsCheck) {
				var checkedKing = gameState.ActiveColor == Color.White ? whiteKingSquare : blackKingSquare; //trust me this is right
				gameState.MoveInfo.IsCheckmate = this.MoveService.IsCheckmate(gameState, checkedKing, allAttacks, blackAttacks);
				if (gameState.MoveInfo.IsCheckmate) {
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

		/// <summary>
		/// In chess, in order for a position to be considered the same, each player must have the same set of legal moves each time,
		/// including the possible rights to castle and capture en passant. Positions are considered the same if the same type of piece
		/// is on a given square. So, for instance, if a player has two knights and the knights are on the same squares, it does not
		/// matter if the positions of the two knights have been exchanged. The game is not automatically drawn if a position occurs
		/// for the third time – one of the players, on their move turn, must claim the draw with the arbiter.
		/// </summary>
		/// <returns></returns>
		private bool hasThreefoldRepition(GameState gameState) {
			return gameState.FEN_Records
					.GroupBy(a => new { a.PiecePlacement, a.CastlingAvailability, a.EnPassantTargetPosition })
					.Where(a => a.Count() >= 3)
					.Any();
		}
	}
}