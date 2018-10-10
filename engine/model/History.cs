namespace chess.v4.engine.model {

	public class History {
		public string FEN { get; set; }
		public string CastlingAvailability { get; set; }

		//En passant target square in algebraic notation. If there's no en passant target square, this is "-". If a pawn has just made a two-square move, this is the position "behind" the pawn. This is recorded regardless of whether there is a pawn in position to make an en passant capture.
		public string EnPassantTargetSquare { get; set; }
	}
}