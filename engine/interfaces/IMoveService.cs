using chess.v4.engine.enumeration;
using chess.v4.engine.model;
using common;
using System.Collections.Generic;

namespace chess.v4.engine.interfaces {

	public interface IMoveService {

		ResultOuput<bool> IsValidCastleAttempt(GameState gameState, Square square, int destination, IEnumerable<AttackedSquare> attackedSquares);

		bool IsCheckmate(GameState gameState, Square checkedKing, IEnumerable<AttackedSquare> allAttacks, IEnumerable<AttackedSquare> blackAttacks);

		bool IsDiagonalMove(int startPosition, int endPosition);

		bool IsEnPassant(char piece, int piecePosition, int newPiecePosition, string enPassantTargetSquare);

		bool IsRealCheck(List<Square> squares, IEnumerable<AttackedSquare> attacksThatCheckWhite, Color activeColor, int kingSquare);

		bool IsValidPawnMove(Square square, List<Square> oldSquares, Color color, int piecePosition, int newPiecePosition, bool isEnPassant);
	}
}