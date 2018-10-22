using System.Collections.Generic;
using chess.v4.engine.enumeration;
using chess.v4.engine.interfaces;
using chess.v4.engine.model;

namespace chess.v4.engine.interfaces {
	public interface IPGNService {
		ICoordinateService CoordinateService { get; }

		Square GetCurrentPositionFromPGNMove(GameState gameState, Piece piece, int newPiecePosition, string pgnMove);
		char GetPieceCharFromPieceTypeColor(PieceType piece, Color playerColor);
		PieceType GetPieceTypeFromPGNMove(string pgnMove);
		int GetPositionFromPGNMove(string pgnMove, Color playerColor);
		bool IsRank(char potentialRank);
		(int piecePosition, int newPiecePosition) PGNMoveToSquarePair(GameState gameState, string pgnMove);
		List<string> PGNSplit(string pgn);
		List<string> PGNSplit(string pgn, bool mostConsise);
		string SquarePairToPGNMove(GameState gameState, Color playerColor, string startSquare, string endSquare, char promoteToPiece);
	}
}