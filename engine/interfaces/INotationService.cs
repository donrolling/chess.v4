using chess.v4.engine.enumeration;
using chess.v4.engine.model;
using System.Collections.Generic;

namespace chess.v4.engine.interfaces {

	public interface INotationService {
		void SetGameState_FEN(GameState gameState, int piecePosition, int newPiecePosition);

		List<Square> GetSquaresFromFEN_Record(FEN_Record fen);
	}
}