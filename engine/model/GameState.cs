﻿using System.Collections.Generic;

namespace chess.v4.engine.model {

	public class GameState : FEN_Record {
		public List<FEN_Record> FEN_Records { get; set; } = new List<FEN_Record>();
		public MoveInfo MoveInfo { get; set; }
		public string PGN { get; set; }
		public List<Square> Squares { get; set; } = new List<Square>();

		public GameState() {
		}

		public GameState(string fen) : base(fen) {
		}

		public GameState(FEN_Record fenRecord) {
			this.PiecePlacement = fenRecord.PiecePlacement;
			this.ActiveColor = fenRecord.ActiveColor;
			this.CastlingAvailability = fenRecord.CastlingAvailability;
			this.EnPassantTargetSquare = fenRecord.EnPassantTargetSquare;
			this.HalfmoveClock = fenRecord.HalfmoveClock;
			this.FullmoveNumber = fenRecord.FullmoveNumber;
		}

		public override string ToString() {
			return FEN_Record.ConvertToString(this);
		}
	}
}