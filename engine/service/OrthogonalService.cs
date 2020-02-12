using Chess.v4.Engine.Interfaces;
using Chess.v4.Engine.Reference;
using Chess.v4.Engine.Utility;
using Chess.v4.Models;
using Chess.v4.Models.Enums;
using System.Collections.Generic;
using System.Linq;

namespace Chess.v4.Engine.Service
{
    public class OrthogonalService : IOrthogonalService
    {
        public List<int> GetEntireFile(int file)
        {
            return GeneralUtility.GetEntireFile(file);
        }

        public List<int> GetEntireRank(int rank)
        {
            return GeneralUtility.GetEntireRank(rank);
        }

        public List<AttackedSquare> GetOrthogonalLine(GameState gameState, Square square, Direction direction)
        {
            var attacks = new List<AttackedSquare>();
            var currentPosition = square.Index;
            var lineTerminator = getOrthogonalLineTerminator(direction, currentPosition);
            var iterator = getIteratorByDirectionEnum(direction);
            var nextPositionInTheLine = currentPosition + iterator;
            for (var position = nextPositionInTheLine; position != lineTerminator; position = position + iterator)
            {
                var isValidCoordinate = GeneralUtility.IsValidCoordinate(position);
                if (!isValidCoordinate) { break; }
                var moveViability = GeneralUtility.DetermineMoveViability(gameState, square.Piece, position);
                //these conditions shouldn't occur
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
            }
            return attacks;
        }

        public void GetOrthogonals(GameState gameState, Square square, List<AttackedSquare> accumulator)
        {
            foreach (var orthogonalLine in GeneralReference.OrthogonalLines)
            {
                var attacks = GetOrthogonalLine(gameState, square, orthogonalLine);
                if (attacks != null && attacks.Any())
                {
                    accumulator.AddRange(attacks);
                }
            }
        }

        private int getOrthogonalLineTerminator(Direction direction, int position)
        {
            var file = NotationUtility.PositionToFileInt(position);
            var rank = NotationUtility.PositionToRankInt(position);
            var iterator = getIteratorByDirectionEnum(direction);

            switch (direction)
            {
                case Direction.Forward:
                    return this.GetEntireFile(file).Max() + iterator;

                case Direction.Backward:
                    return this.GetEntireFile(file).Min() + iterator;

                case Direction.Right:
                    return this.GetEntireRank(rank).Max() + iterator;

                case Direction.Left:
                    return this.GetEntireRank(rank).Min() + iterator;
            }

            return 0;
        }

        private int getIteratorByDirectionEnum(Direction direction)
        {
            switch (direction)
            {
                // toward black side
                case Direction.Forward:
                    return 8;

                // toward white side
                case Direction.Backward:
                    return -8;

                // toward white right side
                case Direction.Right:
                    return 1;

                // toward white left side
                case Direction.Left:
                    return -1;
            }
            return 0;
        }
    }
}