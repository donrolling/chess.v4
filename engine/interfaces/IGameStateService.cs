using chess.v4.engine.enumeration;
using chess.v4.engine.model;
using System.Collections.Generic;

namespace chess.v4.engine.interfaces {

	public interface IGameStateService {

		GameState SetStartPosition(string fen);

		GameState UpdateGameState(GameState gameState, Color color, int piecePosition, int newPiecePosition, string pgnMove, List<History> History);

		GameState UpdateGameStateWithError(GameState gameState, string errorMessage);
	}
}