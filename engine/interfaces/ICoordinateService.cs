using chess.v4.engine.enumeration;
using chess.v4.engine.model;
using System.Collections.Generic;

namespace chess.v4.engine.interfaces {

	public interface ICoordinateService {
		int AbsDiff(int piecePos, int newPiecePos);
		bool BreakAfterAction(bool ignoreKing, char blockingPiece, Color pieceColor);
		bool CanAttackPiece(Color pieceColor, char attackedPiece);
		bool CanAttackPiece(Color pieceColor, Piece attackedPiece);
		int CoordinatePairToPosition(int file, int rank);
		int CoordinateToPosition(string coordinate);
		bool DetermineCastleThroughCheck(List<Square> squares, List<Square> enemyAttacks, string fen, Color color, int kingPos, int rookPos);
		int FileToInt(char file);
		IEnumerable<Square> FindPiece(List<Square> squares, PieceType pieceType, Color color);
		char GetCharFromPieceType(PieceType pieceType, Color color);
		Color GetColorFromChar(char piece);
		List<Square> GetDiagonalLine(List<Square> squares, int position, DiagonalDirection direction, Color pieceColor, bool ignoreKing);
		List<Square> GetDiagonals(List<Square> squares, int position, Color pieceColor, bool ignoreKing = false);
		List<int> GetEntireFile(int file);
		List<int> GetEntireRank(int rank);
		int[] GetKingPositionsDuringCastle(int kingPos, int rookPos);
		char GetOppositeColor(char activeColor);
		Color GetOppositeColor(Color pieceColor);
		List<Square> GetOrthogonalLine(List<Square> squares, int position, Direction direction, Color pieceColor, bool ignoreKing);
		List<Square> GetOrthogonals(List<Square> squares, int position, Color pieceColor, bool ignoreKing = false);
		PieceType GetPieceTypeFromChar(char piece);
		char IntToFile(int file);
		bool IsDiagonalMove(int startPosition, int endPosition);
		bool IsOpposingKing(char piece, Color pieceColor);
		bool IsValidCoordinate(int position);
		string PositionToCoordinate(int position);
		int PositionToFile(int position);
		char PositionToFileChar(int position);
		int PositionToFileInt(int position);
		int PositionToRankInt(int position);
	}
}