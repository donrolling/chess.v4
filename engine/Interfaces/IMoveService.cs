namespace engine.Interfaces;

using common.Responses;
using engine.Models.Enums;
using Models;

public interface IMoveService
{
	OperationResult<StateInfo> GetStateInfo(GameState newGameState, int piecePosition, int newPiecePosition);

	StateInfo GetStateInfo(GameState gameState);

	bool HasThreefoldRepition(GameState gameState);

	bool IsEnPassant(Square square, int newPiecePosition, string enPassantTargetSquare);

	bool IsRealCheck(List<Square> squares, IEnumerable<AttackedSquare> attacksThatCheckWhite, Color activeColor, int kingSquare);

	OperationResult<bool> IsValidCastleAttempt(GameState gameState, Square square, int destination, IEnumerable<AttackedSquare> attackedSquares);

	bool IsValidPawnMove(Square square, List<Square> oldSquares, Color color, int piecePosition, int newPiecePosition, bool isEnPassant);
}