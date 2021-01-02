using chess_engine.Engine.Utility;
using chess_engine.Models;
using chess_engine.Models.Enums;
using System;

namespace chess_engine.Engine.Factory
{
    public static class FenFactory
    {
        public static Snapshot Create(string fen)
        {
            // 5 could be a bad number
            if (fen.Length < 5 || !fen.Contains(' '))
            {
                return null;
            }
            var fenRecord = new Snapshot();
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

        public static string Create(Snapshot snapshot)
        {
            var activeColor = snapshot.ActiveColor == Color.White ? "w" : "b";
            return $"{ snapshot.PiecePlacement } { activeColor } { snapshot.CastlingAvailability } { snapshot.EnPassantTargetSquare } { snapshot.HalfmoveClock } { snapshot.FullmoveNumber }";
        }
    }
}