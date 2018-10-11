using chess.v4.engine.enumeration;
using chess.v4.engine.model;
using common;

namespace chess.v4.engine.interfaces {

	public interface IGameStateService {

		ResultOuput<GameState> SetStartPosition(string fen);

		ResultOuput<GameState> UpdateGameState(GameState gameState, Color color, int piecePosition, int newPiecePosition, string pgnMove);

		ResultOuput<GameState> UpdateGameStateWithError(GameState gameState, string errorMessage);
	}
}