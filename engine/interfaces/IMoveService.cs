using chess.v4.engine.model;

namespace chess.v4.engine.interfaces {
	public interface IMoveService {
		bool IsCastle(Square square, int destination);
	}
}