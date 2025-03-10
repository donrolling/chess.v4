namespace engine.Interfaces;

using common.Responses;
using engine.Models.Enums;
using Models;

public interface IGameStateService
{
	OperationResult<GameState> Initialize(string fen = "");

	OperationResult<GameState> MakeMove(GameState gameState, int piecePosition, int newPiecePosition, PieceType? piecePromotionType = null);

	OperationResult<GameState> MakeMove(GameState gameState, string beginning, string destination, PieceType? piecePromotionType = null);

	OperationResult<GameState> MakeMove(GameState gameState, string pgnMove);
}