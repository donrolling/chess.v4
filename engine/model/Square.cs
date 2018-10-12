using System;

namespace chess.v4.engine.model {

	public class Square : ICloneable {
		public int Index { get; set; }
		public bool Occupied { get { return this.Piece != null; } }
		public Piece Piece { get; set; }

		public object Clone() {
			return new Square {
				Index = this.Index,
				Piece = this.Piece != null ?
					new Piece {
						Identity = this.Piece.Identity,
						Color = this.Piece.Color,
						PieceType = this.Piece.PieceType
					}
					: null
			};
		}
	}
}