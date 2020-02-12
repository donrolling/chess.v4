using Chess.v4.Models;

namespace Chess.v4.Engine.Models
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