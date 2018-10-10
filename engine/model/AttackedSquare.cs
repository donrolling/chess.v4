namespace chess.v4.engine.model {

	public class AttackedSquare {
		public Square AttackingSquare { get; set; }
		public Square Square { get; set; }

		public AttackedSquare(Square attackingSquare, Square square) {
			this.AttackingSquare = attackingSquare;
			this.Square = square;
		}
	}
}