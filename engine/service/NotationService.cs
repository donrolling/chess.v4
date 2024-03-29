﻿using Chess.v4.Engine.Extensions;
using Chess.v4.Engine.Interfaces;
using Chess.v4.Engine.Utility;
using Chess.v4.Models;
using Chess.v4.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chess.v4.Engine.Service
{
	public class NotationService : INotationService
	{
		private static List<PieceType> _castlingPieces { get; set; } = new List<PieceType> { PieceType.Rook, PieceType.King };
		private const int TwoRanks = 16;
		private readonly IPGNService _pgnService;

		public NotationService(IPGNService pgnService)
		{
			_pgnService = pgnService;
		}

		public List<Square> GetSquares(Snapshot fen)
		{
			var squares = new List<Square>();
			var rows = fen.PiecePlacement.Split('/');
			for (int i = 0; i < 8; i++)
			{
				int rowIndex = 7 - i;
				//leftSideIndex is the left side of the board, in numbers: 0 8 16 24 32 40 48 56
				int leftSideIndex = 8 * (rowIndex);
				int charIndex = 0;
				var row = rows[i];
				foreach (char c in row)
				{
					if (char.IsNumber(c))
					{
						int advanceSquare = 0;
						Int32.TryParse(c.ToString(), out advanceSquare);
						//gotta make empty squares
						for (int j = 0; j < advanceSquare; j++)
						{
							var index = leftSideIndex + charIndex + j;
							squares.Add(
								new Square(index, NotationEngine.PositionToCoordinate(index), null)
							);
						}
						//in FEN we move ahead the number of squares that the number says
						charIndex = charIndex + advanceSquare;
					} else
					{
						var index = leftSideIndex + charIndex;
						squares.Add(
							new Square(index, NotationEngine.PositionToCoordinate(index), NotationEngine.GetPieceFromCharacter(c))
						);
						charIndex++;
					}
				}
			}
			return squares.OrderBy(a => a.Index).ToList();
		}

		public void SetGameStateSnapshot(GameState oldGameState, GameState newGameState, StateInfo stateInfo, int piecePosition, int newPiecePosition)
		{
			var piece = newGameState.Squares.GetPiece(newPiecePosition);
			var fenPosition = getFENPosition(newGameState.Squares);
			var castlingAvailability = getCastlingAvailability(newGameState, newGameState.CastlingAvailability, piecePosition, newPiecePosition);
			var enPassantCoord = getEnPassantCoord(piece, newGameState.ActiveColor, piecePosition, newPiecePosition, oldGameState.Attacks);
			var newHalfmoveClock = getHalfmoveClock(oldGameState.Squares, oldGameState.HalfmoveClock, piecePosition, newPiecePosition);
			var activeColor = newGameState.ActiveColor.Reverse();
			newGameState.PiecePlacement = fenPosition;
			newGameState.ActiveColor = activeColor;
			newGameState.CastlingAvailability = castlingAvailability;
			newGameState.EnPassantTargetSquare = enPassantCoord;
			if (enPassantCoord != "-")
			{
				newGameState.EnPassantTargetPosition = NotationEngine.CoordinateToPosition(enPassantCoord);
			}
			newGameState.HalfmoveClock = newHalfmoveClock;
			//better to calculate this value after setting the ActiveColor
			var fullmoveNumber = getFullmoveNumber(newGameState.FullmoveNumber, newGameState.ActiveColor);
			newGameState.FullmoveNumber = fullmoveNumber;
			var pgnMove = stateInfo.IsPawnPromotion
				? _pgnService.SquarePairToPGNMove(oldGameState, oldGameState.ActiveColor, piecePosition, newPiecePosition, stateInfo.PawnPromotedTo)
				: _pgnService.SquarePairToPGNMove(oldGameState, oldGameState.ActiveColor, piecePosition, newPiecePosition);
			newGameState.PGNMoves.Add(pgnMove);
			newGameState.PGN = getUpdatedPGN(newGameState, oldGameState.ActiveColor, fullmoveNumber, pgnMove);
		}

		public void UpdateMatrix_PromotePiece(List<Square> squares, int newPiecePosition, Color pieceColor, char piecePromotedTo)
		{
			var pieceIdentity = pieceColor == Color.White ? char.ToUpper(piecePromotedTo) : char.ToLower(piecePromotedTo);
			var square = squares.Where(a => a.Index == newPiecePosition).First();
			var piece = square.Piece;
			square.Piece = new Piece(piece.PieceType, pieceColor);
		}

		private static string getUpdatedPGN(GameState newGameState, Color activeColor, int fullmoveNumber, string pgnMove)
		{
			var separator = activeColor == Color.White
							? $" {fullmoveNumber}. "
							: " ";
			var pgn = string.IsNullOrEmpty(newGameState.PGN)
				? $"{separator}{pgnMove}"
				: $"{newGameState.PGN}{separator}{pgnMove}";
			return pgn.Trim();
		}

		private string getCastlingAvailability(GameState gameState, string castlingAvailability, int oldPiecePosition, int newPiecePosition)
		{
			var square = gameState.Squares.GetSquare(newPiecePosition);
			if (!_castlingPieces.Contains(square.Piece.PieceType))
			{
				return gameState.CastlingAvailability;
			}
			switch (oldPiecePosition)
			{
				case 0: //R
					return castlingAvailability.Replace("Q", "");

				case 7: //R
					return castlingAvailability.Replace("K", "");

				case 56: //r
					return castlingAvailability.Replace("q", "");

				case 63: //r
					return castlingAvailability.Replace("k", "");

				case 4:  //K
					var retval = castlingAvailability.Replace("K", "").Replace("Q", "");
					return retval;

				case 60: //k
					var result = castlingAvailability.Replace("k", "").Replace("q", "");
					return result;
			}

			if (string.IsNullOrEmpty(castlingAvailability))
			{
				return "-";
			}
			return castlingAvailability;
		}

		private string getEnPassantCoord(Piece piece, Color activeColor, int piecePosition, int newPiecePosition, List<AttackedSquare> attacks)
		{
			if (piece.PieceType != PieceType.Pawn)
			{
				return "-";
			}
			// looking to see if a pawn jumped two ranks
			var diff = Math.Abs(piecePosition - newPiecePosition);
			if (diff != TwoRanks)
			{
				return "-";
			}

			// get the passed over square
			var moveMarker = activeColor == Color.White ? 8 : -8;
			var enPassantSquare = piecePosition + moveMarker;
			var enPassantCoordinate = NotationEngine.PositionToCoordinate(enPassantSquare);

			// here's the trick, the new FEN specs say that the en passant value should be empty
			// if no pawn can attack it
			var enemyPawnAttacksExist = attacks
				.Any(a =>
					a.AttackingSquare.Occupied
					&& a.AttackingSquare.Piece.PieceType == PieceType.Pawn
					&& a.AttackingSquare.Piece.Color != activeColor
					&& a.Index == enPassantSquare
				);

			return enemyPawnAttacksExist ? enPassantCoordinate : "-";
		}

		private string getFENPosition(List<Square> squares)
		{
			var position = new StringBuilder();
			for (int i = 0; i < 8; i++)
			{
				int leftSideIndex = 8 * (7 - i);
				var row = squares.Where(a => a.Index >= leftSideIndex && a.Index < leftSideIndex + 8);
				if (row != null && row.Any())
				{
					int missingPieceCount = 0;
					for (int j = 0; j < 8; j++)
					{
						var index = leftSideIndex + j;
						var square = squares.GetSquare(index);
						var piece = square.Piece;
						if (piece != null)
						{
							if (missingPieceCount > 0)
							{
								position.Append(missingPieceCount.ToString());
								missingPieceCount = 0;
							}
							position.Append(piece.Identity);
						} else
						{
							missingPieceCount += 1;
						}
					}
					if (missingPieceCount > 0)
					{
						position.Append(missingPieceCount.ToString());
					}
				} else
				{
					position.Append('8');
				}
				if (i < 7)
				{
					position.Append('/');
				}
			}
			return position.ToString();
		}

		private int getFullmoveNumber(int fullmoveNumber, Color activeColor)
		{
			if (activeColor == Color.White)
			{
				return fullmoveNumber + 1;
			}
			return fullmoveNumber;
		}

		/// <summary>
		/// Get the halfmove clock.
		/// </summary>
		/// <param name="squares">Must be the current matrix, not the new one.</param>
		/// <param name="halfmoveClock">Current halfmove clock.</param>
		/// <param name="piecePosition">Moving piece position.</param>
		/// <param name="newPiecePosition">Capture piece position.</param>
		/// <returns></returns>
		private int getHalfmoveClock(List<Square> squares, int halfmoveClock, int piecePosition, int newPiecePosition)
		{
			var movingPiece = squares.GetPiece(piecePosition);
			var capturePiece = squares.GetPiece(newPiecePosition);
			//if we're captuing, or moving a pawn the clock resets
			if (capturePiece != null || movingPiece.PieceType == PieceType.Pawn)
			{
				return 0;
			}
			return halfmoveClock + 1;
		}
	}
}