using chess.v4.engine.enumeration;
using chess.v4.engine.model;
using System.Collections.Generic;

namespace chess.v4.engine.interfaces {

	public interface IAttackService {

		IEnumerable<AttackedSquare> GetAttacks(GameState gameState, bool ignoreKing = false);

		IEnumerable<AttackedSquare> GetKingAttacks(GameState gameState, Square square);
	}
}