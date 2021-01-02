using chess_engine.Models;
using System.Collections.Generic;

namespace chess_engine.Engine.Interfaces
{
    public interface IAttackService
    {
        IEnumerable<AttackedSquare> GetAttacks(GameState gameState);
    }
}