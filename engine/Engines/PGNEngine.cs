using chess_engine.Models.Enums;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace chess_engine.Engine.Utility
{
	public static class PGNEngine
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

		public static List<string> PGNSplit(string pgn)
		{
			if (string.IsNullOrEmpty(pgn))
			{ return null; }

			var regex = @"\d{1,3}\.";
			var splitResult = Regex.Split(pgn.Trim(), regex);
			return splitResult.ToList();
		}

		public static List<string> PGNSplit(string pgn, bool mostConsise)
		{
			if (string.IsNullOrEmpty(pgn))
			{ return null; }

			var pgnData = PGNSplit(pgn);
			if (!mostConsise)
			{ return pgnData; }

			if (pgnData != null && pgnData.Any())
			{
				var iterationData = pgnData.ToList();
				var emptyStuffs = iterationData.Where(a => a == " " || string.IsNullOrEmpty(a)).ToList();
				if (emptyStuffs != null && emptyStuffs.Any())
				{
					foreach (var item in emptyStuffs)
					{
						iterationData.Remove(item);
					}
				}
				var returnValue = new List<string>();

				foreach (var item in iterationData)
				{
					var movePair = item.Trim().Split(' ');
					returnValue.Add(movePair[0]);
					if (movePair.Length > 1)
					{
						returnValue.Add(movePair[1]);
					}
				}
				return returnValue;
			}
			return null;
		}
	}
}