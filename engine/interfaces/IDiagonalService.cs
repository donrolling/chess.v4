using chess.v4.models.enumeration;
using chess.v4.models;
using System.Collections.Generic;

namespace chess.v4.engine.interfaces {

	public interface IDiagonalService {
		List<Square> GetEntireDiagonalByFile(GameState gameState, int file, DiagonalDirectionFromFileNumber direction);

		List<Square> GetDiagonalLine(GameState gameState, Square square, Piece attackingPiece, DiagonalDirection direction, bool ignoreKing);

		void GetDiagonals(GameState gameState, Square square, List<AttackedSquare> accumulator, bool ignoreKing = false);
	}
}