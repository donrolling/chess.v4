﻿using chess.v4.engine.extensions;
using chess.v4.engine.factory;
using chess.v4.engine.interfaces;
using chess.v4.engine.reference;
using chess.v4.engine.Utility;
using chess.v4.models;
using chess.v4.models.enumeration;
using Common.Extensions;
using Common.Models;
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
		public IPGNService PGNService { get; }

		public GameStateService(INotationService notationService, IPGNService pgnService, IMoveService moveService, IAttackService attackService) {
			NotationService = notationService;
			PGNService = pgnService;
			MoveService = moveService;
			AttackService = attackService;
		}

		public Envelope<GameState> Initialize(string fen) {
			if (string.IsNullOrEmpty(fen)) {
				fen = GeneralReference.Starting_FEN_Position;
			}
			return hydrateGameState(FenFactory.Create(fen));
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
			var stateInfo = this.getStateInfo(gameState, piecePosition, newPiecePosition, piecePromotionType);
			if (stateInfo.Failure) {
				return Envelope<GameState>.Fail(stateInfo.Message);
			}
			return this.makeMove(gameState, piecePosition, stateInfo.Result, newPiecePosition);
		}

		public Envelope<GameState> MakeMove(GameState gameState, string beginning, string destination, PieceType? piecePromotionType = null) {
			var pos1 = NotationUtility.CoordinateToPosition(beginning);
			var pos2 = NotationUtility.CoordinateToPosition(destination);
			return this.MakeMove(gameState, pos1, pos2, piecePromotionType);
		}

		public Envelope<GameState> MakeMove(GameState gameState, string pgnMove) {
			var pair = this.PGNService.PGNMoveToSquarePair(gameState, pgnMove);
			//todo: what about piece promotion?
			if (pair.promotedPiece == '-') {
				return this.MakeMove(gameState, pair.piecePosition, pair.newPiecePosition);
			}
			var promotedPieceType = NotationUtility.GetPieceTypeFromCharacter(pair.promotedPiece);
			return this.MakeMove(gameState, pair.piecePosition, pair.newPiecePosition, promotedPieceType);
		}

		private static GameState manageSquares(GameState gameState, StateInfo stateInfo, int piecePosition, int newPiecePosition) {
			var movingGameState = gameState.DeepCopy();
			var oldSquare = movingGameState.Squares.GetSquare(piecePosition);
			var oldSquareCopy = oldSquare.DeepCopy();
			oldSquareCopy.Piece = null;
			var newSquare = movingGameState.Squares.GetSquare(newPiecePosition);
			var newSquareCopy = newSquare.DeepCopy();
			newSquareCopy.Piece = stateInfo.IsPawnPromotion
				? new Piece(stateInfo.PawnPromotedTo, gameState.ActiveColor)
				: oldSquare.Piece.DeepCopy();
			movingGameState.Squares.Remove(oldSquare);
			movingGameState.Squares.Remove(newSquare);
			movingGameState.Squares.Add(oldSquareCopy);
			movingGameState.Squares.Add(newSquareCopy);
			movingGameState.Squares = movingGameState.Squares.OrderBy(a => a.Index).ToList();
			return movingGameState;
		}

		private Envelope<StateInfo> getStateInfo(GameState gameState, int piecePosition, int newPiecePosition, PieceType? piecePromotionType) {
			var square = gameState.Squares.GetSquare(piecePosition);
			if (!square.Occupied) {
				return Envelope<StateInfo>.Fail("Square was empty.");
			}
			var moveInfoResult = this.MoveService.GetStateInfo(gameState, piecePosition, newPiecePosition);
			if (moveInfoResult.Failure) {
				return Envelope<StateInfo>.Fail(moveInfoResult.Message);
			}
			var moveInfo = moveInfoResult.Result;
			if (moveInfo.IsPawnPromotion) {
				if (!piecePromotionType.HasValue) {
					return Envelope<StateInfo>.Fail("Must provide pawn promotion piece type in order to promote a pawn.");
				}
				moveInfo.PawnPromotedTo = piecePromotionType.Value;
			}
			//var putsOwnKingInCheck = false;
			if (moveInfo.IsCheck) {
				return Envelope<StateInfo>.Fail("Must move out of check. Must not move into check.");
			}
			return Envelope<StateInfo>.Ok(moveInfo);
		}

		private Envelope<GameState> hydrateGameState(FEN_Record fenRecord, string errorMessage = null) {
			if (!string.IsNullOrEmpty(errorMessage)) {
				return Envelope<GameState>.Fail(errorMessage);
			}
			var gameState = new GameState(fenRecord);
			gameState.Squares = NotationService.GetSquaresFromFEN_Record(gameState);
			gameState.Attacks = this.AttackService.GetAttacks(gameState, false).ToList();
			gameState.StateInfo = this.MoveService.GetStateInfo(gameState);
			return Envelope<GameState>.Ok(gameState);
		}

		private Envelope<GameState> makeMove(GameState gameState, int piecePosition, StateInfo stateInfo, int newPiecePosition) {
			//store important stuff from old gamestate
			var previousStateFEN = gameState.ToString();
			var fenRecords = gameState.FEN_Records.DeepCopy();
			fenRecords.Add(FenFactory.Create(previousStateFEN));

			//verify that the move can be made
			var oldSquare = gameState.Squares.GetSquare(piecePosition);
			var attacks = gameState.Attacks.GetPositionAttacksOnPosition(piecePosition, newPiecePosition);
			if (!attacks.Any()) {
				return Envelope<GameState>.Fail($"Can't find an attack by this piece ({ oldSquare.Index } : { oldSquare.Piece.PieceType }) on this position ({ newPiecePosition }).");
			}
			var badPawnAttacks = attacks.Where(a =>
											a.AttackingSquare.Index == piecePosition
											&& a.Index == newPiecePosition
											&& a.Piece == null
											&& a.MayOnlyMoveHereIfOccupiedByEnemy
										);
			if (badPawnAttacks.Any()) {
				if (
					badPawnAttacks.Count() > 1 
					|| gameState.EnPassantTargetSquare == "-"
					|| newPiecePosition != NotationUtility.CoordinateToPosition(gameState.EnPassantTargetSquare)
				) {
					return Envelope<GameState>.Fail($"This piece can only move here if the new square is occupied. ({ oldSquare.Index } : { oldSquare.Piece.PieceType }) on this position ({ newPiecePosition }).");
				}
			}
			//make the move
			var movingGameState = manageSquares(gameState, stateInfo, piecePosition, newPiecePosition);
			if (stateInfo.IsCastle) {
				var rookPositions = CastleUtility.GetRookPositionsForCastle(gameState.ActiveColor, piecePosition, newPiecePosition);
				movingGameState = manageSquares(movingGameState, stateInfo, rookPositions.RookPos, rookPositions.NewRookPos);
			}
			if (stateInfo.IsEnPassant) {
				//remove the attacked pawn 
				var enPassantAttackedPawnPosition = gameState.ActiveColor == Color.White ? newPiecePosition - 8 : newPiecePosition + 8;
				var enPassantAttackedPawnSquare = movingGameState.Squares.GetSquare(enPassantAttackedPawnPosition);
				enPassantAttackedPawnSquare.Piece = null;
			}
			this.NotationService.SetGameState_FEN(gameState, movingGameState, piecePosition, newPiecePosition);
			var currentStateFEN = movingGameState.ToString();

			//Setup new gamestate
			var newGameStateResult = hydrateGameState(FenFactory.Create(currentStateFEN));
			if (newGameStateResult.Failure) {
				throw new System.Exception(newGameStateResult.Message);
			}
			var newGameState = newGameStateResult.Result;
			newGameState.FEN_Records = fenRecords;
			//make sure we moved out of check.
			if (gameState.StateInfo.IsCheck && newGameState.StateInfo.IsCheck) {
				if (gameState.ActiveColor == Color.White) {
					if (newGameState.StateInfo.IsWhiteCheck) {
						return Envelope<GameState>.Fail("King must move out of check.");
					}
				} else {
					if (newGameState.StateInfo.IsBlackCheck) {
						return Envelope<GameState>.Fail("King must move out of check.");
					}
				}
			}
			return Envelope<GameState>.Ok(newGameState);
		}
	}
}