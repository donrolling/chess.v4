namespace chess.v4.engine.model {

	public class AttackedSquare : Square {
		public Square AttackerSquare { get; set; }

		public AttackedSquare() {

		}

		public AttackedSquare(Square attackingSquare, Square square) {
			this.AttackerSquare = attackingSquare;
			this.Index = square.Index;
			this.Piece = square.Piece;
		}
	}
}