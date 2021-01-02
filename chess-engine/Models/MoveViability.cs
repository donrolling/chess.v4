using chess_engine.Models;

namespace chess_engine.Engine.Models
{
    public class MoveViability
    {
        public bool IsValidCoordinate { get; set; }
        public bool IsTeamPiece { get; set; }
        public Square SquareToAdd { get; set; }

        public MoveViability(bool isValidCoordinate, bool isTeamPiece, Square squareToAdd)
        {
            IsValidCoordinate = isValidCoordinate;
            IsTeamPiece = isTeamPiece;
            SquareToAdd = squareToAdd;
        }
    }
}