using chess_engine.Engine.Factory;
using System.Collections.Generic;

namespace chess_engine.Models
{
    public class GameState : Snapshot
    {
        public List<AttackedSquare> Attacks { get; set; } = new List<AttackedSquare>();
        public List<Snapshot> History { get; set; } = new List<Snapshot>();
        public List<Square> Squares { get; set; } = new List<Square>();
        public StateInfo StateInfo { get; set; }

        public GameState()
        {
        }

        public GameState(Snapshot snapshot)
        {
            this.PiecePlacement = snapshot.PiecePlacement;
            this.ActiveColor = snapshot.ActiveColor;
            this.CastlingAvailability = snapshot.CastlingAvailability;
            this.EnPassantTargetSquare = snapshot.EnPassantTargetSquare;
            this.HalfmoveClock = snapshot.HalfmoveClock;
            this.FullmoveNumber = snapshot.FullmoveNumber;
        }

        public override string ToString()
        {
            return FenFactory.Create(this);
        }
    }
}