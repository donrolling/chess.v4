using Chess.v4.Models;
using Chess.v4.Models.Enums;
using System.Collections.Generic;

namespace Chess.v4.Engine.Interfaces
{
    public interface IPGNService
    {
        Square GetCurrentPositionFromPGNMove(GameState gameState, Piece piece, int newPiecePosition, string pgnMove, bool isCastle);

        bool IsRank(char potentialRank);

        (int piecePosition, int newPiecePosition, char promotedPiece) PGNMoveToSquarePair(GameState gameState, string pgnMove);

        List<string> PGNSplit(string pgn);

        List<string> PGNSplit(string pgn, bool mostConsise);

        string SquarePairToPGNMove(GameState gameState, Color playerColor, string startSquare, string endSquare, char promoteToPiece);
    }
}