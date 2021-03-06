﻿using Chess.v4.Engine.Factory;
using Chess.v4.Models.Enums;
using System.Collections.Generic;

namespace Chess.v4.Models
{
    /// <summary>
    /// Definition
    ///A FEN "record" defines a particular game position, all in one text line and using only the ASCII character set. A text file with only FEN data records should have the file extension ".fen".
    ///A FEN record contains six fields. The separator between fields is a space. The fields are:
    ///1. Piece placement (from white's perspective). Each rank is described, starting with rank 8 and ending with rank 1; within each rank, the contents of each square are described from file "a" through file "h". Following the Standard Algebraic Notation (SAN), each piece is identified by a single letter taken from the standard English names (pawn = "P", knight = "N", bishop = "B", rook = "R", queen = "Q" and king = "K").[1] White pieces are designated using upper-case letters ("PNBRQK") while black pieces use lowercase ("pnbrqk"). Blank squares are noted using digits 1 through 8 (the number of blank squares), and "/" separates ranks.
    ///2. Active color. "w" means white moves next, "b" means black.
    ///3. Castling availability. If neither side can castle, this is "-". Otherwise, this has one or more letters: "K" (White can castle kingside), "Q" (White can castle queenside), "k" (Black can castle kingside), and/or "q" (Black can castle queenside).
    ///4. En passant target square in algebraic notation. If there's no en passant target square, this is "-". If a pawn has just made a two-square move, this is the position "behind" the pawn. This is recorded regardless of whether there is a pawn in position to make an en passant capture.[2]
    ///5. Halfmove clock: This is the number of halfmoves since the last pawn advance or capture. This is used to determine if a draw can be claimed under the fifty-move rule.
    ///6. Fullmove number: The number of the full move. It starts at 1, and is incremented after Black's move.
    ///Examples
    ///Here is the FEN for the starting position:
    ///rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1
    ///Here is the FEN after the move 1. e4:
    ///rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq e3 0 1
    ///And then after 1. ... c5:
    ///rnbqkbnr/pp1ppppp/8/2p5/4P3/8/PPPP1PPP/RNBQKBNR w KQkq c6 0 2
    ///And then after 2. Nf3:
    ///rnbqkbnr/pp1ppppp/8/2p5/4P3/5N2/PPPP1PPP/RNBQKB1R b KQkq - 1 2
    /// </summary>
    public class Snapshot
    {
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

        public string PGN { get; set; }

        public List<string> PGNMoves { get; set; } = new List<string>();
        
        public override string ToString()
        {
            return FenFactory.Create(this);
        }
    }
}