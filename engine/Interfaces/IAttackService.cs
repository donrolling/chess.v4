namespace engine.Interfaces;

using Models;

public interface IAttackService
{
	IEnumerable<AttackedSquare> GetAttacks(GameState gameState);
}