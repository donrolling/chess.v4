namespace engine.Interfaces;

using engine.Models.Enums;
using Models;

public interface ICheckmateService
{
	bool IsCheckMate(GameState gameState, Color white, IEnumerable<AttackedSquare> whiteKingAttacks);
}