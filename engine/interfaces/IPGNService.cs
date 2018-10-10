using System.Collections.Generic;
using chess.v4.engine.enumeration;
using chess.v4.engine.interfaces;

namespace chess.v4.engine.interfaces {
	public interface IPGNService {
		ICoordinateService CoordinateService { get; }

		int GetCurrentPositionFromPGNMove(Dictionary<int, char> matrix, Dictionary<int, List<int>> allAttacks, PieceType piece, Color playerColor, int newPiecePosition, string pgnMove);
		char GetPieceCharFromPieceTypeColor(PieceType piece, Color playerColor);
		PieceType GetPieceTypeFromPGNMove(string pgnMove);
		int GetPositionFromPGNMove(string pgnMove, Color playerColor);
		bool IsRank(char potentialRank);
		(int piecePosition, int newPiecePosition) PGNMoveToSquarePair(Dictionary<int, char> matrix, Dictionary<int, List<int>> allAttacks, Color playerColor, string pgnMove);
		List<string> PGNSplit(string pgn);
		List<string> PGNSplit(string pgn, bool mostConsise);
		string SquarePairToPGNMove(Dictionary<int, char> matrix, Dictionary<int, List<int>> allAttacks, Color playerColor, string startSquare, string endSquare, char promoteToPiece);
	}
}