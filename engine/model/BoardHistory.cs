using System;
using System.Collections.Generic;
using System.Text;

namespace chess.v4.engine.model {
	public class BoardHistory {
		public string FEN { get; set; }
		public string CastlingAvailability { get; set; }
		public string EnPassantTargetSquare { get; set; }
	}
}
