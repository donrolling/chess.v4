using chess.v4.engine.enumeration;
using chess.v4.engine.model;
using System.Collections.Generic;

namespace chess.v4.engine.interfaces {

	public interface ICoordinateService {
		int CoordinatePairToPosition(int file, int rank);

		int CoordinateToPosition(string coordinate);

		int FileToInt(char file);

		//List<Square> GetDiagonalLine(GameState gameState, Square square, DiagonalDirection direction, bool ignoreKing);

		//List<Square> GetDiagonals(GameState gameState, Square square, bool ignoreKing = false);

		//List<int> GetEntireFile(int file);

		//List<int> GetEntireRank(int rank);
		
		//List<Square> GetOrthogonalLine(GameState gameState, Square square, Direction direction, bool ignoreKing = false);

		//List<Square> GetOrthogonals(GameState gameState, Square square, bool ignoreKing = false);

		char IntToFile(int file);

		bool IsDiagonalMove(int startPosition, int endPosition);

		bool IsValidCoordinate(int position);

		string PositionToCoordinate(int position);

		int PositionToFile(int position);

		char PositionToFileChar(int position);

		int PositionToFileInt(int position);

		int PositionToRankInt(int position);

		Color Reverse(Color pieceColor);
	}
}