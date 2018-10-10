using chess.v4.engine.enumeration;
using chess.v4.engine.model;
using System.Collections.Generic;

namespace chess.v4.engine.interfaces {

	public interface IAttackService {

		IEnumerable<AttackedSquare> GetAttacks(Color color, string fen, bool ignoreKing = false);

		IEnumerable<AttackedSquare> GetKingAttacks(string fen, int position, Color pieceColor, string castleAvailability);
	}
}