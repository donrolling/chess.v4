using chess.v4.engine.enumeration;
using chess.v4.engine.interfaces;
using chess.v4.engine.model;
using System;

namespace chess.v4.engine.service {

	public class MoveService : IMoveService {

		public bool IsCastle(Piece piece, int piecePosition, int newPiecePosition) {
			return
				piece.PieceType == PieceType.King
				&& Math.Abs(piecePosition - newPiecePosition) == 2;
		}
	}
}