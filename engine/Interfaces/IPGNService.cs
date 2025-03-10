using chess_engine.Models;
using chess_engine.Models.Enums;

namespace chess_engine.Engine.Interfaces
{
	public interface IPGNService
	{
		Square GetCurrentPositionFromPGNMove(GameState gameState, Piece piece, int newPiecePosition, string pgnMove, bool isCastle);

		bool IsRank(char potentialRank);

		(int piecePosition, int newPiecePosition, char promotedPiece) PGNMoveToSquarePair(GameState gameState, string pgnMove);

		string SquarePairToPGNMove(GameState gameState, Color playerColor, string startSquare, string endSquare);

		string SquarePairToPGNMove(GameState gameState, Color playerColor, int startSquare, int endSquare);

		string SquarePairToPGNMove(GameState gameState, Color playerColor, string startSquare, string endSquare, PieceType promoteToPiece);

		string SquarePairToPGNMove(GameState gameState, Color playerColor, int startSquare, int endSquare, PieceType promoteToPiece);
	}
}