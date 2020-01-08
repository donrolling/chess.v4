using System.Collections.Generic;

namespace Chess.v4.Models
{
    public class GameState : FEN_Record
    {
        public List<AttackedSquare> Attacks { get; set; } = new List<AttackedSquare>();
        public List<FEN_Record> FEN_Records { get; set; } = new List<FEN_Record>();
        public string PGN { get; set; }
        public List<Square> Squares { get; set; } = new List<Square>();
        public StateInfo StateInfo { get; set; }

        public GameState()
        {
        }

        public GameState(FEN_Record fenRecord)
        {
            this.PiecePlacement = fenRecord.PiecePlacement;
            this.ActiveColor = fenRecord.ActiveColor;
            this.CastlingAvailability = fenRecord.CastlingAvailability;
            this.EnPassantTargetSquare = fenRecord.EnPassantTargetSquare;
            this.HalfmoveClock = fenRecord.HalfmoveClock;
            this.FullmoveNumber = fenRecord.FullmoveNumber;
        }

        public override string ToString()
        {
            return FEN_Record.ConvertToString(this);
        }
    }
}