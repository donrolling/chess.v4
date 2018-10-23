using chess.v4.engine.enumeration;
using chess.v4.engine.extensions;
using chess.v4.engine.interfaces;
using chess.v4.engine.model;
using chess.v4.engine.utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace chess.v4.engine.service {

	public class NotationService : INotationService {
		private static List<PieceType> _castlingPieces { get; set; } = new List<PieceType> { PieceType.Rook, PieceType.King };
		private const string defaultCastlingAvailability = "KQkq";

		public NotationService() {
		}

		public List<Square> GetSquaresFromFEN_Record(FEN_Record fen) {
			var squares = new List<Square>();
			var rows = fen.PiecePlacement.Split('/');
			for (int i = 0; i < 8; i++) {
				int rowIndex = 7 - i;
				//leftSideIndex is the left side of the board, in numbers: 0 8 16 24 32 40 48 56
				int leftSideIndex = 8 * (rowIndex);
				int charIndex = 0;
				var row = rows[i];
				foreach (char c in row) {
					if (char.IsNumber(c)) {
						int advanceSquare = 0;
						Int32.TryParse(c.ToString(), out advanceSquare);
						//gotta make empty squares
						for (int j = 0; j < advanceSquare; j++) {
							squares.Add(
								new Square {
									Index = leftSideIndex + charIndex + j
								}
							);
						}
						//in FEN we move ahead the number of squares that the number says
						charIndex = charIndex + advanceSquare;
					} else {
						var index = leftSideIndex + charIndex;
						squares.Add(
							new Square(index,
								NotationUtility.PositionToCoordinate(index),
								NotationUtility.GetPieceFromCharacter(c)
							)
						);
						charIndex++;
					}
				}
			}
			return squares.OrderBy(a => a.Index).ToList();
		}

		public void SetGameState_FEN(GameState gameState, GameState newGameState, int piecePosition, int newPiecePosition) {
			var position = getPiecePosition(newGameState.Squares);
			var castlingAvailability = getCastlingAvailability(newGameState, newGameState.CastlingAvailability, piecePosition, newPiecePosition);
			var enPassantCoord = getEnPassantCoord(newGameState.Squares, newGameState.ActiveColor, piecePosition, newPiecePosition);
			//use old gamestate for halfmove clock
			var halfmoveClock = getHalfmoveClock(gameState.Squares, gameState.HalfmoveClock, piecePosition, newPiecePosition);
			var activeColor = newGameState.ActiveColor.Reverse();
			newGameState.PiecePlacement = position;
			newGameState.ActiveColor = activeColor;
			newGameState.CastlingAvailability = castlingAvailability;
			newGameState.EnPassantTargetSquare = enPassantCoord;
			newGameState.HalfmoveClock = halfmoveClock;
			//better to calculate this value after setting the ActiveColor
			var fullmoveNumber = getFullmoveNumber(newGameState.FullmoveNumber, newGameState.ActiveColor);
			newGameState.FullmoveNumber = fullmoveNumber;
		}

		public void UpdateMatrix_PromotePiece(List<Square> squares, int newPiecePosition, Color pieceColor, char piecePromotedTo) {
			var pieceIdentity = pieceColor == Color.White ? char.ToUpper(piecePromotedTo) : char.ToLower(piecePromotedTo);
			var square = squares.Where(a => a.Index == newPiecePosition).First();
			var piece = square.Piece;
			square.Piece = new Piece {
				Identity = pieceIdentity,
				PieceType = piece.PieceType,
				Color = pieceColor
			};
		}

		private string getCastlingAvailability(GameState newGameState, string castlingAvailability, int oldPiecePosition, int newPiecePosition) {
			var square = newGameState.Squares.GetSquare(newPiecePosition);
			if (!_castlingPieces.Contains(square.Piece.PieceType)) {
				return newGameState.CastlingAvailability;
			}
			switch (oldPiecePosition) {
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

			if (string.IsNullOrEmpty(castlingAvailability)) {
				return "-";
			}
			return castlingAvailability;
		}

		private string getEnPassantCoord(List<Square> squares, Color activeColor, int piecePosition, int newPiecePosition) {
			var piece = squares.GetPiece(newPiecePosition);
			if (piece.PieceType != PieceType.Pawn) {
				return "-";
			}
			var diff = Math.Abs(piecePosition - newPiecePosition);
			if (diff != 16) {//16 is two rows
				return "-";
			}
			var moveMarker = activeColor == Color.White ? 8 : -8;
			var enPassantSquare = piecePosition + moveMarker;
			return NotationUtility.PositionToCoordinate(enPassantSquare);
		}

		private int getFullmoveNumber(int fullmoveNumber, Color activeColor) {
			if (activeColor == Color.White) {
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
		private int getHalfmoveClock(List<Square> squares, int halfmoveClock, int piecePosition, int newPiecePosition) {
			var movingPiece = squares.GetPiece(piecePosition);
			var capturePiece = squares.GetPiece(newPiecePosition);
			//if we're captuing, or moving a pawn the clock resets
			if (capturePiece != null || (movingPiece.PieceType == PieceType.Pawn)) {
				return 0;
			}
			return halfmoveClock + 1;
		}

		private string getPiecePosition(List<Square> squares) {
			var position = new StringBuilder();
			for (int i = 0; i < 8; i++) {
				int leftSideIndex = 8 * (7 - i);
				var row = squares.Where(a => a.Index >= leftSideIndex && a.Index < leftSideIndex + 8);
				if (row != null && row.Any()) {
					int missingPieceCount = 0;
					for (int j = 0; j < 8; j++) {
						var index = leftSideIndex + j;
						var square = squares.GetSquare(index);
						var piece = square.Piece;
						if (piece != null) {
							if (missingPieceCount > 0) {
								position.Append(missingPieceCount.ToString());
								missingPieceCount = 0;
							}
							position.Append(piece.Identity);
						} else {
							missingPieceCount += 1;
						}
					}
					if (missingPieceCount > 0) {
						position.Append(missingPieceCount.ToString());
					}
				} else {
					position.Append('8');
				}
				if (i < 7) {
					position.Append('/');
				}
			}
			return position.ToString();
		}
	}
}