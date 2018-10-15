using chess.v4.engine.enumeration;
using chess.v4.engine.model;
using System.Collections.Generic;

namespace chess.v4.engine.interfaces {

	public interface IOrthogonalService {
		List<int> GetEntireFile(int file);

		List<int> GetEntireRank(int rank);
		
		List<Square> GetOrthogonalLine(GameState gameState, Square square, Direction direction, bool ignoreKing = false);

		List<Square> GetOrthogonals(GameState gameState, Square square, bool ignoreKing = false);
	}
}