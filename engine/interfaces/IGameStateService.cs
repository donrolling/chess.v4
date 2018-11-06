using chess.v4.models.enumeration;
using chess.v4.models;
using Common;
using Common.Models;

namespace chess.v4.engine.interfaces {

	public interface IGameStateService {

		Envelope<GameState> Initialize(string fen = "");

		Envelope<GameState> MakeMove(GameState gameState, int piecePosition, int newPiecePosition, PieceType? piecePromotionType = null);

		Envelope<GameState> MakeMove(GameState gameState, string beginning, string destination, PieceType? piecePromotionType = null);
	}
}