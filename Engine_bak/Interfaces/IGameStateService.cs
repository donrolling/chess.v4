using Chess.v4.Models;
using Chess.v4.Models.Enums;
using Common.Responses;

namespace Chess.v4.Engine.Interfaces
{
    public interface IGameStateService
    {
        OperationResult<GameState> Initialize(string fen = "");

        OperationResult<GameState> MakeMove(GameState gameState, int piecePosition, int newPiecePosition, PieceType? piecePromotionType = null);

        OperationResult<GameState> MakeMove(GameState gameState, string beginning, string destination, PieceType? piecePromotionType = null);

        OperationResult<GameState> MakeMove(GameState gameState, string pgnMove);
    }
}