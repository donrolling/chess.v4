namespace engine.Interfaces;

using engine.Models.Enums;
using Models;

public interface IOrthogonalService
{
	List<int> GetEntireFile(int file);

	List<int> GetEntireRank(int rank);

	List<AttackedSquare> GetOrthogonalLine(GameState gameState, Square square, Direction direction);

	void GetOrthogonals(GameState gameState, Square square, List<AttackedSquare> accumulator);
}