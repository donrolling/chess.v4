using chess.v4.engine.enumeration;
using chess.v4.engine.extensions;
using chess.v4.engine.interfaces;
using chess.v4.engine.model;
using chess.v4.engine.utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace chess.v4.engine.service {

	// Pawn promotions are notated by appending an "=" to the destination square, followed by the piece the pawn is promoted to.
	// "e8=Q". If the move is a checking move, the plus sign "+" is also appended;
	// if the move is a checkmating move, the number sign "#" is appended instead. For example: "e8=Q#".
	// kingside castling is indicated by the sequence "O-O"; queenside castling is indicated by the sequence "O-O-O"
	public class PGNService : IPGNService {
		public IDiagonalService DiagonalService { get; }
		public IOrthogonalService OrthogonalService { get; }
		public const char NullPiece = '-';
		public const char PawnPromotionIndicator = '=';

		public PGNService(IDiagonalService diagonalService, IOrthogonalService orthogonalService) {
			DiagonalService = diagonalService;
			OrthogonalService = orthogonalService;
		}

		public Square GetCurrentPositionFromPGNMove(GameState gameState, Piece piece, int newPiecePosition, string pgnMove) {
			var potentialSquares = gameState.Attacks.Where(a => a.Index == newPiecePosition);
			if (!potentialSquares.Any()) {
				throw new Exception("No squares found.");
			}
			if (potentialSquares.Count() == 1) {
				return potentialSquares.First().AttackerSquare;
			}
			//differentiate
			var potentialPositions = from s in gameState.Squares
									 join p in potentialSquares on s.Index equals p.AttackerSquare.Index
									 where s.Piece.Identity == piece.Identity
									 select p;
			if (!potentialPositions.Any()) {
				throw new Exception("No squares found.");
			}
			//x means capture and shouldn't be used in the equation below
			var capture = isCapture(pgnMove);
			var check = isCheck(pgnMove);
			var castleKingside = isCastleKingside(pgnMove);
			var castleQueenside = isCastleQueenside(pgnMove);
			var isCastle = castleKingside || castleQueenside;
			var newPgnMove = pgnMove.Replace("x", "").Replace("+", "");
			if (isCastle) {
				return getOriginationPositionForCastling(gameState, piece.Color);
			}

			//todo: refactor to eliminate redundancy
			//look at the beginning of the pgnMove string to determine which of the pieces are the one that should be moved.
			//this should only happen if there are two pieces of the same type that can attack here.
			var moveLength = newPgnMove.Length;
			switch (moveLength) {
				case 2:
					return pgnLength2(piece, potentialPositions, capture);

				case 3: //this should be a pawn attack that can be made by two pawns
					return pgnLength3(potentialPositions, newPgnMove);

				case 4: //this would be any other piece
					return pgnLength4(gameState, potentialPositions, newPgnMove);

				case 5: //we have rank and file, so just find the piece. this should be very rare
					return pgnLength5(gameState, newPgnMove);

				default:
					throw new Exception("Failed to find square.");
			}
		}

		public char GetPieceCharFromPieceTypeColor(PieceType piece, Color playerColor) {
			char pieceChar = 'a';
			switch (piece) {
				case PieceType.Bishop:
					pieceChar = 'b';
					break;

				case PieceType.Pawn:
					pieceChar = 'p';
					break;

				case PieceType.King:
					pieceChar = 'k';
					break;

				case PieceType.Knight:
					pieceChar = 'n';
					break;

				case PieceType.Queen:
					pieceChar = 'q';
					break;

				case PieceType.Rook:
					pieceChar = 'r';
					break;
			}
			if (playerColor == Color.White) {
				pieceChar = char.ToUpper(pieceChar);
			}
			return pieceChar;
		}

		public PieceType GetPieceTypeFromPGNMove(string pgnMove) {
			if (pgnMove.Length == 2) {
				return PieceType.Pawn;
			}
			if (pgnMove == "O-O" || pgnMove == "O-O-O") {
				return PieceType.King;
			}
			var piece = pgnMove[0]; //should not capitalize this to check because all piece disambiguity notation is caps, therefore a file indicator will not be.
			switch (piece) {
				case 'B':
				case 'b':
					return PieceType.Bishop;

				case 'K':
				case 'k':
					return PieceType.King;

				case 'N':
				case 'n':
					return PieceType.Knight;

				case 'Q':
				case 'q':
					return PieceType.Queen;

				case 'R':
				case 'r':
					return PieceType.Rook;
			}
			return PieceType.Pawn;
		}

		public int GetPositionFromPGNMove(string pgnMove, Color playerColor) {
			if (pgnMove == "O-O" || pgnMove == "O-O-O") {
				if (playerColor == Color.White) {
					if (pgnMove == "O-O") {
						return 6;
					} else {
						return 2;
					}
				} else {
					if (pgnMove == "O-O") {
						return 62;
					} else {
						return 58;
					}
				}
			}
			pgnMove = pgnMove.Replace("x", "").Replace("+", "").Replace("#", "");
			var result = pgnMove.Contains("=")
							? NotationUtility.CoordinateToPosition(pgnMove.Substring(pgnMove.Length - 4, 2))
							: NotationUtility.CoordinateToPosition(pgnMove.Substring(pgnMove.Length - 2, 2));
			return result;
		}

		public bool IsRank(char potentialRank) {
			return char.IsNumber(potentialRank);
		}

		public (int piecePosition, int newPiecePosition) PGNMoveToSquarePair(GameState gameState, string pgnMove) {
			var newPiecePosition = GetPositionFromPGNMove(pgnMove, gameState.ActiveColor);
			var pieceType = GetPieceTypeFromPGNMove(pgnMove);
			var piece = new Piece(pieceType, gameState.ActiveColor);
			var piecePosition = GetCurrentPositionFromPGNMove(gameState, piece, newPiecePosition, pgnMove);
			return (piecePosition.Index, newPiecePosition);
		}

		public List<string> PGNSplit(string pgn) {
			if (string.IsNullOrEmpty(pgn)) { return null; }

			var regex = @"\d{1,3}\.";
			var splitResult = Regex.Split(pgn.Trim(), regex);
			return splitResult.ToList();
		}

		public List<string> PGNSplit(string pgn, bool mostConsise) {
			if (string.IsNullOrEmpty(pgn)) { return null; }

			var pgnData = PGNSplit(pgn);
			if (!mostConsise) { return pgnData; }

			if (pgnData != null && pgnData.Any()) {
				var iterationData = pgnData.ToList();
				var emptyStuffs = iterationData.Where(a => a == " " || string.IsNullOrEmpty(a)).ToList();
				if (emptyStuffs != null && emptyStuffs.Any()) {
					foreach (var item in emptyStuffs) {
						iterationData.Remove(item);
					}
				}
				var returnValue = new List<string>();

				foreach (var item in iterationData) {
					var movePair = item.Trim().Split(' ');
					returnValue.Add(movePair[0]);
					if (movePair.Length > 1) {
						returnValue.Add(movePair[1]);
					}
				}
				return returnValue;
			}
			return null;
		}

		public string SquarePairToPGNMove(GameState gameState, Color playerColor, string startSquare, string endSquare, char promoteToPiece) {
			var pgnMove = squarePairToPGNMove(gameState, playerColor, startSquare, endSquare);
			if (promoteToPiece == PGNService.NullPiece) {
				return pgnMove;
			} else {
				return $"{ pgnMove }{ PGNService.PawnPromotionIndicator }{ promoteToPiece }";
			}
		}

		private Square getOriginationPositionForCastling(GameState gameState, Color color) {
			var rank = color == Color.White ? 1 : 8;
			var file = 4;
			var fileChar = NotationUtility.IntToFile(file);
			var coord = string.Concat(fileChar, rank);
			var origination = NotationUtility.CoordinateToPosition(coord);
			return gameState.Squares.GetSquare(origination);
		}

		private string getPgnMove(char notationPiece, Piece piece, string coord, int startPos, int endPos, bool isCapture, GameState gameState) {
			string captureMarker = isCapture ? "x" : string.Empty;
			string pgnMove = getPGNMoveBeginState(notationPiece, coord, startPos, endPos, isCapture);
			string result = string.Empty;

			//figure out if additional information needs to be placed on the pgn move
			var otherSquaresOfThisTypeWithThisAttack = from s in gameState.Attacks
													   where s.Index == endPos
													   select s;
			var otherPiecesOfThisTypeWithThisAttack = from s in gameState.Attacks
													  join o in otherSquaresOfThisTypeWithThisAttack on s.Index equals o.Index
													  select s;

			if (otherPiecesOfThisTypeWithThisAttack.Count() <= 0) {
				return string.Concat(pgnMove.Substring(0, 1), captureMarker, pgnMove.Substring(1, pgnMove.Length - 1));
			}

			var secondPiece = otherPiecesOfThisTypeWithThisAttack.First();
			if (secondPiece.Piece.PieceType == PieceType.Pawn && !isCapture) {
				result = string.Concat(pgnMove.Substring(0, 1), captureMarker, pgnMove.Substring(1, pgnMove.Length - 1));
				return result;
			}

			//if other piece is on same the file of departure (if they differ); or
			var movingPieceFile = NotationUtility.PositionToFileChar(startPos);
			var otherPieceFile = NotationUtility.PositionToFileChar(secondPiece.Index);

			if (movingPieceFile != otherPieceFile) {
				if (notationPiece == 'P') {
					result = string.Concat(movingPieceFile, captureMarker, pgnMove);
				} else {
					result = string.Concat(pgnMove.Substring(0, 1), movingPieceFile, captureMarker, pgnMove.Substring(1, pgnMove.Length - 1));
				}
				return result;
			} else {
				//the rank of departure (if the files are the same but the ranks differ)
				var movingPieceRank = NotationUtility.PositionToRankInt(startPos);
				var otherPieceRank = NotationUtility.PositionToRankInt(secondPiece.Index);
				if (movingPieceRank != otherPieceRank) {
					result = string.Concat(pgnMove.Substring(0, 1), movingPieceRank, captureMarker, pgnMove.Substring(1, pgnMove.Length - 1));
					return result;
				} else {
					//both the rank and file
					//(if neither alone is sufficient to identify the piece—which occurs only in rare cases where one or more pawns have promoted,
					//resulting in a player having three or more identical pieces able to reach the same square).
					result = string.Concat(pgnMove.Substring(0, 1), movingPieceFile, movingPieceRank, captureMarker, pgnMove.Substring(1, 2));
					return result;
				}
			}
		}

		private string getPGNMoveBeginState(char notationPiece, string coord, int startPos, int endPos, bool isCapture) {
			string pgnMove = string.Empty;

			switch (notationPiece) {
				case 'P':
					if (isCapture) {
						var file = NotationUtility.PositionToFileChar(startPos);
						pgnMove = string.Concat(file, coord);
					} else {
						pgnMove = coord;
					}
					break;

				case 'K':
					var moveDiff = startPos - endPos;
					switch (moveDiff) {
						case -2:
							pgnMove = "O-O";
							break;

						case 2:
							pgnMove = "O-O-O";
							break;

						default:
							pgnMove = string.Concat(notationPiece, coord);
							break;
					}
					break;

				default:
					pgnMove = string.Concat(notationPiece, coord);
					break;
			}
			return pgnMove;
		}

		private bool isCapture(string move) {
			bool retval = false;
			if (move.Contains('x')) {
				retval = true;
			}
			return retval;
		}

		private bool isCastleKingside(string move) {
			bool retval = false;
			if (move.Contains("O-O")) {
				retval = true;
			}
			return retval;
		}

		private bool isCastleQueenside(string move) {
			bool retval = false;
			if (move.Contains("O-O-O")) {
				retval = true;
			}
			return retval;
		}

		private bool isCheck(string move) {
			bool retval = false;
			if (move.Contains('+')) {
				retval = true;
			}
			return retval;
		}

		private Square pgnLength2(Piece piece, IEnumerable<AttackedSquare> potentialPositions, bool capture) {
			if (piece.PieceType != PieceType.Pawn || capture) {
				throw new Exception("Failed to find square.");
			}
			var moves = piece.Color == Color.White ? new int[2] { -8, -16 } : new int[2] { 8, 16 };
			return potentialPositions.Where(a => moves.Contains(a.Index)).First();
		}

		private Square pgnLength3(IEnumerable<AttackedSquare> potentialPositions, string newPgnMove) {
			var ambiguityResolver = newPgnMove[0];
			var files = this.OrthogonalService.GetEntireFile(NotationUtility.FileToInt(ambiguityResolver)); //this will always be a file if this is a pawn
			var potentialSquare = potentialPositions.Where(a => files.Contains(a.Index)).ToList();
			if (potentialSquare.Count() > 1) {
				throw new Exception("There should not be more than one item found here.");
			}
			return potentialSquare.First();
		}

		private Square pgnLength4(GameState gameState, IEnumerable<AttackedSquare> potentialPositions, string newPgnMove) {
			var ambiguityResolver = newPgnMove[1];
			var isRank = IsRank(ambiguityResolver); //this could be either a rank or a file
			List<int> ambiguityResolutionSet;
			if (isRank) {
				int rank = 0;
				Int32.TryParse(ambiguityResolver.ToString(), out rank);
				ambiguityResolutionSet = this.OrthogonalService.GetEntireRank(rank - 1);//needs to be using zero-based rank offset
			} else {
				var iFile = NotationUtility.FileToInt(ambiguityResolver);
				ambiguityResolutionSet = this.OrthogonalService.GetEntireFile(iFile);
			}
			var intersection = potentialPositions.Select(a => a.AttackerSquare.Index).Intersect(ambiguityResolutionSet);
			if (intersection.Count() > 1) {
				throw new Exception("There should not be more than one item found here.");
			}
			return gameState.Squares.GetSquare(intersection.First());
		}

		private Square pgnLength5(GameState gameState, string newPgnMove) {
			var _file = NotationUtility.FileToInt(newPgnMove[1]);
			var _rank = 0;
			Int32.TryParse(newPgnMove[2].ToString(), out _rank);
			var pos = NotationUtility.CoordinatePairToPosition(_file, _rank);
			return gameState.Squares.GetSquare(pos);
		}

		private string squarePairToPGNMove(GameState gameState, Color playerColor, string startSquare, string endSquare) {
			var startPos = NotationUtility.CoordinateToPosition(startSquare);
			var endPos = NotationUtility.CoordinateToPosition(endSquare);
			var destinationSquare = gameState.Squares.GetSquare(endPos);
			var isCapture = destinationSquare.Occupied && destinationSquare.Piece.Color != playerColor;

			var attacks = gameState.Attacks.Where(a => a.AttackerSquare.Index == startPos);
			if (attacks == null || !attacks.Any() || !attacks.Any(a => a.Index == endPos)) {
				throw new Exception("No attacks can be made on this ending square.");
			}

			var square = gameState.Squares.GetSquare(startPos);
			if (!square.Occupied) {
				throw new Exception("Bad coordinates were given.");
			}

			var piece = square.Piece;
			if (piece.Color != playerColor) {
				throw new Exception("Color doesn't match given positions.");
			}
			var notationPiece = char.ToUpper(piece.Identity);
			var coord = NotationUtility.PositionToCoordinate(endPos);
			var pgnMove = getPgnMove(notationPiece, piece, coord, startPos, endPos, isCapture, gameState);
			return pgnMove;
		}
	}
}