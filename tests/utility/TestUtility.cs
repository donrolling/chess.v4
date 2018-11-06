using chess.v4.models.enumeration;
using chess.v4.engine.extensions;
using chess.v4.engine.interfaces;
using chess.v4.models;
using chess.v4.engine.reference;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tests.Utility {
	internal class TestUtility {

		internal static GameState GetGameState(IGameStateService gameStateService, string fen = "") {
			if (string.IsNullOrEmpty(fen)) {
				fen = GeneralReference.Starting_FEN_Position;
			}
			var gamestateResult = gameStateService.Initialize(fen);
			Assert.IsTrue(gamestateResult.Success);
			var gamestate = gamestateResult.Result;
			return gamestate;
		}

		internal static void ListContainsSquare(List<AttackedSquare> attacks, PieceType pieceType, int x) {
			var msg = $"{ pieceType } should be able to attack square: { x }";
			var square = attacks.GetSquareMaybe(x);
			Assert.IsNotNull(square, msg);
		}

		internal static void ListContainsSquares(List<AttackedSquare> attacks, List<int> squares, PieceType pieceType) {
			var found = from a in attacks
						join s in squares on a.Index equals s
						select s;
			var notFound = squares.Except(found);
			var nf = string.Join(",", notFound.Select(n => n.ToString()).ToArray());
			var msg = $"{ pieceType } should be able to attack squares: { nf }";
			Assert.IsFalse(notFound.Any(), msg);
		}

		internal static void ListContainsSquares(List<Square> diagonalLine, List<int> squares) {
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