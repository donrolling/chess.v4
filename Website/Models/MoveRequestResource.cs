using Chess.v4.Models.Enums;

namespace Website.Models
{
    public class MoveRequestResource
    {
        public GameStateResource GameState { get; set; }
        public string Beginning { get; set; }
        public string Destination { get; set; }
        public PieceType? PiecePromotionType { get; set; }
    }
}