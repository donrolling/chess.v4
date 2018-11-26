using chess.v4.models.enumeration;
using chess.v4.models;
using System.Collections.Generic;

namespace chess.v4.engine.interfaces {

	public interface IOrthogonalService {

		List<int> GetEntireFile(int file);

		List<int> GetEntireRank(int rank);

		List<AttackedSquare> GetOrthogonalLine(GameState gameState, Square square, Direction direction, bool ignoreKing = false);

		void GetOrthogonals(GameState gameState, Square square, List<AttackedSquare> accumulator, bool ignoreKing = false);
	}
}