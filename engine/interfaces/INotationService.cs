using chess.v4.engine.model;
using System.Collections.Generic;

namespace chess.v4.engine.interfaces {

	public interface INotationService {

		List<Square> GetSquaresFromFEN_Record(FEN_Record fen);

		void SetGameState_FEN(GameState gameState, GameState newGameState, int piecePosition, int newPiecePosition);
	}
}