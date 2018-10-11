using chess.v4.engine.enumeration;
using System;
using System.Collections.Generic;
using System.Text;

namespace chess.v4.engine.model {
	public class Square {
		public int Index { get; set; }
		public bool Occupied { get { return this.Piece != null; } }
		public Piece Piece { get; set; }
	}
}
