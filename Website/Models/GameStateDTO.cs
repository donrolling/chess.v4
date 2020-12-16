using Chess.v4.Models;
using System.Collections.Generic;

namespace Website.Models
{
    public class GameStateDTO : FEN_Record
    {
        public List<AttackedSquare> Attacks { get; set; } = new List<AttackedSquare>();
        public List<FEN_Record> FEN_Records { get; set; } = new List<FEN_Record>();
        public List<Square> Squares { get; set; } = new List<Square>();
        public StateInfo StateInfo { get; set; }
        public string PGN { get; set; }
        public string FEN { get; set; }
    }
}