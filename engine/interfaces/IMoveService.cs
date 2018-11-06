using chess.v4.models.enumeration;
using chess.v4.models;
using Common;
using System.Collections.Generic;
using Common.Models;

namespace chess.v4.engine.interfaces {

	public interface IMoveService {

		Envelope<StateInfo> GetStateInfo(GameState newGameState, int piecePosition, int newPiecePosition);

		StateInfo GetStateInfo(GameState gameState);

		bool HasThreefoldRepition(GameState gameState);

		bool IsDiagonalMove(int startPosition, int endPosition);

		bool IsEnPassant(Square square, int newPiecePosition, string enPassantTargetSquare);

		bool IsRealCheck(List<Square> squares, IEnumerable<AttackedSquare> attacksThatCheckWhite, Color activeColor, int kingSquare);

		Envelope<bool> IsValidCastleAttempt(GameState gameState, Square square, int destination, IEnumerable<AttackedSquare> attackedSquares);

		bool IsValidPawnMove(Square square, List<Square> oldSquares, Color color, int piecePosition, int newPiecePosition, bool isEnPassant);
	}
}