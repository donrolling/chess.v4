namespace chess.v4.engine.model {
	public class AttackedSquare : Square {
		public Square AttackingSquare { get; set; }

		public bool IsPassiveAttack { get; set; }

		public AttackedSquare() {
		}

		public AttackedSquare(Square attackingSquare, Square square, bool isPassiveAttack = false) : base(square.Index, square.Name, square.Piece) {
			this.AttackingSquare = attackingSquare;
			this.IsPassiveAttack = isPassiveAttack;
		}
	}
}