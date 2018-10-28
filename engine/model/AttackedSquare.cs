namespace chess.v4.engine.model {
	public class AttackedSquare : Square {
		public Square AttackerSquare { get; set; }
		
		public AttackedSquare() {
		}

		public AttackedSquare(Square attackingSquare, Square square) : base(square.Index, square.Name, square.Piece) {
			this.AttackerSquare = attackingSquare;
		}
	}
}