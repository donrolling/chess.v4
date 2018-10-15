using chess.v4.engine.enumeration;
using chess.v4.engine.model;
using System.Collections.Generic;

namespace chess.v4.engine.interfaces {

	public interface ICoordinateService {
		int CoordinatePairToPosition(int file, int rank);

		int CoordinateToPosition(string coordinate);

		int FileToInt(char file);

		char IntToFile(int file);

		bool IsValidCoordinate(int position);

		string PositionToCoordinate(int position);

		int PositionToFile(int position);

		char PositionToFileChar(int position);

		int PositionToFileInt(int position);

		int PositionToRankInt(int position);

		Color Reverse(Color pieceColor);
	}
}