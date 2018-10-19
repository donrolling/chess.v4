using chess.v4.engine.model;
using common;

namespace chess.v4.engine.interfaces {

	public interface IGameStateService {

		Envelope<GameState> Initialize();

		Envelope<GameState> Initialize(string fen);

		Envelope<GameState> MakeMove(GameState gameState, int piecePosition, int newPiecePosition, string pgnMove);

		Envelope<GameState> MakeMove(GameState gameState, string beginning, string destination);
	}
}