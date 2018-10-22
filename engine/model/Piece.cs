using chess.v4.engine.enumeration;

namespace chess.v4.engine.model {

	public class Piece {
		public Color Color { get; set; }
		public char Identity { get; set; }
		public PieceType PieceType { get; set; }

		public Piece() {
		}

		public Piece(PieceType pieceType, Color color) {
			this.PieceType = pieceType;
			this.Color = color;
			this.Identity = getIdentityViaPieceType(color, pieceType);
		}

		private static char getIdentityViaPieceType(PieceType pieceType) {
			switch (pieceType) {
				case PieceType.Pawn:
					return 'p';

				case PieceType.Knight:
					return 'n';

				case PieceType.Bishop:
					return 'b';

				case PieceType.Rook:
					return 'r';

				case PieceType.Queen:
					return 'q';

				case PieceType.King:
					return 'k';

				default:
					throw new System.Exception("Type not matched.");
			}
		}

		private static char getIdentityViaPieceType(Color color, PieceType pieceType) {
			var identity = getIdentityViaPieceType(pieceType);
			return color == Color.White ? char.ToUpper(identity) : char.ToLower(identity);
		}
	}
}