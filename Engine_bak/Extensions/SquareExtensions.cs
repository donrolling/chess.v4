using Chess.v4.Models;
using Chess.v4.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Chess.v4.Engine.Extensions
{
    public static class SquareExtensions
    {
        public static List<T> Clone<T>(this IList<T> listToClone) where T : ICloneable
        {
            return listToClone.Select(item => (T)item.Clone()).ToList();
        }

        /// <summary>
        /// Might not return the piece you want if there are more than one of that type and color.
        /// </summary>
        /// <param name="squares"></param>
        /// <param name="pieceType"></param>
        /// <param name="pieceColor"></param>
        /// <returns></returns>
        public static Square FindPiece(this List<Square> squares, PieceType pieceType, Color pieceColor)
        {
            return squares.Where(a => a.Occupied && a.Piece.PieceType == pieceType && a.Piece.Color == pieceColor).FirstOrDefault();
        }

        public static IEnumerable<Square> FindPieces(this List<Square> squares, PieceType pieceType, Color pieceColor)
        {
            return squares.Where(a => a.Occupied && a.Piece.PieceType == pieceType && a.Piece.Color == pieceColor);
        }

        public static Piece GetPiece(this List<Square> squares, int piecePosition)
        {
            return squares.Where(a => a.Index == piecePosition).First().Piece;
        }

        public static Square GetSquare(this List<Square> squares, int piecePosition)
        {
            return squares.Where(a => a.Index == piecePosition).First();
        }

        public static bool Intersects(this List<Square> squares, int[] positions)
        {
            return positions.Intersect<int>(squares.Select(a => a.Index)).Any();
        }

        //public static bool Intersects(this List<Square> squares, int position) {
        //	return squares.Where(a => a.Index == position).Any();
        //}

        public static IEnumerable<Square> Occupied(this List<Square> squares)
        {
            return squares.Where(a => a.Piece != null);
        }
    }
}