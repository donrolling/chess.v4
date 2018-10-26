using chess.v4.engine.enumeration;

namespace chess.v4.engine.model {
	public class StateInfo {
		public bool HasThreefoldRepition { get; set; }
		public bool IsBlackCheck { get; set; }
		public bool IsCheck { get { return IsWhiteCheck || IsBlackCheck; } }
		public bool IsCheckmate { get; set; }
		public bool IsDraw { get; set; }
		public bool IsWhiteCheck { get; set; }
		public bool IsPawnPromotion { get; set; }
		public PieceType PawnPromotedTo { get; set; }

		public StateInfo() {
		}
	}
}