using chess.v4.engine.enumeration;
using chess.v4.engine.model;
using System.Collections.Generic;

namespace chess.v4.engine.interfaces {

	public interface IAttackService {

		IEnumerable<Square> GetAttacks(Color color, string fen, bool ignoreKing = false);

		IEnumerable<Square> GetBishopAttacks(List<Square> matrix, int position, Color pieceColor, bool ignoreKing);

		(PieceType, Color) GetChessTypeFromChar(char piece);

		IEnumerable<Square> GetKingAttacks(string fen, int position, Color pieceColor, string castleAvailability);

		IEnumerable<Square> GetKnightAttacks(List<Square> squares, int currentPosition, Color pieceColor);

		IEnumerable<Square> GetPawnAttacks(List<Square> squares, int position, Color pieceColor, int enPassantPosition);

		IEnumerable<Square> GetPieceAttacks(string fen, Square square, bool ignoreKing = false);

		IEnumerable<Square> GetQueenAttacks(List<Square> squares, int position, Color pieceColor, bool ignoreKing);

		IEnumerable<Square> GetRookAttacks(List<Square> squares, int position, Color pieceColor, bool ignoreKing);
	}
}