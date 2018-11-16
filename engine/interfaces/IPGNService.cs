using chess.v4.models;
using chess.v4.models.enumeration;
using System.Collections.Generic;

namespace chess.v4.engine.interfaces {
	public interface IPGNService {

		Square GetCurrentPositionFromPGNMove(GameState gameState, Piece piece, int newPiecePosition, string pgnMove);

		char GetPieceCharFromPieceTypeColor(PieceType piece, Color playerColor);

		PieceType GetPieceTypeFromPGNMove(string pgnMove);

		bool IsRank(char potentialRank);

		(int piecePosition, int newPiecePosition, char promotedPiece) PGNMoveToSquarePair(GameState gameState, string pgnMove);

		List<string> PGNSplit(string pgn);

		List<string> PGNSplit(string pgn, bool mostConsise);

		string SquarePairToPGNMove(GameState gameState, Color playerColor, string startSquare, string endSquare, char promoteToPiece);
	}
}