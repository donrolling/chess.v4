using Chess.v4.Engine.Extensions;
using Chess.v4.Engine.Reference;
using Chess.v4.Models;
using Chess.v4.Models.Enums;
using System;
using System.Collections.Generic;

namespace Chess.v4.Engine.Utility
{
    public static class DiagonalEngine
    {
        private static List<int> File = GeneralEngine.GetEntireFile(7);

        public static List<int> GetDiagonalLine(int location, int destination)
        {
            var _isDiagonal = IsDiagonal(location, destination);
            if (!_isDiagonal)
            {
                throw new Exception("You're doing it wrong! GetDiagonalLine has to be used with a diagonal.");
            }
            var direction = getDirectionByMovePositions(location, destination);
            var result = new List<int>();
            var increment = getIteratorByDirectionEnum(direction);
            // pick the higer of the two numbers and go up until just before you reach the destination
            var positionCursorTerminator = location > destination ? location : destination;
            var positionCursor = location + increment;
            while (positionCursor < positionCursorTerminator)
            {
                result.Add(positionCursor);
                positionCursor += increment;
            }
            return result;
        }

        public static List<AttackedSquare> GetDiagonalLine(GameState gameState, Square square, Piece attackingPiece, DiagonalDirection direction)
        {
            var attacks = new List<AttackedSquare>();
            var diagonalLine = getIteratorByDirectionEnum(direction);
            var position = square.Index;
            var attackPosition = square.Index;
            do
            {
                if (!canDoDiagonalsFromStartPosition(position, diagonalLine))
                {
                    break;
                }
                attackPosition = attackPosition + diagonalLine;
                var moveViability = GeneralEngine.DetermineMoveViability(gameState, attackingPiece, attackPosition);
                //I don't think either of these conditions should occur.
                if (!moveViability.IsValidCoordinate || moveViability.SquareToAdd == null)
                {
                    continue;
                }
                var attack = new AttackedSquare(square, moveViability.SquareToAdd, isProtecting: moveViability.IsTeamPiece);
                attacks.Add(attack);
                if (moveViability.SquareToAdd.Occupied)
                {
                    break;
                }
            } while (isValidDiagonalCoordinate(attackPosition));
            return attacks;
        }

        public static void GetDiagonals(GameState gameState, Square square, List<AttackedSquare> accumulator)
        {
            foreach (var direction in GeneralReference.DiagonalLines)
            {
                var attacks = GetDiagonalLine(gameState, square, square.Piece, direction);
                accumulator.AddRange(attacks);
            }
        }

        public static List<int> GetEntireDiagonalByFile(int file, DiagonalDirectionFromFileNumber direction)
        {
            var list = new List<int>();
            var increment = getDiagonalIncrement(direction);
            var numberOfSquares = howFarToGo(file, direction);
            var index = file;
            for (int i = 0; i < numberOfSquares; i++)
            {
                list.Add(index);
                index = index + increment;
            }
            return list;
        }

        public static List<Square> GetEntireDiagonalByFile(GameState gameState, int file, DiagonalDirectionFromFileNumber direction)
        {
            var indexes = GetEntireDiagonalByFile(file, direction);
            var list = new List<Square>();
            //optimize, a linq join here would perform better
            indexes.ForEach(a => list.Add(gameState.Squares.GetSquare(a)));
            return list;
        }

        public static List<int> GetEntireDiagonalByPosition(int position)
        {
            var list = new List<int>();
            foreach (var direction in GeneralReference.DiagonalLines)
            {
                var result = getEntireDiagonalLine(direction, position);
                list.AddRange(result);
            }
            return list;
        }

        public static bool IsDiagonal(int p1, int p2)
        {
            var list = GetEntireDiagonalByPosition(p1);
            return list.Contains(p2);
        }

