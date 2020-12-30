using Chess.v4.Engine.Extensions;
using Chess.v4.Engine.Factory;
using Chess.v4.Engine.Interfaces;
using Chess.v4.Engine.Reference;
using Chess.v4.Engine.Utility;
using Chess.v4.Models;
using Chess.v4.Models.Enums;
using Common.Extensions;
using Common.Responses;
using System.Linq;

namespace Chess.v4.Engine.Service
{
    /// <summary>
    /// There is a guiding principle here. Don't edit the GameState object anywhere but here.
    /// It gets confusing.
    /// Go ahead and calculate things elsewhere, but bring the results back here to apply them.
    /// </summary>
    public class GameStateService : IGameStateService
    {
        private readonly IAttackService _attackService;
        private readonly IMoveService _moveService;
        private readonly INotationService _notationService;
        private readonly IPGNService _pgnService;

        public GameStateService(INotationService notationService, IPGNService pgnService, IMoveService moveService, IAttackService attackService)
        {
            _notationService = notationService;
            _pgnService = pgnService;
            _moveService = moveService;
            _attackService = attackService;
        }

        public OperationResult<GameState> Initialize(string fen)
        {
            if (string.IsNullOrEmpty(fen))
            {
                fen = GeneralReference.Starting_FEN_Position;
            }
            var fenRecord = FenFactory.Create(fen);
            if (fenRecord == null)
            {
                OperationResult<GameState>.Fail("Bad fen.");
            }
            return hydrateGameState(fenRecord);
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
        public OperationResult<GameState> MakeMove(GameState gameState, int piecePosition, int newPiecePosition, PieceType? piecePromotionType = null)
        {
            var stateInfo = this.getStateInfo(gameState, piecePosition, newPiecePosition, piecePromotionType);
            if (stateInfo.Failure)
            {
                return OperationResult<GameState>.Fail(stateInfo.Message);
            }
            return this.makeMove(gameState, piecePosition, stateInfo.Result, newPiecePosition);
        }

        public OperationResult<GameState> MakeMove(GameState gameState, string beginning, string destination, PieceType? piecePromotionType = null)
        {
            var pos1 = NotationEngine.CoordinateToPosition(beginning);
            var pos2 = NotationEngine.CoordinateToPosition(destination);
            return this.MakeMove(gameState, pos1, pos2, piecePromotionType);
        }

        public OperationResult<GameState> MakeMove(GameState gameState, string pgnMove)
        {
            var pair = _pgnService.PGNMoveToSquarePair(gameState, pgnMove);
            //todo: what about piece promotion?
            if (pair.promotedPiece == '-')
            {
                return this.MakeMove(gameState, pair.piecePosition, pair.newPiecePosition);
            }
            var promotedPieceType = NotationEngine.GetPieceTypeFromCharacter(pair.promotedPiece);
            return this.MakeMove(gameState, pair.piecePosition, pair.newPiecePosition, promotedPieceType);
        }

        private static GameState manageSquares(GameState gameState, StateInfo stateInfo, int piecePosition, int newPiecePosition)
        {
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

        private OperationResult<StateInfo> getStateInfo(GameState gameState, int piecePosition, int newPiecePosition, PieceType? piecePromotionType)
        {
            var square = gameState.Squares.GetSquare(piecePosition);
            if (!square.Occupied)
            {
                return OperationResult<StateInfo>.Fail("Square was empty.");
            }
            if (square.Piece.Color != gameState.ActiveColor)
            {
                return OperationResult<StateInfo>.Fail("Wrong team.");
            }
            var moveInfoResult = _moveService.GetStateInfo(gameState, piecePosition, newPiecePosition);
            if (moveInfoResult.Failure)
            {
                return OperationResult<StateInfo>.Fail(moveInfoResult.Message);
            }
            var moveInfo = moveInfoResult.Result;
            if (moveInfo.IsPawnPromotion)
            {
                if (!piecePromotionType.HasValue)
                {
                    return OperationResult<StateInfo>.Fail("Must provide pawn promotion piece type in order to promote a pawn.");
                }
                moveInfo.PawnPromotedTo = piecePromotionType.Value;
            }
            //var putsOwnKingInCheck = false;
            if (moveInfo.IsCheck)
            {
                return OperationResult<StateInfo>.Fail("Must move out of check. Must not move into check.");
            }
            return OperationResult<StateInfo>.Ok(moveInfo);
        }

        private OperationResult<GameState> hydrateGameState(Snapshot fenRecord, string errorMessage = null)
        {
            if (!string.IsNullOrEmpty(errorMessage))
            {
                return OperationResult<GameState>.Fail(errorMessage);
            }
            var gameState = new GameState(fenRecord);
            gameState.Squares = _notationService.GetSquaresFromFEN_Record(gameState);
            gameState.Attacks = _attackService.GetAttacks(gameState).ToList();
            gameState.StateInfo = _moveService.GetStateInfo(gameState);
            return OperationResult<GameState>.Ok(gameState);
        }

        private OperationResult<GameState> makeMove(GameState gameState, int piecePosition, StateInfo stateInfo, int newPiecePosition)
        {
            //verify that the move can be made
            var verifiedMove = verifyMove(gameState, piecePosition, newPiecePosition);
            if (verifiedMove.Failure)
            {
                return OperationResult<GameState>.Fail(verifiedMove.Message);
            }

            //make the move
            var newGameState = GetNewGameState(gameState, piecePosition, stateInfo, newPiecePosition);

            //Setup new gamestate
            //var newGameStateResult = hydrateGameState(FenFactory.Create(newGameStateFENandPGN.fen));
            //if (newGameStateResult.Failure)
            //{
            //    throw new System.Exception(newGameStateResult.Message);
            //}
            //var newGameState = newGameStateResult.Result;
            //newGameState.PGN = newGameStateFENandPGN.pgn;
            //newGameState.History = fenRecords;
            //make sure we moved out of check.
            if (gameState.StateInfo.IsCheck && newGameState.StateInfo.IsCheck)
            {
                if (gameState.ActiveColor == Color.White)
                {
                    if (newGameState.StateInfo.IsWhiteCheck)
                    {
                        return OperationResult<GameState>.Fail("King must move out of check.");
                    }
                }
                else
                {
                    if (newGameState.StateInfo.IsBlackCheck)
                    {
                        return OperationResult<GameState>.Fail("King must move out of check.");
                    }
                }
            }
            return OperationResult<GameState>.Ok(newGameState);
        }

        private OperationResult verifyMove(GameState gameState, int piecePosition, int newPiecePosition)
        {
            var oldSquare = gameState.Squares.GetSquare(piecePosition);
            var attacks = gameState.Attacks.GetPositionAttacksOnPosition(piecePosition, newPiecePosition);
            if (!attacks.Any())
            {
                return OperationResult.Fail($"Can't find an attack by this piece ({ oldSquare.Index } : { oldSquare.Piece.PieceType }) on this position ({ newPiecePosition }).");
            }
            var badPawnAttacks = attacks.Where(a =>
                                            a.AttackingSquare.Index == piecePosition
                                            && a.Index == newPiecePosition
                                            && a.Piece == null
                                            && a.MayOnlyMoveHereIfOccupiedByEnemy
                                        );
            if (badPawnAttacks.Any())
            {
                if (
                    badPawnAttacks.Count() > 1
                    || gameState.EnPassantTargetSquare == "-"
                    || newPiecePosition != NotationEngine.CoordinateToPosition(gameState.EnPassantTargetSquare)
                )
                {
                    return OperationResult.Fail($"This piece can only move here if the new square is occupied. ({ oldSquare.Index } : { oldSquare.Piece.PieceType }) on this position ({ newPiecePosition }).");
                }
            }
            return OperationResult.Ok();
        }

        private GameState GetNewGameState(GameState gameState, int piecePosition, StateInfo stateInfo, int newPiecePosition)
        {
            var transitionGameState = manageSquares(gameState, stateInfo, piecePosition, newPiecePosition);
            if (stateInfo.IsCastle)
            {
                var rookPositions = CastlingEngine.GetRookPositionsForCastle(gameState.ActiveColor, piecePosition, newPiecePosition);
                transitionGameState = manageSquares(transitionGameState, stateInfo, rookPositions.RookPos, rookPositions.NewRookPos);
            }
            if (stateInfo.IsEnPassant)
            {
                //remove the attacked pawn
                var enPassantAttackedPawnPosition = gameState.ActiveColor == Color.White ? newPiecePosition - 8 : newPiecePosition + 8;
                var enPassantAttackedPawnSquare = transitionGameState.Squares.GetSquare(enPassantAttackedPawnPosition);
                enPassantAttackedPawnSquare.Piece = null;
            }
            _notationService.SetGameStateSnapshot(gameState, transitionGameState, stateInfo, piecePosition, newPiecePosition);

            var fenRecords = gameState.History.DeepCopy();
            var previousStateFEN = gameState.ToString();
            fenRecords.Add(FenFactory.Create(previousStateFEN));
            transitionGameState.History = fenRecords;

            return transitionGameState;
        }
    }
}