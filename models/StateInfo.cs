using chess.v4.models.enumeration;

namespace chess.v4.models {
	public class StateInfo {
		public bool HasThreefoldRepition { get; set; }
		public bool IsBlackCheck { get; set; }
		public bool IsCastle { get; set; }
		public bool IsCheck { get { return IsWhiteCheck || IsBlackCheck; } }
		public bool IsCheckmate { get; set; }
		public bool IsDraw { get; set; }
		public bool IsEnPassant { get; set; }
		public bool IsPawnPromotion { get; set; }
		public bool IsWhiteCheck { get; set; }
		public PieceType PawnPromotedTo { get; set; }
		public string Result { get; set; } = "-";

		public StateInfo() {
		}
	}
}