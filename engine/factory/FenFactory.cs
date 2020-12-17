using Chess.v4.Engine.Utility;
using Chess.v4.Models;
using Chess.v4.Models.Enums;
using System;

namespace Chess.v4.Engine.Factory
{
    public static class FenFactory
    {
        public static FEN_Record Create(string fen)
        {
            // 5 could be a bad number
            if (fen.Length < 5 || !fen.Contains(' '))
            {
                return null;
            }
            var fenRecord = new FEN_Record();
            var gameData = fen.Split(' ');
            fenRecord.PiecePlacement = gameData[0];

            fenRecord.ActiveColor = gameData[1][0] == 'w' ? Color.White : Color.Black;

            fenRecord.CastlingAvailability = gameData[2];

            fenRecord.EnPassantTargetSquare = gameData[3];
            if (fenRecord.EnPassantTargetSquare != "-")
            {
                fenRecord.EnPassantTargetPosition = NotationEngine.CoordinateToPosition(fenRecord.EnPassantTargetSquare);
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