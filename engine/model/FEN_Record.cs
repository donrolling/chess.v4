using chess.v4.engine.enumeration;
using System;

namespace chess.v4.engine.model {

	/// <summary>
	/// Starting Position:
	/// rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1
	/// </summary>
	public class FEN_Record {

		//Active colour. "w" means White moves next, "b" means Black.
		public Color ActiveColor { get; set; }

		//Castling availability. If neither side can castle, this is "-".
		//Otherwise, this has one or more letters: "K" (White can castle kingside),
		//"Q" (White can castle queenside), "k" (Black can castle kingside), and/or "q" (Black can castle queenside).
		public string CastlingAvailability { get; set; }

		//Mirrors EnPassantTargetSquare, but uses a board index rather than the algebraic notation
		public int EnPassantTargetPosition { get; set; }

		//En passant target square in algebraic notation.If there's no en passant target square, this is "-".
		//If a pawn has just made a two-square move, this is the position "behind" the pawn.
		//This is recorded regardless of whether there is a pawn in position to make an en passant capture.[2]
		public string EnPassantTargetSquare { get; set; }

		//Fullmove number: The number of the full move. It starts at 1, and is incremented after Black's move.
		public int FullmoveNumber { get; set; }

		//Halfmove clock: This is the number of halfmoves since the last capture or pawn advance.
		//This is used to determine if a draw can be claimed under the fifty-move rule.
		public int HalfmoveClock { get; set; }

		//Piece placement(from white's perspective). Each rank is described,
		//starting with rank 8 and ending with rank 1; within each rank, the contents
		//of each square are described from file "a" through file "h".
		//Following the Standard Algebraic Notation (SAN), each piece is identified by a
		//single letter taken from the standard English names
		//(pawn = "P", knight = "N", bishop = "B", rook = "R", queen = "Q" and king = "K").[1]
		//White pieces are designated using upper-case letters ("PNBRQK") while black pieces use
		//lowercase ("pnbrqk"). Empty squares are noted using digits 1 through 8 (the number of empty squares), and "/" separates ranks.
		public string PiecePlacement { get; set; }

		public FEN_Record() {
		}

		public FEN_Record(string fen) {
			var gameData = fen.Split(' ');
			this.PiecePlacement = gameData[0];

			this.ActiveColor = gameData[1][0] == 'w' ? Color.White : Color.Black;

			this.CastlingAvailability = gameData[2];

			this.EnPassantTargetSquare = gameData[3];

			var enPassantTargetPosition = -1;
			Int32.TryParse(gameData[3], out enPassantTargetPosition);
			this.EnPassantTargetPosition = enPassantTargetPosition;

			int halfmoveClock = 0;
			Int32.TryParse(gameData[4], out halfmoveClock);
			this.HalfmoveClock = halfmoveClock;

			int fullmoveNumber = 0;
			Int32.TryParse(gameData[5], out fullmoveNumber);
			this.FullmoveNumber = fullmoveNumber;
		}

		public static string ConvertToString(FEN_Record fenRecord) {
			var activeColor = fenRecord.ActiveColor == Color.White ? "w" : "b";
			return $"{ fenRecord.PiecePlacement } { activeColor } { fenRecord.CastlingAvailability } { fenRecord.EnPassantTargetSquare } { fenRecord.HalfmoveClock } { fenRecord.FullmoveNumber }";
		}

		public override string ToString() {
			return ConvertToString(this);
		}
	}
}