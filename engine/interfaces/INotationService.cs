using Chess.v4.Models;
using System.Collections.Generic;

namespace Chess.v4.Engine.Interfaces
{
    public interface INotationService
    {
        List<Square> GetSquaresFromFEN_Record(FEN_Record fen);

        void SetGameState_FEN(List<Square> squares, int halfmoveClock, GameState newGameState, int piecePosition, int newPiecePosition);
    }
}