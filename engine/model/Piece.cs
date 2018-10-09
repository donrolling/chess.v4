using chess.v4.engine.enumeration;

namespace chess.v4.engine.model {

	public class Piece {
		public char Identity { get; set; }
		public PieceType PieceType { get; set; }
		public Color Color { get; set; }
	}
}