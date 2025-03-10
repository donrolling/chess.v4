namespace engine.Interfaces;

using Models;

public interface INotationService
{
	List<Square> GetSquares(Snapshot fen);

	void SetGameStateSnapshot(GameState oldGameState, GameState newGameState, StateInfo stateInfo, int piecePosition, int newPiecePosition);
}