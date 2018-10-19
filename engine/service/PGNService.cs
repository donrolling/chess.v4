using chess.v4.engine.enumeration;
using chess.v4.engine.extensions;
using chess.v4.engine.interfaces;
using chess.v4.engine.model;
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
		public ICoordinateService CoordinateService { get; }
		public IDiagonalService DiagonalService { get; }
		public IOrthogonalService OrthogonalService { get; }
		public const char NullPiece = '-';
		public const char PawnPromotionIndicator = '=';

		public PGNService(ICoordinateService coordinateService, IDiagonalService diagonalService, IOrthogonalService orthogonalService) {
			CoordinateService = coordinateService;
			DiagonalService = diagonalService;
			OrthogonalService = orthogonalService;
		}

		public int GetCurrentPositionFromPGNMove(GameState gameState, PieceType piece, Color playerColor, int newPiecePosition, string pgnMove) {
			char pieceChar = GetPieceCharFromPieceTypeColor(piece, playerColor);
			var potentialSquares = gameState.Attacks.Where(a => a.Index == newPiecePosition);
			var potentialPositions = from s in gameState.Squares
									 join p in potentialSquares on s.Index equals p.Index
									 where s.Occupied && s.Piece.Identity == pieceChar
									 select p;
			if (!potentialPositions.Any()) {
				return -1; //meaning no postion available
			}

			//x means capture and shouldn't be used in the equation below
			var capture = isCapture(pgnMove);
			var check = isCheck(pgnMove);
			var castleKingside = isCastleKingside(pgnMove);
			var castleQueenside = isCastleQueenside(pgnMove);
			var isCastle = castleKingside || castleQueenside;
			var newPgnMove = pgnMove.Replace("x", "").Replace("+", "");

			if (isCastle) {
				return getOriginationPositionForCastling(playerColor);
			}

			//what??
			//if (potentialPositions.Count() < 1) {
			//	return potentialPositions.First().Key;
			//}

			//todo: refactor to eliminate redundancy
			//look at the beginning of the pgnMove string to determine which of the pieces are the one that should be moved.
			//this should only happen if there are two pieces of the same type that can attack here.
			var moveLength = newPgnMove.Length;
			var ambiguityResolver = '-';
			switch (moveLength) {
				case 2:
					if (piece == PieceType.Pawn) {
						if (!capture) { //todo: make sure this makes sense, i was distracted - seems ok two days later
							if (playerColor == Color.White) {
								return potentialPositions.Where(a => a.Index == newPiecePosition - 8 || a.Index == newPiecePosition - 16).First().Index;
							} else {
								return potentialPositions.Where(a => a.Index == newPiecePosition + 8 || a.Index == newPiecePosition + 16).First().Index;
							}
						}
					}
					return -1; //indicates failure
				case 3: //this should be a pawn attack that can be made by two pawns
					ambiguityResolver = newPgnMove[0];
					var files = this.OrthogonalService.GetEntireFile(CoordinateService.FileToInt(ambiguityResolver)); //this will always be a file if this is a pawn
					var pieces = potentialPositions.Where(a => files.Contains(a.Index)).ToList();
					if (pieces.Count() > 1) {
						throw new Exception("There should not be more than one item found here.");
					}
					return pieces.First().Index;

				case 4: //this would be any other piece
					ambiguityResolver = newPgnMove[1];
					var isRank = IsRank(ambiguityResolver); //this could be either a rank or a file
					List<int> ambiguityResolutionSet;
					if (isRank) {
						int rank = 0;
						Int32.TryParse(ambiguityResolver.ToString(), out rank);
						ambiguityResolutionSet = this.OrthogonalService.GetEntireRank(rank - 1);//needs to be using zero-based rank offset
					} else {
						var iFile = CoordinateService.FileToInt(ambiguityResolver);
						ambiguityResolutionSet = this.OrthogonalService.GetEntireFile(iFile);
					}
					var intersection = potentialPositions.Select(a => a.Index).Intersect(ambiguityResolutionSet);
					if (intersection.Count() > 1) {
						throw new Exception("There should not be more than one item found here.");
					}
					return intersection.First();

				case 5: //we have rank and file, so just find the piece. this should be very rare
					var _file = CoordinateService.FileToInt(newPgnMove[1]);
					int _rank = 0;
					Int32.TryParse(newPgnMove[2].ToString(), out _rank);
					int pos = CoordinateService.CoordinatePairToPosition(_file, _rank);
					return pos;

				default:
					return -1; //indicates failure
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
							? CoordinateService.CoordinateToPosition(pgnMove.Substring(pgnMove.Length - 4, 2))
							: CoordinateService.CoordinateToPosition(pgnMove.Substring(pgnMove.Length - 2, 2));
			return result;
		}

		public bool IsRank(char potentialRank) {
			return char.IsNumber(potentialRank);
		}

		public (int piecePosition, int newPiecePosition) PGNMoveToSquarePair(GameState gameState, Color playerColor, string pgnMove) {
			var newPiecePosition = GetPositionFromPGNMove(pgnMove, playerColor);
			var piece = GetPieceTypeFromPGNMove(pgnMove);
			var piecePosition = GetCurrentPositionFromPGNMove(gameState, piece, playerColor, newPiecePosition, pgnMove);
			return (piecePosition, newPiecePosition);
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
			var pgnMove = "";
			try {
				pgnMove = squarePairToPGNMove(gameState, playerColor, startSquare, endSquare);
			} catch {
				pgnMove = "Invalid";
			}
			if (promoteToPiece == PGNService.NullPiece) {
				return pgnMove;
			} else {
				var result = string.Concat(pgnMove, PGNService.PawnPromotionIndicator, promoteToPiece);
				return result;
			}
		}

		private int getOriginationPositionForCastling(Color color) {
			int rank = color == Color.White ? 1 : 8;
			int file = 4;
			char fileChar = CoordinateService.IntToFile(file);
			var coord = string.Concat(fileChar, rank);
			int origination = CoordinateService.CoordinateToPosition(coord);
			return origination;
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
			var movingPieceFile = CoordinateService.PositionToFileChar(startPos);
			var otherPieceFile = CoordinateService.PositionToFileChar(secondPiece.Index);

			if (movingPieceFile != otherPieceFile) {
				if (notationPiece == 'P') {
					result = string.Concat(movingPieceFile, captureMarker, pgnMove);
				} else {
					result = string.Concat(pgnMove.Substring(0, 1), movingPieceFile, captureMarker, pgnMove.Substring(1, pgnMove.Length - 1));
				}
				return result;
			} else {
				//the rank of departure (if the files are the same but the ranks differ)
				var movingPieceRank = CoordinateService.PositionToRankInt(startPos);
				var otherPieceRank = CoordinateService.PositionToRankInt(secondPiece.Index);
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
						var file = CoordinateService.PositionToFileChar(startPos);
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

		private string squarePairToPGNMove(GameState gameState, Color playerColor, string startSquare, string endSquare) {
			var startPos = CoordinateService.CoordinateToPosition(startSquare);
			var endPos = CoordinateService.CoordinateToPosition(endSquare);
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
			var coord = CoordinateService.PositionToCoordinate(endPos);
			var pgnMove = getPgnMove(notationPiece, piece, coord, startPos, endPos, isCapture, gameState);
			return pgnMove;
		}
	}
}