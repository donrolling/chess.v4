using chess_engine.Models;

namespace chess_engine.Engine.Interfaces;

public interface IAttackService
{
	IEnumerable<AttackedSquare> GetAttacks(GameState gameState);
}