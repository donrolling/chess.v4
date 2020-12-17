using Chess.v4.Models;
using System.Collections.Generic;

namespace Chess.v4.Engine.Interfaces
{
    public interface INotationService
    {
        List<Square> GetSquaresFromFEN_Record(Snapshot fen);

        void SetGameStateSnapshot(GameState oldGameState, GameState newGameState, StateInfo stateInfo, int piecePosition, int newPiecePosition);
    }
}