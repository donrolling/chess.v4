using chess.v4.engine.model;
using System.Collections.Generic;

namespace chess.v4.engine.interfaces {

	public interface IAttackService {

		IEnumerable<AttackedSquare> GetAttacks(GameState gameState, bool ignoreKing = false);

		IEnumerable<Square> GetKingAttack(AttackedSquare attacker, GameState gameState, Square enemyKingPosition);

		IEnumerable<AttackedSquare> GetKingAttacks(GameState gameState, Square square);
	}
}