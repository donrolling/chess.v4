using chess_engine.Models.Enums;
using chess_webapi.Models;

namespace chess_webapi.Models
{
    public class MoveRequestResource
    {
        public GameStateResource GameState { get; set; }
        public string Beginning { get; set; }
        public string Destination { get; set; }
        public PieceType? PiecePromotionType { get; set; }
    }
}