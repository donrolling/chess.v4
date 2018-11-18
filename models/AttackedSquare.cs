namespace chess.v4.models {
	public class AttackedSquare : Square {
		public Square AttackingSquare { get; set; }
		public bool MayOnlyMoveHereIfOccupiedByEnemy { get; }
		public bool IsPassiveAttack { get; set; }

		public AttackedSquare() {
		}

		public AttackedSquare(Square attackingSquare, Square square, bool isPassiveAttack = false, bool canOnlyMoveHereIfOccupied = false) : base(square.Index, square.Name, square.Piece) {
			this.AttackingSquare = attackingSquare;
			this.IsPassiveAttack = isPassiveAttack;
			this.MayOnlyMoveHereIfOccupiedByEnemy = canOnlyMoveHereIfOccupied;
		}
	}
}