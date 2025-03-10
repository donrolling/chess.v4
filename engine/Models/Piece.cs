using chess_engine.Models.Enums;

namespace chess_engine.Models
{
	public class Piece
	{
		public Color Color { get; set; }
		public char Identity { get; set; }
		public PieceType PieceType { get; set; }
		public int OrderOfOperation { get; set; }

		public Piece()
		{
		}

		public Piece(PieceType pieceType, Color color)
		{
			this.PieceType = pieceType;
			this.Color = color;
			this.Identity = getIdentityViaPieceType(color, pieceType);
			this.OrderOfOperation = getOrderOfOperation(pieceType);
		}

		private int getOrderOfOperation(PieceType pieceType)
		{
			switch (pieceType)
			{
				case PieceType.Pawn:
					return 1;

				case PieceType.Knight:
					return 2;

				case PieceType.Bishop:
					return 3;

				case PieceType.Rook:
					return 4;

				case PieceType.Queen:
					return 5;

				case PieceType.King:
					return 6;

				default:
					return -1;
			}
		}

		private static char getIdentityViaPieceType(PieceType pieceType)
		{
			switch (pieceType)
			{
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

		private static char getIdentityViaPieceType(Color color, PieceType pieceType)
		{
			var identity = getIdentityViaPieceType(pieceType);
			return color == Color.White ? char.ToUpper(identity) : char.ToLower(identity);
		}
	}
}