﻿namespace chess.v4.engine.model {

	public class MoveInfo {
		public bool HasThreefoldRepition { get; set; }
		public bool IsBlackCheck { get; set; }
		public bool IsCastle { get; set; }
		public bool IsCheck { get { return IsWhiteCheck || IsBlackCheck; } }
		public bool IsCheckmate { get; set; }
		public bool IsEnPassant { get; set; }
		public bool IsPawnPromotion { get; set; }
		public bool IsWhiteCheck { get; set; }
		public bool IsResign { get; set; }
		public bool IsDraw { get; set; }
		public bool PutsOwnKingInCheck { get; set; }
	}
}