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
		private const string defaultCastlingAvailability = "KQkq";

		public ICoordinateService CoordinateService { get; }

		public NotationService(ICoordinateService coordinateService) {
			CoordinateService = coordinateService;
		}

		public List<Square> ApplyMoveToSquares(List<Square> squares, int piecePosition, int newPiecePosition) {
			var xs = squares.Clone();
			var square = xs.GetSquare(piecePosition);
			var newSquare = xs.Where(a => a.Index == newPiecePosition).First();
			var piece = square.Piece;
			square.Piece = null;
			newSquare.Piece = new Piece {
				Identity = piece.Identity,
				PieceType = piece.PieceType,
				Color = piece.Color
			};
			return xs;
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
							new Square {
								Index = index,
								Piece = NotationUtility.GetPieceFromCharacter(c)
							}
						);
						charIndex++;
					}
				}
			}
			return squares.OrderBy(a => a.Index).ToList();
		}

		public FEN_Record CreateNewFENFromGameState(GameState gameState, List<Square> squares, int piecePosition, int newPiecePosition) {
			//"rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
			var position = createNewPositionFromMatrix(squares);
			var castlingAvailability = getCastlingAvailability(squares, gameState.CastlingAvailability, piecePosition, newPiecePosition);
			var enPassantCoord = getEnPassantCoord(squares, gameState.ActiveColor, piecePosition, newPiecePosition);
			var halfmoveClock = gethalfmoveClock(gameState.Squares, gameState.HalfmoveClock, piecePosition, newPiecePosition);
			var fullmoveNumber = getFullmoveNumber(gameState.FullmoveNumber, gameState.ActiveColor);
			var fenParams = new string[6] { position, CoordinateService.Reverse(gameState.ActiveColor).ToString(), castlingAvailability, enPassantCoord, halfmoveClock, fullmoveNumber };
			var fen = string.Join(" ", fenParams);
			return new FEN_Record(fen);
		}

		private string getCastlingAvailability(List<Square> matrix, string castlingAvailability, int piecePosition, int newPiecePosition) {
			var square = matrix.Where(a => a.Index == newPiecePosition).First();
			var movingPiece = square.Piece;
			if (movingPiece.PieceType == PieceType.Rook || movingPiece.PieceType == PieceType.King) {
				switch (piecePosition) {
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
			}

			if (string.IsNullOrEmpty(castlingAvailability)) {
				return "-";
			} else {
				return castlingAvailability;
			}
		}

		/// <summary>
		/// Get the halfmove clock.
		/// </summary>
		/// <param name="squares">Must be the current matrix, not the new one.</param>
		/// <param name="halfmoveClock">Current halfmove clock.</param>
		/// <param name="piecePosition">Moving piece position.</param>
		/// <param name="newPiecePosition">Capture piece position.</param>
		/// <returns></returns>
		private string gethalfmoveClock(List<Square> squares, int halfmoveClock, int piecePosition, int newPiecePosition) {
			var movingPiece = squares.GetPiece(piecePosition);
			var capturePiece = squares.GetPiece(newPiecePosition); 
			//if we're captuing, or moving a pawn the clock resets
			if (capturePiece != null || (movingPiece.PieceType == PieceType.Pawn)) {
				return "0";
			}
			return (halfmoveClock + 1).ToString();
		}

		private string getFullmoveNumber(int fullmoveNumber, Color activeColor) {
			if (activeColor == Color.Black) {
				return (fullmoveNumber + 1).ToString();
			}
			return (fullmoveNumber).ToString();
		}

		private string getEnPassantCoord(List<Square> squares, Color activeColor, int piecePosition, int newPiecePosition) {
			var piece = squares.GetPiece(newPiecePosition);
			if (piece.PieceType == PieceType.Pawn) {
				var diff = Math.Abs(piecePosition - newPiecePosition);
				if (diff == 16) {
					var moveMarker = 8;
					if (activeColor == Color.White) { moveMarker = (moveMarker * -1); }
					var enPassantSquare = newPiecePosition + moveMarker;
					var enPassantCoord = this.CoordinateService.PositionToCoordinate(enPassantSquare);
					return enPassantCoord;
				}
			}
			return "-";
		}

		private string createNewPositionFromMatrix(List<Square> squares) {
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