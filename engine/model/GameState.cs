using System.Collections.Generic;

namespace chess.v4.engine.model {

	public class GameState {
		public string FEN { get; set; }
		public List<Square> Squares { get; set; } = new List<Square>();
		public bool IsCheck { get { return IsWhiteCheck || IsBlackCheck; } }
		public bool IsWhiteCheck { get; set; }
		public bool IsBlackCheck { get; set; }
		public bool IsCheckmate { get; set; }
		public bool HasThreefoldRepition { get; set; }
	}
}