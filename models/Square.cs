using System;

namespace chess.v4.models {

	public class Square : ICloneable {
		public int Index { get; set; }
		public string Name { get; set; }
		public bool Occupied { get { return this.Piece != null; } }
		public Piece Piece { get; set; }

		public Square() {
		}

		public Square(int index, string name, Piece piece) {
			this.Index = index;
			this.Name = name;
			this.Piece = piece;
		}

		public object Clone() {
			return new Square {
				Index = this.Index,
				Name = this.Name,
				Piece = this.Piece != null ?
					new Piece(this.Piece.PieceType, this.Piece.Color)
					: null
			};
		}
	}
}