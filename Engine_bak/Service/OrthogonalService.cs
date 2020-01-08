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
        public OrthogonalService()
        {
        }

        public List<int> GetEntireFile(int file)
        {
            return GeneralUtility.GetEntireFile(file);
        }

        public List<int> GetEntireRank(int rank)
        {
            return GeneralUtility.GetEntireRank(rank);
        }

        public List<AttackedSquare> GetOrthogonalLine(GameState gameState, Square movingSquare, Direction direction, bool ignoreKing = false)
        {
            var currentPosition = movingSquare.Index;
            var orthogonalLine = getOrthogonalLineFromPos(direction, currentPosition);
            var attacks = new List<AttackedSquare>();
            var iterator = getIteratorByDirectionEnum(direction);
            for (var position = currentPosition + iterator; position != orthogonalLine + iterator; position = position + iterator)
            {
                var isValidCoordinate = GeneralUtility.IsValidCoordinate(position);
                if (!isValidCoordinate) { break; }
                var moveViability = GeneralUtility.DetermineMoveViability(gameState, movingSquare.Piece, position, ignoreKing);
                //these conditions shouldn't occur
                if (!moveViability.IsValidCoordinate || moveViability.SquareToAdd == null)
                {
                    continue;
                }
                if (moveViability.IsTeamPiece)
                {
                    attacks.Add(new AttackedSquare(movingSquare, moveViability.SquareToAdd, isProtecting: true));
                }
                else
                {
                    attacks.Add(new AttackedSquare(movingSquare, moveViability.SquareToAdd));
                }
                if (moveViability.BreakAfterAction)
                {
                    break;
                }
            }
            return attacks;
        }

        public void GetOrthogonals(GameState gameState, Square square, List<AttackedSquare> accumulator, bool ignoreKing = false)
        {
            foreach (var orthogonalLine in GeneralReference.OrthogonalLines)
            {
                var attacks = GetOrthogonalLine(gameState, square, orthogonalLine, ignoreKing);
                if (attacks != null && attacks.Any())
                {
                    accumulator.AddRange(attacks);
                }
            }
        }

        private int getOrthogonalLineFromPos(Direction direction, int position)
        {
            var file = NotationUtility.PositionToFileInt(position);
            var rank = NotationUtility.PositionToRankInt(position);

            switch (direction)
            {
                case Direction.RowUp:
                    return this.GetEntireFile(file).Max();

                case Direction.RowDown:
                    return this.GetEntireFile(file).Min();

                case Direction.FileUp:
                    return this.GetEntireRank(rank).Max();

                case Direction.FileDown:
                    return this.GetEntireRank(rank).Min();
            }

            return 0;
        }

        private int getIteratorByDirectionEnum(Direction direction)
        {
            switch (direction)
            {
                case Direction.RowUp:
                    return 8;

                case Direction.RowDown:
                    return -8;

                case Direction.FileUp:
                    return 1;

                case Direction.FileDown:
                    return -1;
            }
            return 0;
        }
    }
}