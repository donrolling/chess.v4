using Chess.v4.Engine.Extensions;
using Chess.v4.Engine.Interfaces;
using Chess.v4.Engine.Utility;
using Chess.v4.Models;
using Chess.v4.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Chess.v4.Engine.Service
{
    public class CheckmateService : ICheckmateService
    {
        //kings don't count here
        private static List<PieceType> diagonalAttackers = new List<PieceType> { PieceType.Queen, PieceType.Pawn, PieceType.Bishop };
        //kings don't count here
        private static List<PieceType> orthogonalAttackers = new List<PieceType> { PieceType.Queen, PieceType.Rook };

        private readonly IOrthogonalService _orthogonalService;

        public CheckmateService(IOrthogonalService orthogonalService)
        {
            _orthogonalService = orthogonalService;
        }

        public bool IsCheckMate(GameState gameState, Color kingColor, IEnumerable<AttackedSquare> attacksOnKing)
        {
            //find king moves. If there are valid king moves, then this isn't checkmate
            var _kingMovesExist = kingMovesExist(gameState, kingColor, attacksOnKing);
            if (_kingMovesExist) { return false; }
            //find interpositions and attacks that could break check.
            var teamAttacks = gameState.Attacks.Where(a => a.AttackingSquare.Piece.Color == kingColor);
            var _interpositionsExist = this.interpositionsExist(attacksOnKing, teamAttacks);
            if (_interpositionsExist) { return false; }

            var _checkBreakingAttacksExist = this.checkBreakingAttacksExist(attacksOnKing, teamAttacks);
            if (_checkBreakingAttacksExist) { return false; }

            return true;
        }

        private bool checkBreakingAttacksExist(IEnumerable<AttackedSquare> attacksOnKing, IEnumerable<AttackedSquare> teamAttacks)
        {
            var attackCount = attacksOnKing.Count();
            if (attackCount > 1)
            {
                return false;
            }
            var attackOnKing = attacksOnKing.First();
            return teamAttacks.Any(a => a.Index == attackOnKing.Index);
        }

        //private List<int> getEntireDiagonalLine(int pos1, int pos2)
        //{
        //    //diagonal moves: rise LtoR /  or RtoL \
        //    var diff = Math.Abs(pos1 - pos2);
        //    var ltr = diff % 9 == 0;
        //    var rtl = diff % 7 == 0;
        //    if (!ltr && !rtl)
        //    {
        //        throw new Exception("What? This is supposed to be diagonal.");
        //    }
        //    var dxs = new List<int> { pos1, pos2 };
        //    //smallest # will be closest to the left or right in the line
        //    //the left terminating position is evenly divisible by 8
        //    //the right terminating position is evently divisible by 7
        //    //find terminators
        //    //left
        //    var increment = ltr ? 9 : 7;
        //    var smallest = dxs.Min();
        //    var largest = dxs.Max();
        //    var nextSmallest = smallest;
        //    while (nextSmallest % 8 > 0 && nextSmallest >= 0)
        //    {
        //        nextSmallest = nextSmallest - increment;
        //        if (nextSmallest >= 0 && !dxs.Contains(nextSmallest))
        //        {
        //            dxs.Add(nextSmallest);
        //        }
        //    };
        //    //right
        //    var nextLargest = largest;
        //    while (nextLargest % 7 > 0 && nextLargest <= 63)
        //    {
        //        nextLargest = nextLargest + increment;
        //        if (nextLargest <= 63 && !dxs.Contains(nextLargest))
        //        {
        //            dxs.Add(nextLargest);
        //        }
        //    };
        //    //fill in the middle
        //    var mid = smallest;
        //    var nextMid = mid;
        //    if (diff > increment)
        //    {
        //        while (nextMid < largest)
        //        {
        //            nextMid = nextMid + increment;
        //            if (!dxs.Contains(nextMid))
        //            {
        //                dxs.Add(nextMid);
        //            }
        //        }
        //    }
        //    return dxs;
        //}

        private List<int> getEntireOrthogonalLine(bool isRankMove, AttackedSquare x, bool trim = false)
        {
            List<int> result;
            if (isRankMove)
            {
                var file = NotationUtility.PositionToFile(x.Index);
                result = this._orthogonalService.GetEntireFile(file);
            }
            else
            {
                var rank = NotationUtility.PositionToRank(x.Index);
                result = this._orthogonalService.GetEntireRank(rank);
            }
            if (!trim)
            {
                return result;
            }
            var low = x.Index > x.AttackingSquare.Index ? x.AttackingSquare.Index : x.Index;
            var high = x.Index < x.AttackingSquare.Index ? x.AttackingSquare.Index : x.Index;
            return result.Where(a => a > low && a < high).ToList();
        }

        private Square getKing(GameState gameState, Color color)
        {
            return gameState.Squares.Where(a => a.Occupied && a.Piece.Color == color && a.Piece.PieceType == PieceType.King).First();
        }

        private int[] getKingCastleCoordinates(Square kingSquare, int destination)
        {
            switch (kingSquare.Piece.Color)
            {
                case Color.Black:
                    switch (destination)
                    {
                        case 58:
                            return new int[2] { 61, 62 };

                        case 62:
                            return new int[2] { 58, 59 };

                        default:
                            throw new Exception("Invalid destination.");
                    }

                case Color.White:
                    switch (destination)
                    {
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

        /// <summary>
        /// In the future, I should be passing back all the moves rather than a boolean
        /// that way the engine can have metadata about all the possible moves that
        /// could be taken and maybe offer a suggestion.
        /// </summary>
        /// <param name="attacksOnKing"></param>
        /// <param name="teamAttacks"></param>
        /// <returns></returns>
        private bool interpositionsExist(IEnumerable<AttackedSquare> attacksOnKing, IEnumerable<AttackedSquare> teamAttacks)
        {
            foreach (var attackOnKing in attacksOnKing)
            {
                var attackIsOrthogonal = GeneralUtility.IsOrthogonal(attackOnKing.AttackingSquare.Index, attackOnKing.Index);
                var attackIsDiagonal = DiagonalUtility.IsDiagonal(attackOnKing.AttackingSquare.Index, attackOnKing.Index);
                var range = new List<Square>();
                if (attackIsOrthogonal)
                {
                    var isRankMove = GeneralUtility.GivenOrthogonalMove_IsItARankMove(attackOnKing.AttackingSquare.Index, attackOnKing.Index);
                    var oxs = getEntireOrthogonalLine(isRankMove, attackOnKing, true);
                    var ixs = teamAttacks.Select(a => a.Index).Intersect(oxs);
                    if (ixs.Any())
                    {
                        return true;
                    }
                }
                else if (attackIsDiagonal)
                {
                    var dxs = DiagonalUtility.GetDiagonalLine(attackOnKing.AttackingSquare.Index, attackOnKing.Index);
                    //if dxs contains the clearMove.Index, then the king has not moved out of check
                    var ixs = teamAttacks.Select(a => a.Index).Intersect(dxs);
                    if (ixs.Any())
                    {
                        return true;
                    }
                }
                //else it could be a knight attack, but no interpositions would exist
            }
            return false;
        }

        private bool kingMovesExist(GameState gameState, Color kingColor, IEnumerable<AttackedSquare> attacksOnKing)
        {
            var opponentAttacks = gameState.Attacks.Where(a => a.AttackingSquare.Piece.Color == kingColor.Reverse()).ToList();
            var kingMoves = gameState.Attacks
                                .Where(a =>
                                    a.AttackingSquare.Piece.PieceType == PieceType.King
                                    && a.AttackingSquare.Piece.Color == kingColor
                                    && !a.IsProtecting
                                ).ToList();
            var clearMoves = kingMoves.Select(a => a.Index).Except(opponentAttacks.Select(b => b.Index));
            if (!clearMoves.Any()) { return false; }
            //todo: there is a bug here: when the king is checked like so (5rk1/5pbp/5Qp1/8/8/8/5PPP/3q2K1 w - - 0 1)
            //he is boxed in, but h1 looks a valid move for him because the Queen doesn't currently have it as an attack square
            //because the King is blocking it.
            //fix this by examining if the king is moving orthoganally or diagonally and determine if any attackers are on that line as well
            var clearMoveCount = clearMoves.Count();
            foreach (var clearMoveIndex in clearMoves)
            {
                //clearMove.AttackerSquare is the king here
                //clearMove.Index is where he is going
                var clearMove = kingMoves.GetSquare(clearMoveIndex);
                var isOrthogonal = GeneralUtility.IsOrthogonal(clearMove.AttackingSquare.Index, clearMove.Index);
                var _mayKingMoveHere = false;
                if (isOrthogonal)
                {
                    _mayKingMoveHere = this.mayKingMoveOrthogonallyHere(clearMove, attacksOnKing, gameState.Attacks);
                    if (!_mayKingMoveHere)
                    {
                        clearMoveCount--;
                    }
                }
                else
                {
                    //has to be diagonal
                    _mayKingMoveHere = this.mayKingMoveDiagonallyHere(clearMove, attacksOnKing, gameState.Attacks);
                    if (!_mayKingMoveHere)
                    {
                        clearMoveCount--;
                    }
                }
            }
            return clearMoveCount > 0;
        }

        private bool mayKingMoveDiagonallyHere(AttackedSquare clearMove, IEnumerable<AttackedSquare> attacksOnKing, List<AttackedSquare> attacks)
        {
            var kingSquare = (Square)attacksOnKing.First();
            //need to detect if we're moving into check
            var anybodyAttackingThisSquare = attacks.Any(a => a.Index == clearMove.Index && a.AttackingSquare.Piece.Color != kingSquare.Piece.Color);
            if (anybodyAttackingThisSquare)
            {
                return false;
            }
            var diagonalAttacksOnKing = attacksOnKing.Where(a =>
                diagonalAttackers.Contains(a.AttackingSquare.Piece.PieceType)
                && DiagonalUtility.IsDiagonal(a.Index, a.AttackingSquare.Index)
            );
            if (!diagonalAttacksOnKing.Any()) { return true; }
            foreach (var x in diagonalAttacksOnKing)
            {
                var dxs = DiagonalUtility.GetDiagonalLine(x.Index, x.AttackingSquare.Index);
                //if dxs contains the clearMove.Index, then the king has not moved out of check
                if (dxs.Contains(clearMove.Index))
                {
                    return false;
                }
            }
            return true;
        }

        private bool mayKingMoveOrthogonallyHere(AttackedSquare clearMove, IEnumerable<AttackedSquare> attacksOnKing, List<AttackedSquare> attacks)
        {
            var kingSquare = (Square)attacksOnKing.First();
            //need to detect if we're moving into check
            var anybodyAttackingThisSquare = attacks.Any(a => a.Index == clearMove.Index && a.AttackingSquare.Piece.Color != kingSquare.Piece.Color);
            if (anybodyAttackingThisSquare)
            {
                return false;
            }
            //now make sure we're not ignoring the issue where an attack isn't displayed because the king was blocking the square
            //that he would move into, that is still being attacked by the original attacker.
            var isRankMove = GeneralUtility.GivenOrthogonalMove_IsItARankMove(clearMove.AttackingSquare.Index, clearMove.Index);
            //find all attackers who attack orthogonally and determine if they are on the same line
            var orthogonalAttacksOnKing = attacksOnKing.Where(a => orthogonalAttackers.Contains(a.AttackingSquare.Piece.PieceType));
            foreach (var x in orthogonalAttacksOnKing)
            {
                var oxs = getEntireOrthogonalLine(isRankMove ? false : true, x);
                //if oxs contains the clearMove.Index, then the king has not moved out of check
                if (oxs.Contains(clearMove.Index))
                {
                    return false;
                }
            }
            return true;
        }
    }
}