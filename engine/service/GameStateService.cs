using chess.v4.engine.enumeration;
using chess.v4.engine.extensions;
using chess.v4.engine.interfaces;
using chess.v4.engine.model;
using chess.v4.engine.reference;
using chess.v4.engine.utility;
using common;
using System.Linq;

namespace chess.v4.engine.service {

	/// <summary>
	/// There is a guiding principle here. Don't edit the GameState object anywhere but here.
	/// It gets confusing.
	/// Go ahead and calculate things elsewhere, but bring the results back here to apply them.
	/// </summary>
	public class GameStateService : IGameStateService {
		public IAttackService AttackService { get; }

		public IMoveService MoveService { get; }
		public INotationService NotationService { get; }

		public GameStateService(INotationService notationService, IMoveService moveService, IAttackService attackService) {
			NotationService = notationService;

			MoveService = moveService;
			AttackService = attackService;
		}

		public Envelope<GameState> Initialize(string fen) {
			if (string.IsNullOrEmpty(fen)) {
				fen = GeneralReference.Starting_FEN_Position;
			}
			return hydrateGameState(new FEN_Record(fen));
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
		/// <returns></returns>
		public Envelope<GameState> MakeMove(GameState gameState, int piecePosition, int newPiecePosition, PieceType? piecePromotionType = null) {
			var moveInfo = this.getMoveInfo(gameState, piecePosition, newPiecePosition, piecePromotionType);
			if (moveInfo.Failure) {
				return Envelope<GameState>.Error(moveInfo.Message);
			}
			return this.makeMove(gameState, piecePosition, moveInfo.Result, newPiecePosition);
		}

		public Envelope<GameState> MakeMove(GameState gameState, string beginning, string destination, PieceType? piecePromotionType = null) {
			var pos1 = NotationUtility.CoordinateToPosition(beginning);
			var pos2 = NotationUtility.CoordinateToPosition(destination);
			return this.MakeMove(gameState, pos1, pos2, piecePromotionType);
		}

		private Envelope<StateInfo> getMoveInfo(GameState gameState, int piecePosition, int newPiecePosition, PieceType? piecePromotionType) {
			var square = gameState.Squares.GetSquare(piecePosition);
			if (!square.Occupied) {
				return Envelope<StateInfo>.Error("Square was empty.");
			}			
			var moveInfoResult = this.MoveService.GetStateInfo(gameState, piecePosition, newPiecePosition);
			if (moveInfoResult.Failure) {
				return Envelope<StateInfo>.Error(moveInfoResult.Message);
			}
			var moveInfo = moveInfoResult.Result;
			if (moveInfo.IsPawnPromotion) {
				if (!piecePromotionType.HasValue) {
					return Envelope<StateInfo>.Error("Must provide pawn promotion piece type in order to promote a pawn.");
				}
				moveInfo.PawnPromotedTo = piecePromotionType.Value;
			}
			//var putsOwnKingInCheck = false;
			if (moveInfo.IsCheck) {
				return Envelope<StateInfo>.Error("Must move out of check. Must not move into check.");
			}
			return Envelope<StateInfo>.Ok(moveInfo);
		}

		private Envelope<GameState> hydrateGameState(FEN_Record fenRecord, string errorMessage = null) {
			if (!string.IsNullOrEmpty(errorMessage)) {
				return Envelope<GameState>.Error(errorMessage);
			}
			var gameState = new GameState(fenRecord);
			gameState.Squares = NotationService.GetSquaresFromFEN_Record(gameState);
			gameState.Attacks = this.AttackService.GetAttacks(gameState, false).ToList();
			gameState.StateInfo = this.MoveService.GetStateInfo(gameState);
			return Envelope<GameState>.Ok(gameState);
		}

		private Envelope<GameState> makeMove(GameState gameState, int position, StateInfo moveInfo, int newPiecePosition) {
			var newGameState = gameState.DeepCopy();
			var oldFen = newGameState.ToString();
			newGameState.FEN_Records.Add(new FEN_Record(oldFen));
			newGameState.StateInfo = moveInfo;
			var oldSquare = newGameState.Squares.GetSquare(position);
			var oldSquareCopy = (Square)oldSquare.Clone();
			oldSquareCopy.Piece = null;
			var newSquare = newGameState.Squares.GetSquare(newPiecePosition);
			var newSquareCopy = (Square)newSquare.Clone();
			newSquareCopy.Piece = moveInfo.IsPawnPromotion
				? new Piece(moveInfo.PawnPromotedTo, gameState.ActiveColor)
				: new Piece { Identity = oldSquare.Piece.Identity, PieceType = oldSquare.Piece.PieceType, Color = oldSquare.Piece.Color };
			newGameState.Squares.Remove(oldSquare);
			newGameState.Squares.Remove(newSquare);
			newGameState.Squares.Add(oldSquareCopy);
			newGameState.Squares.Add(newSquareCopy);
			this.NotationService.SetGameState_FEN(gameState, newGameState, position, newPiecePosition);
			return Envelope<GameState>.Ok(newGameState);
		}
	}
}