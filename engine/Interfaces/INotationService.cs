using chess_engine.Models;

namespace chess_engine.Engine.Interfaces;

public interface INotationService
{
	List<Square> GetSquares(Snapshot fen);

	void SetGameStateSnapshot(GameState oldGameState, GameState newGameState, StateInfo stateInfo, int piecePosition, int newPiecePosition);
}