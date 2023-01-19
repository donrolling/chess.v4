namespace chess_engine.Models
{
	public class AttackedSquare : Square
	{
		public Square AttackingSquare { get; set; }

		/// <summary>
		/// indicates a move that cannot capture such as a pawn forward attack
		/// </summary>
		public bool IsPassiveAttack { get; set; }

		/// <summary>
		/// indicates that the attack is there for retaliation if the existing piece is taken
		/// the existing piece has to be the same color as the attacker.
		/// </summary>
		public bool IsProtecting { get; set; }

		public bool MayOnlyMoveHereIfOccupiedByEnemy { get; }

		public AttackedSquare()
		{
		}

		public AttackedSquare(Square attackingSquare, Square square, bool isPassiveAttack = false, bool canOnlyMoveHereIfOccupied = false, bool isProtecting = false) : base(square.Index, square.Name, square.Piece)
		{
			AttackingSquare = attackingSquare;
			IsPassiveAttack = isPassiveAttack;
			MayOnlyMoveHereIfOccupiedByEnemy = canOnlyMoveHereIfOccupied;
			IsProtecting = isProtecting;
		}
	}
}