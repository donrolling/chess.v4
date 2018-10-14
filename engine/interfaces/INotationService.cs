using chess.v4.engine.enumeration;
using chess.v4.engine.model;
using System.Collections.Generic;

namespace chess.v4.engine.interfaces {

	public interface INotationService {
		List<Square> ApplyMoveToSquares(List<Square> squares, int piecePosition, int newPiecePosition);

		void UpdateMatrix_PromotePiece(List<Square> squares, int newPiecePosition, Color pieceColor, char piecePromotedTo);

		List<Square> GetSquaresFromFEN_Record(FEN_Record fen);

		FEN_Record CreateNewFENFromGameState(GameState gameState, List<Square> squares, int piecePosition, int newPiecePosition);
	}
}