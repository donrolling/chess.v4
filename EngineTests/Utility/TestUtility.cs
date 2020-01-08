using Chess.v4.Engine.Extensions;
using Chess.v4.Engine.Interfaces;
using Chess.v4.Engine.Reference;
using Chess.v4.Models;
using Chess.v4.Models.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace Tests.Utility
{
    internal class TestUtility
    {
        internal static GameState GetGameState(IGameStateService gameStateService, string fen = "")
        {
            if (string.IsNullOrEmpty(fen))
            {
                fen = GeneralReference.Starting_FEN_Position;
            }
            var gamestateResult = gameStateService.Initialize(fen);
            Assert.IsTrue(gamestateResult.Success);
            var gamestate = gamestateResult.Result;
            return gamestate;
        }

        internal static void ListContainsSquare(List<AttackedSquare> attacks, PieceType pieceType, int x)
        {
            var msg = $"{ pieceType } should be able to attack square: { x }";
            var square = attacks.GetSquareMaybe(x);
            Assert.IsNotNull(square, msg);
        }

        internal static void ListContainsSquares(List<AttackedSquare> attacks, List<int> squares, PieceType pieceType)
        {
            var found = from a in attacks
                        join s in squares on a.Index equals s
                        select s;
            var notFound = squares.Except(found);
            var nf = string.Join(",", notFound.Select(n => n.ToString()).ToArray());
            var msg = $"{ pieceType } should be able to attack squares: { nf }";
            Assert.IsFalse(notFound.Any(), msg);
        }

        internal static void ListContainsSquares(List<Square> diagonalLine, List<int> squares)
        {
            var found = from a in diagonalLine
                        join s in squares on a.Index equals s
                        select s;
            var notFound = squares.Except(found);
            var nf = string.Join(",", notFound.Select(n => n.ToString()).ToArray());
            var msg = $"List should contian squares: { nf }";
            Assert.IsFalse(notFound.Any(), msg);
        }
    }
}