﻿using chess.v4.engine.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace chess.v4.engine.extensions {
	public static class SquareExtensions {
		public static Square GetSquare(this List<Square> squares, int piecePosition) {
			return squares.Where(a => a.Index == piecePosition).First();
		}

		public static Piece GetPiece(this List<Square> squares, int piecePosition) {
			return squares.Where(a => a.Index == piecePosition).First().Piece;
		}

		public static bool Intersects(this List<Square> squares, int[] positions) {
			return positions.Intersect<int>(squares.Select(a => a.Index)).Any();
		}

		public static bool Intersects(this List<Square> squares, int position) {
			return squares.Where(a => a.Index == position).Any();
		}
	}
}
