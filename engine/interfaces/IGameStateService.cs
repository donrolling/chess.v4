using chess.v4.engine.enumeration;
using chess.v4.engine.model;
using common;

namespace chess.v4.engine.interfaces {

	public interface IGameStateService {

		ResultOuput<GameState> SetStartPosition(string fen);

		ResultOuput<GameState> MakeMove(GameState gameState, int piecePosition, int newPiecePosition, string pgnMove);

		ResultOuput<GameState> MakeMove(GameState gameState, string beginning, string destination);

		ResultOuput<GameState> UpdateGameStateWithError(GameState gameState, string errorMessage);
	}
}