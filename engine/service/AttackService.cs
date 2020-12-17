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
    public class AttackService : IAttackService
    {
        private static readonly List<PieceType> _dangerPieceTypes = new List<PieceType> { PieceType.Queen, PieceType.Rook, PieceType.Bishop };
        private static readonly List<PieceType> _diagonalDangerPieceTypes = new List<PieceType> { PieceType.Queen, PieceType.Bishop };
        private static readonly List<PieceType> _orthogonalFangerPieceTypes = new List<PieceType> { PieceType.Queen, PieceType.Rook };
        
        private readonly IOrthogonalService _orthogonalService;

        public AttackService(IOrthogonalService orthogonalService)
        {
            _orthogonalService = orthogonalService;
        }

        public IEnumerable<AttackedSquare> GetAttacks(GameState gameState)
        {
            var accumulator = new List<AttackedSquare>();
            foreach (var square in gameState.Squares.Where(a => a.Occupied).OrderBy(a => a.Piece.OrderOfOperation))
            {
                this.getPieceAttacks(gameState, square, accumulator);
            }
            trimKingMoves(accumulator);
            return accumulator;
        }

        private void getKingAttacks(GameState gameState, Square square, List<AttackedSquare> accumulator)
        {
            var attacks = new List<AttackedSquare>();
            var squares = gameState.Squares;
            var offsets = getKingOffsets(square.Index);
            var pieceColor = square.Piece.Color;
            foreach (var offset in offsets)
            {
                var destinationPosition = square.Index + offset;
                var isValidCoordinate = GeneralUtility.IsValidCoordinate(destinationPosition);
                if (!isValidCoordinate)
                {
                    continue;
                }
                var _isValidMove = isValidMove(accumulator, gameState, square, destinationPosition, pieceColor);
                if (!_isValidMove.IsValid)
                {
                    continue;
                }
                attacks.Add(
                    new AttackedSquare(square, squares.GetSquare(destinationPosition), isProtecting: !_isValidMove.CanAttackOccupyingPiece)
                );
            }

            //*********************
            //castling stuff
            //*********************
            var castleAvailability = gameState.CastlingAvailability;
            if (pieceColor == Color.White)
            {
                if (castleAvailability.Contains("K"))
                {
                    addCastleAttackIfPossible(
                        square,
                        attacks,
                        squares,
                        7,
                        new List<int> { 5, 6 },
                        6
                    );
                }
                if (castleAvailability.Contains("Q"))
                {
                    addCastleAttackIfPossible(
                        square,
                        attacks,
                        squares,
                        0,
                        new List<int> { 1, 2, 3 },
                        2
                    );
                }
            }
            else if (pieceColor == Color.Black)
            {
                if (castleAvailability.Contains("k"))
                {
                    addCastleAttackIfPossible(
                        square,
                        attacks,
                        squares,
                        63,
                        new List<int> { 61, 62 },
                        62
                    );
                }
                if (castleAvailability.Contains("q"))
                {
                    addCastleAttackIfPossible(
                        square,
                        attacks,
                        squares,
                        56,
                        new List<int> { 57, 58, 59 },
                        58
                    );
                }
            }
            //*********************
            //end castling stuff
            //*********************

            if (!attacks.Any())
            {
                return;
            }
            accumulator.AddRange(attacks);
        }

        private static void addCastleAttackIfPossible(Square square, List<AttackedSquare> attacks, List<Square> squares, int rookPos, List<int> castlingClearPositions, int attackToAdd)
        {
            var areSquaresClear = squares.Count(
                                    a => castlingClearPositions.Contains(a.Index)
                                    && !a.Occupied
                                ) == castlingClearPositions.Count();
            if (areSquaresClear)
            {
                var rookSquare = squares.GetSquare(rookPos);
                if (rookSquare.Occupied && rookSquare.Piece.PieceType == PieceType.Rook)
                {
                    attacks.Add(new AttackedSquare(square, squares.GetSquare(attackToAdd)));
                }
            }
        }

        private static List<int> getKingOffsets(int position)
        {
            var offsets = new List<int> { -9, -8, -7, -1, 1, 7, 8, 9 };
            if (position % 8 == 0)
            {
                offsets.Remove(-1);
                offsets.Remove(-9);
                offsets.Remove(7);
            }
            if (position % 8 == 7)
            {
                offsets.Remove(1);
                offsets.Remove(9);
                offsets.Remove(-7);
            }

            return offsets;
        }

        private static void trimKingMoves(List<AttackedSquare> accumulator)
        {
            //trim king moves now that all moves have been calculated.
            var removeableAttacks = new List<AttackedSquare>();
            foreach (var color in new List<Color> { Color.Black, Color.White })
            {
                var kingAttacks = accumulator
                                    .Where(a =>
                                        a.AttackingSquare.Piece.PieceType == PieceType.King
                                        && a.AttackingSquare.Piece.Color == color
                                    );
                var conflictingAttacks = from a in accumulator
                                         join k in kingAttacks on a.Index equals k.Index
                                         where
                                            a.AttackingSquare.Piece.Color == color.Reverse()
                                            && !a.IsPassiveAttack
                                         select a;
                if (conflictingAttacks.Any())
                {
                    var conflictingIndexes = conflictingAttacks.Select(a => a.Index);
                    var rs = kingAttacks.Where(a => conflictingIndexes.Contains(a.Index));
                    removeableAttacks.AddRange(rs);
                }
            }
            foreach (var removeableAttack in removeableAttacks)
            {
                accumulator.Remove(removeableAttack);
            }
        }

        private void getKnightAttacks(GameState gameState, Square square, List<AttackedSquare> accumulator)
        {
            var squares = gameState.Squares;
            var currentPosition = square.Index;
            var pieceColor = square.Piece.Color;
            var attacks = new List<AttackedSquare>();
            var coord = NotationUtility.PositionToCoordinate(currentPosition);
            var file = NotationUtility.FileToInt(coord[0]);
            var rank = (int)coord[1];
            var potentialPositions = new List<int> { 6, 10, 15, 17, -6, -10, -15, -17 };
            foreach (var potentialPosition in potentialPositions)
            {
                var position = currentPosition + potentialPosition;
                var _isValidKnightMove = isValidKnightMove(currentPosition, position, file, rank);
                var _isValidMove = isValidMove(accumulator, gameState, square, position, pieceColor);
                var _isValidCoordinate = GeneralUtility.IsValidCoordinate(position);

                if (!_isValidKnightMove || !_isValidMove.IsValid || !_isValidCoordinate) { continue; }

                var attackedSquare = squares.GetSquare(position);
                if (!attackedSquare.Occupied)
                {
                    attacks.Add(new AttackedSquare(square, attackedSquare));
                }
                else
                {
                    attacks.Add(new AttackedSquare(square, attackedSquare, isProtecting: attackedSquare.Piece.Color == pieceColor));
                }
            }
            if (attacks.Any())
            {
                accumulator.AddRange(attacks);
            }
        }

        private void getPawnAttacks(GameState gameState, Square square, List<AttackedSquare> accumulator)
        {
            var squares = gameState.Squares;
            var position = square.Index;
            var pieceColor = square.Piece.Color;
            var coord = NotationUtility.PositionToCoordinate(position);
            int file = NotationUtility.FileToInt(coord[0]);
            int rank = NotationUtility.PositionToRankInt(position);

            var directionIndicator = pieceColor == Color.White ? 1 : -1;
            var homeRankIndicator = pieceColor == Color.White ? 2 : 7;

            var nextRank = (rank + directionIndicator);
            var aheadOneRankPosition = NotationUtility.CoordinatePairToPosition(file, nextRank);
            var aheadOneRankSquare = squares.GetSquare(aheadOneRankPosition);
            var attacks = new List<AttackedSquare>();
            if (!aheadOneRankSquare.Occupied)
            {
                //can't attack going forward
                attacks.Add(new AttackedSquare(square, aheadOneRankSquare, true));
            }

            managePawnAttacks(squares, square, pieceColor, file, rank, directionIndicator, homeRankIndicator, nextRank, attacks, aheadOneRankSquare.Occupied);

            //add en passant position: -1 indicates null here
            if (gameState.EnPassantTargetSquare != "-")
            {
                var enPassantTargetPosition = NotationUtility.CoordinateToPosition(gameState.EnPassantTargetSquare);
                var leftPos = NotationUtility.CoordinatePairToPosition(file - 1, nextRank);
                var rightPos = NotationUtility.CoordinatePairToPosition(file + 1, nextRank);
                if (enPassantTargetPosition == leftPos || enPassantTargetPosition == rightPos)
                {
                    var enPassantSquare = squares.GetSquare(enPassantTargetPosition);
                    attacks.Add(new AttackedSquare(square, enPassantSquare));
                }
            }
            if (attacks.Any())
            {
                accumulator.AddRange(attacks);
            }
        }

        private void getPawnDiagonalAttack(List<Square> squares, Square square, Color pieceColor, int fileIndicator, int nextRank, List<AttackedSquare> attacks)
        {
            var pos = NotationUtility.CoordinatePairToPosition(fileIndicator, nextRank);
            var attackedSquare = squares.GetSquare(pos);
            if (attackedSquare.Occupied && attackedSquare.Piece.Color != pieceColor)
            {
                attacks.Add(new AttackedSquare(square, attackedSquare, false, true));
            }
            else
            {
                attacks.Add(new AttackedSquare(square, attackedSquare, false, true, true));
            }
        }

        private void getPieceAttacks(GameState gameState, Square square, List<AttackedSquare> accumulator)
        {
            switch (square.Piece.PieceType)
            {
                case PieceType.Pawn:
                    getPawnAttacks(gameState, square, accumulator);
                    break;

                case PieceType.Knight:
                    getKnightAttacks(gameState, square, accumulator);
                    break;

                case PieceType.Bishop:
                    DiagonalUtility.GetDiagonals(gameState, square, accumulator);
                    break;

                case PieceType.Rook:
                    this._orthogonalService.GetOrthogonals(gameState, square, accumulator);
                    break;

                case PieceType.Queen:
                    this._orthogonalService.GetOrthogonals(gameState, square, accumulator);
                    DiagonalUtility.GetDiagonals(gameState, square, accumulator);
                    break;

                case PieceType.King:
                    getKingAttacks(gameState, square, accumulator);
                    break;

                default:
                    throw new Exception("Mismatched Enum!");
            }
        }

        private bool isValidKnightMove(int position, int tempPosition, int file, int rank)
        {
            var isValidCoordinate = GeneralUtility.IsValidCoordinate(tempPosition);
            if (!isValidCoordinate)
            {
                return false;
            }

            var tempCoord = NotationUtility.PositionToCoordinate(tempPosition);
            var tempFile = NotationUtility.FileToInt(tempCoord[0]);
            var tempRank = (int)tempCoord[1];

            var fileDiff = Math.Abs(tempFile - file);
            var rankDiff = Math.Abs(tempRank - rank);

            if (fileDiff > 2 || fileDiff < 1)
            {
                return false;
            }
            if (rankDiff > 2 || rankDiff < 1)
            {
                return false;
            }

            return true;
        }

        private (bool IsValid, bool CanAttackOccupyingPiece) isValidMove(List<AttackedSquare> attacks, GameState gameState, Square locationSquare, int destination, Color pieceColor)
        {
            var isValidCoordinate = GeneralUtility.IsValidCoordinate(destination);
            if (!isValidCoordinate || !gameState.Squares.Any(a => a.Index == destination))
            {
                // throw new Exception($"Invalid position passed: { position }");
                // probably doing this just as an easy way to not have to trim squares from every algorithm
                return (false, false);
            }
            var destinationSquare = gameState.Squares.GetSquare(destination);

            // if you're not a king you've got simpler rules
            // but if you're the king, you still have to obey the basics
            var basics = basicMoveValidity(pieceColor, destinationSquare);
            if (locationSquare.Piece.PieceType != PieceType.King || !basics.CanAttackOccupyingPiece)
            {
                return basics;
            }

            // check to make sure that this move would get us out of check
            var newPositionAttacks = attacks.Where(a => a.Index == destination && a.AttackingSquare.Piece.Color != locationSquare.Piece.Color);
            if (newPositionAttacks.Any())
            {
                // king can't move into check
                return (true, false);
            }
            // no-one is attacking the destination that we can tell, let's make sure
            // piece currently being attacked?
            var attacksOnThisPosition = attacks.Where(a => a.Index == locationSquare.Index && a.AttackingSquare.Piece.Color != locationSquare.Piece.Color);
            if (!attacksOnThisPosition.Any())
            {
                // nobody is attacking this king now, or in the new position
                return (true, true);
            }
            var dangerSquares = attacksOnThisPosition.Where(a => _dangerPieceTypes.Contains(a.AttackingSquare.Piece.PieceType) && a.AttackingSquare.Piece.Color != locationSquare.Piece.Color);
            if (!dangerSquares.Any())
            {
                // nobody dangerous is attacking this king now, or in the new position
                return (true, true);
            }
            // which direction are we moving?
            var directionInfo = GeneralUtility.GetDirectionInfo(locationSquare.Index, destination);
            var directionalDangerPieceTypes = directionInfo.IsDiagonal ? _diagonalDangerPieceTypes : _orthogonalFangerPieceTypes;
            var directionalDangerSquares = attacksOnThisPosition.Where(a => directionalDangerPieceTypes.Contains(a.AttackingSquare.Piece.PieceType) && a.AttackingSquare.Piece.Color != locationSquare.Piece.Color);
            foreach (var dangerSquare in directionalDangerSquares)
            {
                // is that going to get us out of the attack path that this piece would normally have?
                var dangerPieceDirectionInfo = GeneralUtility.GetDirectionInfo(dangerSquare.AttackingSquare.Index, locationSquare.Index);
                if (dangerPieceDirectionInfo.IsDiagonal && directionInfo.IsOrthogonal)
                {
                    // won't intersect paths
                    continue;
                }
                if (dangerPieceDirectionInfo.IsOrthogonal && directionInfo.IsDiagonal)
                {
                    // won't intersect paths
                    continue;
                }

                if (dangerPieceDirectionInfo.IsDiagonal && directionInfo.IsDiagonal)
                {
                    // are we moving on the same diagonal plane?
                    if (dangerPieceDirectionInfo.DiagonalDirection == directionInfo.DiagonalDirection)
                    {
                        // not safe
                        return (false, false);
                    }
                }
                if (dangerPieceDirectionInfo.IsOrthogonal && directionInfo.IsOrthogonal)
                {
                    // are we moving on the same orthogonal plane?
                    if (dangerPieceDirectionInfo.Direction == directionInfo.Direction)
                    {
                        // not safe
                        return (false, false);
                    }
                }
            }
            // safe
            return (true, true);
        }

        private static (bool IsValid, bool CanAttackOccupyingPiece) basicMoveValidity(Color pieceColor, Square square)
        {
            if (!square.Occupied)
            {
                // go for it
                return (true, true);
            }

            // if it is your teammate, you can't do that
            if (GeneralUtility.IsTeamPiece(pieceColor, square.Piece))
            {
                return (true, true);
            }
            else
            {
                return (true, false);
            }
        }

        private void managePawnAttacks(List<Square> squares, Square square, Color pieceColor, int file, int rank, int directionIndicator, int homeRankIndicator, int nextRank, List<AttackedSquare> attacks, bool aheadOneRankSquareOccupied)
        {
            var notOnFarLeftFile = file - 1 >= 0;
            var notOnFarRightFile = file + 1 <= 7;
            if (notOnFarLeftFile)
            {
                //get attack square on left
                var fileIndicator = file - 1;
                getPawnDiagonalAttack(squares, square, pieceColor, fileIndicator, nextRank, attacks);
            }
            if (notOnFarRightFile)
            {
                //get attack square on right
                var fileIndicator = file + 1;
                getPawnDiagonalAttack(squares, square, pieceColor, fileIndicator, nextRank, attacks);
            }
            //can't move ahead two if the one in front of you is blocked.
            if (aheadOneRankSquareOccupied)
            {
                return;
            }
            //have to plus one here because rank is zero based and coordinate is base 1
            //if this pawn is on it's home rank, then add a second attack square.
            var isOnHomeRank = rank + 1 == homeRankIndicator;
            if (isOnHomeRank)
            {
                var forwardOne = nextRank + directionIndicator;
                var rankForwardPosition = NotationUtility.CoordinatePairToPosition(file, forwardOne);
                var rankForwardSquare = squares.GetSquare(rankForwardPosition);
                //pawns don't attack forward, so we don't have attacks when people occupy ahead of us
                if (!rankForwardSquare.Occupied)
                {
                    attacks.Add(new AttackedSquare(square, rankForwardSquare, true));
                }
            }
        }
    }
}