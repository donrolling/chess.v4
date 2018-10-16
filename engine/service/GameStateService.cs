﻿using chess.v4.engine.enumeration;
using chess.v4.engine.extensions;
using chess.v4.engine.interfaces;
using chess.v4.engine.model;
using common;
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

		public Envelope<GameState> Initialize(string fen) {
			return getNewGameState(new FEN_Record(fen));
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
		public Envelope<GameState> MakeMove(GameState gameState, int piecePosition, int newPiecePosition, string pgnMove) {
			var square = gameState.Squares.GetSquare(piecePosition);
			if (!square.Occupied) {
				return Envelope<GameState>.Error("Square was empty.");
			}
			var allAttacks = this.AttackService.GetAttacks(gameState, false);
			var moveInfoResult = this.MoveService.GetMoveInfo(gameState, piecePosition, newPiecePosition, allAttacks);
			if (moveInfoResult.Failure) {
				return Envelope<GameState>.Error(moveInfoResult.Message);
			}
			var moveInfo = moveInfoResult.Result;
			//var putsOwnKingInCheck = false;
			if (moveInfo.IsCheck) {
				return Envelope<GameState>.Error("Must move out of check. Must not move into check.");
			}
			return this.makeMove(gameState, piecePosition, moveInfo, newPiecePosition);
		}

		public Envelope<GameState> MakeMove(GameState gameState, string beginning, string destination) {
			var pos1 = CoordinateService.CoordinateToPosition("e2");
			var pos2 = CoordinateService.CoordinateToPosition("e4");
			return this.MakeMove(gameState, pos1, pos2, string.Empty);
		}

		private Envelope<GameState> getNewGameState(FEN_Record fenRecord, string errorMessage = null) {
			if (!string.IsNullOrEmpty(errorMessage)) {
				return Envelope<GameState>.Error(errorMessage);
			}

			var gameState = new GameState(fenRecord);

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

			//gameState.MoveInfo = this.MoveService.GetMoveInfo(gameState, allAttacks);

			//if (!string.IsNullOrEmpty(pgnMove)) {
			//	bool isPawnPromotion = pgnMove.Contains(PGNService.PawnPromotionIndicator);
			//	if (isPawnPromotion && isCheck) {
			//		pgnMove = string.Concat(pgnMove, '#');
			//	}
			//	var pgnNumbering = (gameState.ActiveColor == Color.Black ? gameState.FullmoveNumber.ToString() + ". " : string.Empty);
			//	var nextPGNMove = string.Concat(pgnNumbering, pgnMove, ' ');
			//	gameState.PGN = pgn + nextPGNMove;
			//} else { gameState.PGN = pgn; }

			//if (gameState.MoveInfo.IsCheck) {
			//	var checkedKing = gameState.ActiveColor == Color.White ? whiteKingSquare : blackKingSquare; //trust me this is right
			//	gameState.MoveInfo.IsCheckmate = this.MoveService.IsCheckmate(gameState, checkedKing, allAttacks, blackAttacks);
			//	if (gameState.MoveInfo.IsCheckmate) {
			//		var score = string.Concat(" ", gameState.ActiveColor == Color.White ? "1-0" : "0-1");
			//		gameState.PGN += score;
			//	}
			//}
			return Envelope<GameState>.Ok(gameState);
		}

		private Envelope<GameState> makeMove(GameState gameState, int position, MoveInfo moveInfo, int newPiecePosition) {
			var newGameState = gameState.DeepCopy();
			newGameState.MoveInfo = moveInfo;
			var oldFen = gameState.ToString();
			var oldSquare = newGameState.Squares.GetSquare(position);
			var newSquare = newGameState.Squares.GetSquare(newPiecePosition);
			newSquare.Piece = new Piece {
				Identity = oldSquare.Piece.Identity,
				PieceType = oldSquare.Piece.PieceType,
				Color = oldSquare.Piece.Color
			};
			oldSquare.Piece = null;
			newGameState.FEN_Records.Add(new FEN_Record(oldFen));
			return Envelope<GameState>.Ok(newGameState);
		}
	}
}