        private static bool canDoDiagonalsFromStartPosition(int startPosition, int direction)
        {
            var isLeftSide = startPosition % 8 == 0;
            var isRightSide = startPosition % 8 == 7;
            if (isLeftSide && (direction == 7 || direction == -9)) { return false; }
            if (isRightSide && (direction == -7 || direction == 9)) { return false; }
            return true;
        }

        private static int getDiagonalIncrement(DiagonalDirectionFromFileNumber direction)
        {
            return direction == DiagonalDirectionFromFileNumber.Left ? 7 : 9;
        }

        private static List<int> getEntireDiagonalLine(DiagonalDirection direction, int position)
        {
            var list = new List<int> { position };
            var increment = 0;
            var currentPosition = position;
            switch (direction)
            {
                case DiagonalDirection.UpLeft:
                    if (position % 8 == 0) { return list; }
                    if (position > 56) { return list; }
                    increment = 7;
                    while (currentPosition <= 63)
                    {
                        currentPosition = currentPosition + increment;
                        if (currentPosition <= 63)
                        {
                            list.Add(currentPosition);
                        }
                        if (currentPosition % 8 == 0) { break; }
                    }
                    break;

                case DiagonalDirection.UpRight:
                    if (File.Contains(position)) { return list; }
                    if (position > 56) { return list; }
                    increment = 9;
                    while (currentPosition <= 63)
                    {
                        currentPosition = currentPosition + increment;
                        if (currentPosition <= 63)
                        {
                            list.Add(currentPosition);
                        }
                        if (File.Contains(currentPosition)) { break; }
                    }
                    break;

                case DiagonalDirection.DownLeft:
                    if (position % 8 == 0) { return list; }
                    if (position < 7) { return list; }
                    increment = -9;
                    while (currentPosition >= 0)
                    {
                        currentPosition = currentPosition + increment;
                        if (currentPosition >= 0)
                        {
                            list.Add(currentPosition);
                        }
                        if (currentPosition % 8 == 0) { break; }
                    }
                    break;

                case DiagonalDirection.DownRight:
                    if (File.Contains(position)) { return list; }
                    if (position < 7) { return list; }
                    increment = -7;
                    while (currentPosition >= 0)
                    {
                        currentPosition = currentPosition + increment;
                        if (currentPosition >= 0)
                        {
                            list.Add(currentPosition);
                        }
                        if (File.Contains(currentPosition)) { break; }
                    }
                    break;

                case DiagonalDirection.Invalid:
                default:
                    break;
            }
            return list;
        }

        private static int getIteratorByDirectionEnum(DiagonalDirection direction)
        {
            switch (direction)
            {
                case DiagonalDirection.UpLeft:
                    return 7;

                case DiagonalDirection.UpRight:
                    return 9;

                case DiagonalDirection.DownLeft:
                    return -9;

                case DiagonalDirection.DownRight:
                    return -7;
            }
            throw new Exception($"getIteratorByDirectionEnum(): Invalid Direction { direction }");
        }

        private static DiagonalDirection getDirectionByMovePositions(int location, int destination)
        {
            var distance = destination - location;
            var nine = Math.Abs(distance) % 9;
            var seven = Math.Abs(distance) % 7;
            if (nine == 0)
            {
                return distance > 0 ? DiagonalDirection.UpLeft : DiagonalDirection.DownRight;
            }
            if (seven == 0)
            {
                return distance > 0 ? DiagonalDirection.UpRight : DiagonalDirection.DownLeft;
            }
            return DiagonalDirection.Invalid;
        }

        private static int howFarToGo(int file, DiagonalDirectionFromFileNumber direction)
        {
            return direction == DiagonalDirectionFromFileNumber.Left ? file + 1 : 8 - file;
        }

        private static bool isValidDiagonalCoordinate(int position)
        {
            if (!GeneralEngine.IsValidCoordinate(position)) { return false; }
            if (position % 8 == 0 || position % 8 == 7) { return false; }
            if (position < 7 || position > 56) { return false; }
            return true;
        }
    }
}