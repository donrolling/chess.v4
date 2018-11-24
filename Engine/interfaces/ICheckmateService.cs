using System;
using System.Collections.Generic;
using System.Text;
using chess.v4.models;
using chess.v4.models.enumeration;

namespace chess.v4.engine.interfaces {
	public interface ICheckmateService {
		bool IsCheckMate(GameState gameState, Color white, IEnumerable<AttackedSquare> whiteKingAttacks);
	}
}
