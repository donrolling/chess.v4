using Chess.v4.Models.Enums;
using System.Collections.Generic;

namespace Chess.v4.Engine.Reference
{
    public static class GeneralReference
    {
        public static List<DiagonalDirection> DiagonalLines { get; } = new List<DiagonalDirection> { DiagonalDirection.UpLeft, DiagonalDirection.UpRight, DiagonalDirection.DownLeft, DiagonalDirection.DownRight };

        public static List<Direction> OrthogonalLines { get; } = new List<Direction> { Direction.RowUp, Direction.RowDown, Direction.FileUp, Direction.FileDown };

        public const string Files = "abcdefgh";

        public const string Starting_FEN_Position = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
    }
}