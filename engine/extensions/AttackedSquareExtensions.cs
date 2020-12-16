using Chess.v4.Models;
using Chess.v4.Models.Enums;
using System.Collections.Generic;
using System.Linq;

namespace Chess.v4.Engine.Extensions
{
    public static class AttackedSquareExtensions
    {
        /// <summary>
        /// Might not return the piece you want if there are more than one of that type and color.
        /// </summary>
        /// <param name="squares"></param>
        /// <param name="pieceType"></param>
        /// <param name="pieceColor"></param>
        /// <returns></returns>
        public static AttackedSquare FindPiece(this List<AttackedSquare> squares, PieceType pieceType, Color pieceColor)
        {
            return squares.Where(a => a.Occupied && a.Piece.PieceType == pieceType && a.Piece.Color == pieceColor).FirstOrDefault();
        }

        public static IEnumerable<AttackedSquare> FindPieces(this List<AttackedSquare> squares, PieceType pieceType, Color pieceColor)
        {
            return squares.Where(a => a.Occupied && a.Piece.PieceType == pieceType && a.Piece.Color == pieceColor);
        }

        public static IEnumerable<AttackedSquare> GetAttacks(this List<AttackedSquare> squares, int piecePosition)
        {
            return squares.Where(a => a.AttackingSquare.Index == piecePosition);
        }

        public static IEnumerable<AttackedSquare> GetPositionAttacksOnPosition(this List<AttackedSquare> squares, int piecePosition, int newPiecePosition)
        {
            return squares.Where(a => a.AttackingSquare.Index == piecePosition && a.Index == newPiecePosition);
        }

        public static Piece GetPiece(this List<AttackedSquare> squares, int piecePosition)
        {
            return squares.Where(a => a.Index == piecePosition).First().Piece;
        }

        public static AttackedSquare GetSquare(this List<AttackedSquare> squares, int piecePosition)
        {
            return squares.Where(a => a.Index == piecePosition).First();
        }

        public static AttackedSquare GetSquareMaybe(this List<AttackedSquare> squares, int piecePosition)
        {
            return squares.Where(a => a.Index == piecePosition).FirstOrDefault();
        }

        public static bool Intersects(this List<AttackedSquare> squares, int[] positions)
        {
            return positions.Intersect(squares.Select(a => a.Index)).Any();
        }

        public static bool Intersects(this List<AttackedSquare> squares, int position)
        {
            return squares.Where(a => a.Index == position).Any();
        }

        public static IEnumerable<AttackedSquare> Occupied(this List<AttackedSquare> squares)
        {
            return squares.Where(a => a.Piece != null);
        }
    }
}