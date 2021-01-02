using chess_engine.Models;
using chess_engine.Models.Enums;
using System.Collections.Generic;

namespace chess_engine.Engine.Interfaces
{
    public interface IOrthogonalService
    {
        List<int> GetEntireFile(int file);

        List<int> GetEntireRank(int rank);

        List<AttackedSquare> GetOrthogonalLine(GameState gameState, Square square, Direction direction);

        void GetOrthogonals(GameState gameState, Square square, List<AttackedSquare> accumulator);
    }
}