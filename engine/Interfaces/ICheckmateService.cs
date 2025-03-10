using chess_engine.Models;
using chess_engine.Models.Enums;

namespace chess_engine.Engine.Interfaces;

public interface ICheckmateService
{
	bool IsCheckMate(GameState gameState, Color white, IEnumerable<AttackedSquare> whiteKingAttacks);
}