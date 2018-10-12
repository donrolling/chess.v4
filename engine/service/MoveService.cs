using chess.v4.engine.enumeration;
using chess.v4.engine.interfaces;
using chess.v4.engine.model;
using System;

namespace chess.v4.engine.service {

	public class MoveService : IMoveService {

		public bool IsCastle(Square square, int destination) {
			if (!square.Occupied) {
				return false;
			}
			return
				square.Piece.PieceType == PieceType.King
				&& Math.Abs(square.Index - destination) == 2;
		}
	}
}