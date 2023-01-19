using chess_engine.Models;
using chess_engine.Models.Enums;
using System.Collections.Generic;

namespace chess_engine.Engine.Interfaces
{
	public interface ICheckmateService
	{
		bool IsCheckMate(GameState gameState, Color white, IEnumerable<AttackedSquare> whiteKingAttacks);
	}
}