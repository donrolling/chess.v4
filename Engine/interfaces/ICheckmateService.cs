using Chess.v4.Models;
using Chess.v4.Models.Enums;
using System.Collections.Generic;

namespace Chess.v4.Engine.Interfaces
{
    public interface ICheckmateService
    {
        bool IsCheckMate(GameState gameState, Color white, IEnumerable<AttackedSquare> whiteKingAttacks);
    }
}