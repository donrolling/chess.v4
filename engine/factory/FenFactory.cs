using chess.v4.engine.Utility;
using chess.v4.models;
using chess.v4.models.enumeration;
using System;

namespace chess.v4.engine.factory {

	public static class FenFactory {

		public static FEN_Record Create(string fen) {
			var fenRecord = new FEN_Record();
			var gameData = fen.Split(' ');
			fenRecord.PiecePlacement = gameData[0];

			fenRecord.ActiveColor = gameData[1][0] == 'w' ? Color.White : Color.Black;

			fenRecord.CastlingAvailability = gameData[2];

			fenRecord.EnPassantTargetSquare = gameData[3];
			if (fenRecord.EnPassantTargetSquare != "-") {
				fenRecord.EnPassantTargetPosition = NotationUtility.CoordinateToPosition(fenRecord.EnPassantTargetSquare);
			}

			var enPassantTargetPosition = -1;
			Int32.TryParse(gameData[3], out enPassantTargetPosition);
			fenRecord.EnPassantTargetPosition = enPassantTargetPosition;

			int halfmoveClock = 0;
			Int32.TryParse(gameData[4], out halfmoveClock);
			fenRecord.HalfmoveClock = halfmoveClock;

			int fullmoveNumber = 0;
			Int32.TryParse(gameData[5], out fullmoveNumber);
			fenRecord.FullmoveNumber = fullmoveNumber;

			return fenRecord;
		}
	}
}