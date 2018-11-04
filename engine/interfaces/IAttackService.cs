using chess.v4.models;
using System.Collections.Generic;

namespace chess.v4.engine.interfaces {

	public interface IAttackService {

		IEnumerable<AttackedSquare> GetAttacks(GameState gameState, bool ignoreKing = false);

		void GetKingAttacks(GameState gameState, Square square, List<AttackedSquare> accumulator);
	}
}