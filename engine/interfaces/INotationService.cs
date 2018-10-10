using chess.v4.engine.enumeration;
using chess.v4.engine.model;
using System.Collections.Generic;

namespace chess.v4.engine.interfaces {

	public interface INotationService {

		void UpdateMatrix(List<Square> squares, int piecePosition, int newPiecePosition);

		void UpdateMatrix_PromotePiece(List<Square> squares, int newPiecePosition, Color pieceColor, char piecePromotedTo);

		List<Square> CreateMatrixFromFEN(string fen);

		string CreateNewFENFromGameState(GameState gameState, List<Square> squares, int piecePosition, int newPiecePosition);
	}
}