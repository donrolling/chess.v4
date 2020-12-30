using Chess.v4.Models;
using System.Collections.Generic;

namespace Website.Models
{
    public class GameStateResource : Snapshot
    {
        public List<AttackedSquare> Attacks { get; set; } = new List<AttackedSquare>();
        public List<Snapshot> History { get; set; } = new List<Snapshot>();
        public List<Square> Squares { get; set; } = new List<Square>();
        public StateInfo StateInfo { get; set; }
        public string FEN { get; set; }
    }
}