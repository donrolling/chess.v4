using chess.v4.engine.enumeration;
using System.Collections.Generic;

namespace chess.v4.engine.model {

	public class GameState : History {
		public Color ActiveColor { get; set; }
		public bool HasThreefoldRepition { get; set; }
		public bool IsBlackCheck { get; set; }
		public bool IsCheck { get { return IsWhiteCheck || IsBlackCheck; } }
		public bool IsCheckmate { get; set; }
		public bool IsWhiteCheck { get; set; }

		//This is the number of halfmoves since the last pawn advance or capture. This is used to determine if a draw can be claimed under the fifty-move rule.
		public int HalfmoveClock { get; set; }

		//The number of the full move. It starts at 1, and is incremented after Black's move.
		public int FullmoveNumber { get; set; }

		public List<Square> Squares { get; set; }
		public string PGN { get; set; }
		public List<History> History { get; set; }
	}
}