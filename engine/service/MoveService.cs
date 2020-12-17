using Chess.v4.Engine.Extensions;
using Chess.v4.Engine.Interfaces;
using Chess.v4.Engine.Utility;
using Chess.v4.Models;
using Chess.v4.Models.Enums;
using Common.Responses;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Chess.v4.Engine.Service
{
    public class MoveService : IMoveService
    {
        private readonly ICheckmateService _checkmateService;

        public MoveService(ICheckmateService checkmateService)
        {
            _checkmateService = checkmateService;
        }

        public OperationResult<StateInfo> GetStateInfo(GameState gameState, int piecePosition, int newPiecePosition)
        {
            var newActiveColor = gameState.ActiveColor.Reverse();
            var stateInfo = new StateInfo();
            var oldSquare = gameState.Squares.GetSquare(piecePosition);

            var isValidCastleAttempt = this.IsValidCastleAttempt(gameState, oldSquare, newPiecePosition, gameState.Attacks);
            if (isValidCastleAttempt.Success)
            {
                stateInfo.IsCastle = isValidCastleAttempt.Result;
            }
            else
            {
                return OperationResult<StateInfo>.Fail(isValidCastleAttempt.Message);
            }

            stateInfo.IsPawnPromotion = this.isPawnPromotion(oldSquare, newPiecePosition);
            if (gameState.EnPassantTargetSquare != "-" && oldSquare.Piece.PieceType == PieceType.Pawn)
            {
                var pawnEnPassantPosition = NotationEngine.CoordinateToPosition(gameState.EnPassantTargetSquare);
                if (newPiecePosition == pawnEnPassantPosition)
                {
                    stateInfo.IsEnPassant = true;
                }
            }

            var isResign = false;
            var isDraw = false;
            // todo: i don't think we can get here
            // also, this is incomplete
            if (isDraw || isResign)
            {
                if (isDraw)
                {
                    var score = string.Concat(" ", "1/2-1/2");
                    gameState.PGN += score;
                }
                if (isResign)
                {
                    var score = string.Concat(" ", newActiveColor == Color.White ? "1-0" : "0-1");
                    gameState.PGN += score;
                }
            }

            return OperationResult<StateInfo>.Ok(stateInfo);
        }

        public StateInfo GetStateInfo(GameState gameState)
        {
            var stateInfo = new StateInfo();
            var whiteKingAttacks = getAttacksOnKing(gameState, Color.White);
            stateInfo.IsWhiteCheck = whiteKingAttacks.Any();
            if (stateInfo.IsWhiteCheck)
            {
                var isCheckMate = this._checkmateService.IsCheckMate(gameState, Color.White, whiteKingAttacks);
                if (isCheckMate)
                {
                    stateInfo.IsCheckmate = true;
                    stateInfo.Result = "0-1";
                }
            }
            else
            {
                var blackKingAttacks = getAttacksOnKing(gameState, Color.Black);
                stateInfo.IsBlackCheck = blackKingAttacks.Any();
                if (stateInfo.IsBlackCheck)
                {
                    var isCheckMate = this._checkmateService.IsCheckMate(gameState, Color.Black, blackKingAttacks);
                    if (isCheckMate)
                    {
                        stateInfo.IsCheckmate = true;
                        stateInfo.Result = "1-0";
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
        public bool HasThreefoldRepition(GameState gameState)
        {
            return gameState.History
                    .GroupBy(a => new { a.PiecePlacement, a.CastlingAvailability, a.EnPassantTargetPosition })
                    .Where(a => a.Count() >= 3)
                    .Any();
        }

        // duplicated code?
        //public bool IsDiagonalMove(int startPosition, int endPosition)
        //{
        //    var startMod = startPosition % 8;
        //    var endMod = endPosition % 8;
        //    var modDiff = Math.Abs(startMod - endMod);

        //    var startRow = NotationUtility.PositionToRankInt(startPosition);
        //    var endRow = NotationUtility.PositionToRankInt(endPosition);
        //    var rowDiff = Math.Abs(startRow - endRow);
        //    if (modDiff == rowDiff)
        //    {
        //        return true;
        //    }
        //    return false;
        //}

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
        public bool IsEnPassant(Square square, int newPiecePosition, string enPassantTargetSquare)
        {
            if (enPassantTargetSquare == "-")
            {
                return false;
            }
            var piece = square.Piece;
            if (piece.PieceType != PieceType.Pawn) { return false; } //only pawns can perform en passant
            var enPassantPosition = NotationEngine.CoordinateToPosition(enPassantTargetSquare);
            if (enPassantPosition != newPiecePosition) { return false; } //if we're not moving to the en passant position, this is not en passant
            var moveDistance = Math.Abs(square.Index - newPiecePosition);
            if (!new List<int> { 7, 9 }.Contains(moveDistance)) { return false; } //is this a diagonal move?
            if (piece.Color == Color.Black && square.Index < newPiecePosition) { return false; } //black can't move up
            if (piece.Color == Color.White && square.Index > newPiecePosition) { return false; } //white can't move down
            return true;
        }

        //	//fix enemyKingAttacks. trying to figure out the moves that the king can make
        //	var enemyKingAttacks = squares;
        public bool IsRealCheck(List<Square> squares, IEnumerable<AttackedSquare> attacksThatCheck, Color activeColor, int kingSquare)
        {
            if (attacksThatCheck == null || !attacksThatCheck.Any())
            {
                return false;
            }
            if (attacksThatCheck.Count() > 1)
            {
                //if there are more than one, then this is real
                return true;
            }
            var key = attacksThatCheck.First().Index;
            var attackingPiece = squares.GetPiece(key);
            //this code is here to remove the possibility that the king is said to be in check by an enemy pawn when he is directly in front of the pawn
            if (attackingPiece.PieceType == PieceType.Pawn)
            {
                var onSameFile = (key % 8) == (kingSquare % 8) ? true : false;
                return !onSameFile;
            }
            return true;
        }

        //	var friendlyAttacks = (activeColor == Color.White ? whiteAttacks : blackAttacks);
        //	var opponentAttacks = (activeColor == Color.White ? blackAttacks : whiteAttacks);
        public OperationResult<bool> IsValidCastleAttempt(GameState gameState, Square square, int destination, IEnumerable<AttackedSquare> attackedSquares)
        {
            var piece = square.Piece;

            var isCastleAttempt =
                (square.Index == 4 || square.Index == 60)
                && piece.PieceType == PieceType.King
                && Math.Abs(square.Index - destination) == 2;

            //if not a castle, then no validation needed.
            if (!isCastleAttempt)
            {
                return OperationResult<bool>.Ok(isCastleAttempt);
            }

            var castleInfo = checkCastleAvailability(gameState, destination, piece);
            if (!castleInfo.CastleAvailability)
            {
                return OperationResult<bool>.Fail("Castling is not available.");
            }

            //validate the move
            if (gameState.StateInfo.IsCheck)
            {
                return OperationResult<bool>.Fail("Can't castle out of check.");
            }

            var castleThroughCheck = CastlingEngine.DetermineCastleThroughCheck(gameState, square.Index, castleInfo.RookPosition);
            if (castleThroughCheck)
            {
                return OperationResult<bool>.Fail("Can't castle through check.");
            }

            return OperationResult<bool>.Ok(isCastleAttempt);
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
        public bool IsValidPawnMove(Square currentSquare, List<Square> squares, Color color, int piecePosition, int newPiecePosition, bool isEnPassant)
        {
            var isDiagonalMove = DiagonalEngine.IsDiagonal(currentSquare.Index, newPiecePosition);
            if (!isDiagonalMove)
            {
                return true;
            }
            var pieceToCapture = squares.GetSquare(newPiecePosition).Piece;
            var isCapture = pieceToCapture != null;
            return isCapture || isEnPassant;
        }

        private (bool CastleAvailability, int RookPosition) checkCastleAvailability(GameState gameState, int destination, Piece piece)
        {
            if (gameState.CastlingAvailability == "-")
            {
                return (false, 0);
            }
            switch (piece.Color)
            {
                case Color.Black:
                    switch (destination)
                    {
                        case 58:
                            return (gameState.CastlingAvailability.Contains("q", StringComparison.CurrentCulture), 56);

                        case 62:
                            return (gameState.CastlingAvailability.Contains("k", StringComparison.CurrentCulture), 63);

                        default:
                            throw new Exception("Invalid destination.");
                    }

                case Color.White:
                    switch (destination)
                    {
                        case 2:
                            return (gameState.CastlingAvailability.Contains("Q", StringComparison.CurrentCulture), 0);

                        case 6:
                            return (gameState.CastlingAvailability.Contains("K", StringComparison.CurrentCulture), 7);

                        default:
                            throw new Exception("Invalid destination.");
                    }

                default:
                    throw new Exception("Enum not matched.");
            }
        }

        private IEnumerable<AttackedSquare> getAttacksOnKing(GameState gameState, Color color)
        {
            return gameState.Attacks
                            .Where(a =>
                                a.Occupied
                                && a.Piece.Color == color
                                && a.Piece.PieceType == PieceType.King
                                && !a.IsProtecting
                            );
        }

        private bool isPawnPromotion(Square square, int newPiecePosition)
        {
            if (square.Piece.PieceType != PieceType.Pawn)
            {
                return false;
            }
            if (square.Piece.Color == Color.White && newPiecePosition >= 56 && newPiecePosition <= 63)
            {
                return true;
            }
            if (square.Piece.Color == Color.Black && newPiecePosition >= 0 && newPiecePosition <= 7)
            {
                return true;
            }
            return false;
        }
    }
}