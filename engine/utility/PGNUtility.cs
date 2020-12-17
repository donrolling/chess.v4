using Chess.v4.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chess.v4.Engine.Utility
{
    public static class PGNUtility
    {
        public static char GetPieceCharFromPieceTypeColor(PieceType piece, Color playerColor)
        {
            char pieceChar = 'a';
            switch (piece)
            {
                case PieceType.Bishop:
                    pieceChar = 'b';
                    break;

                case PieceType.Pawn:
                    pieceChar = 'p';
                    break;

                case PieceType.King:
                    pieceChar = 'k';
                    break;

                case PieceType.Knight:
                    pieceChar = 'n';
                    break;

                case PieceType.Queen:
                    pieceChar = 'q';
                    break;

                case PieceType.Rook:
                    pieceChar = 'r';
                    break;
            }
            if (playerColor == Color.White)
            {
                pieceChar = char.ToUpper(pieceChar);
            }
            return pieceChar;
        }

        public static PieceType GetPieceTypeFromPGNMove(string pgnMove)
        {
            if (pgnMove.Length == 2)
            {
                return PieceType.Pawn;
            }
            if (pgnMove == "O-O" || pgnMove == "O-O-O")
            {
                return PieceType.King;
            }
            var piece = pgnMove[0]; //should not capitalize this to check because all piece disambiguity notation is caps, therefore a file indicator will not be.
            switch (piece)
            {
                case 'B':
                case 'b':
                    return PieceType.Bishop;

                case 'K':
                case 'k':
                    return PieceType.King;

                case 'N':
                case 'n':
                    return PieceType.Knight;

                case 'Q':
                case 'q':
                    return PieceType.Queen;

                case 'R':
                case 'r':
                    return PieceType.Rook;
            }
            return PieceType.Pawn;
        }
    }
}
