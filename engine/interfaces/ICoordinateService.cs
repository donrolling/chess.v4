using chess.v4.engine.enumeration;
using chess.v4.engine.model;
using System.Collections.Generic;

namespace chess.v4.engine.interfaces {

	public interface ICoordinateService {
		List<DiagonalDirection> DiagonalLines { get; }
		List<Direction> OrthogonalLines { get; }
				
		bool BreakAfterAction(bool ignoreKing, char blockingPiece, Color pieceColor);
		bool CanAttackPiece(Color pieceColor, char attackedPiece);
		bool CanAttackPiece(Color pieceColor, Piece attackedPiece);
		int CoordinatePairToPosition(int file, int rank);
		int CoordinateToPosition(string coordinate);
		bool DetermineCastleThroughCheck(GameState gameState, List<Square> enemyAttacks, int kingPos, int rookPos);
		int FileToInt(char file);
		IEnumerable<Square> FindPiece(List<Square> squares, PieceType pieceType, Color color);
		char GetCharFromPieceType(PieceType pieceType, Color color);
		Color GetColorFromChar(char piece);
		List<Square> GetDiagonalLine(GameState gameState, Square square, DiagonalDirection direction, bool ignoreKing);
		List<Square> GetDiagonals(GameState gameState, Square square, bool ignoreKing = false);
		List<int> GetEntireFile(int file);
		List<int> GetEntireRank(int rank);
		int[] GetKingPositionsDuringCastle(int kingPos, int rookPos);
		char GetOppositeColor(char activeColor);
		Color Reverse(Color pieceColor);
		List<Square> GetOrthogonalLine(GameState gameState, Square square, Direction direction, bool ignoreKing = false);
		List<Square> GetOrthogonals(GameState gameState, Square square, bool ignoreKing = false);
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