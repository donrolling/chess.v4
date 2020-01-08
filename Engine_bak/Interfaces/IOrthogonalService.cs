using Chess.v4.Models;
using Chess.v4.Models.Enums;
using System.Collections.Generic;

namespace Chess.v4.Engine.Interfaces
{
    public interface IOrthogonalService
    {
        List<int> GetEntireFile(int file);

        List<int> GetEntireRank(int rank);

        List<AttackedSquare> GetOrthogonalLine(GameState gameState, Square square, Direction direction, bool ignoreKing = false);

        void GetOrthogonals(GameState gameState, Square square, List<AttackedSquare> accumulator, bool ignoreKing = false);
    }
}