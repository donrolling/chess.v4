using chess.v4.engine.enumeration;
using chess.v4.engine.model;
using System.Collections.Generic;

namespace chess.v4.engine.interfaces {

	public interface IDiagonalService {

		List<Square> GetDiagonalLine(GameState gameState, Square square, Piece attackingPiece, DiagonalDirection direction, bool ignoreKing);

		List<Square> GetDiagonals(GameState gameState, Square square, bool ignoreKing = false);
	}
}