using Chess.v4.Models;
using System.Collections.Generic;

namespace Chess.v4.Engine.Interfaces
{
    public interface IAttackService
    {
        IEnumerable<AttackedSquare> GetAttacks(GameState gameState, bool ignoreKing = false);
    }
}