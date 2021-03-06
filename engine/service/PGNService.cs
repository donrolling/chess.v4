using Chess.v4.Engine.Extensions;
using Chess.v4.Engine.Interfaces;
using Chess.v4.Engine.Utility;
using Chess.v4.Models;
using Chess.v4.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Chess.v4.Engine.Service
{
    // Pawn promotions are notated by appending an "=" to the destination square, followed by the piece the pawn is promoted to.
    // "e8=Q". If the move is a checking move, the plus sign "+" is also appended;
    // if the move is a checkmating move, the number sign "#" is appended instead. For example: "e8=Q#".
    // kingside castling is indicated by the sequence "O-O"; queenside castling is indicated by the sequence "O-O-O"
    public class PGNService : IPGNService
    {
        private readonly IOrthogonalService _orthogonalService;

        private Regex pawnCapturePromotionPattern { get; } = new Regex(@"[a-h]x[a-h]\d[rbnkqRBNKQ][\+\#]?");
        private Regex pawnPromotionPattern { get; } = new Regex(@"[a-h]\d[rbnkqRBNKQ][\+\#]?");
        private Regex castlePattern { get; } = new Regex(@"O\-O(\-O)?[\+\#]?");
        public const char NullPiece = '-';
        public const char PawnPromotionIndicator = '=';

        public PGNService(IOrthogonalService orthogonalService)
        {
            _orthogonalService = orthogonalService;
        }

        public Square GetCurrentPositionFromPGNMove(GameState gameState, Piece piece, int newPiecePosition, string pgnMove, bool isCastle)
        {
            if (isCastle)
            {
                return getOriginationPositionForCastling(gameState, piece.Color);
            }
            // adding !a.MayOnlyMoveHereIfOccupiedByEnemy fixed the test I was working on, but there may be a deeper issue here.
            var potentialSquares = gameState.Attacks.Where(a =>
                                                        a.Index == newPiecePosition
                                                        && (
                                                            a.Occupied || (!a.Occupied && !a.MayOnlyMoveHereIfOccupiedByEnemy)
                                                        )
                                                        && a.AttackingSquare.Piece.PieceType == piece.PieceType
                                                        && a.AttackingSquare.Piece.Color == piece.Color
                                                    );
            if (!potentialSquares.Any())
            {
                // this could be a pawn en passant
                if (
                    piece.PieceType == PieceType.Pawn
                    && gameState.EnPassantTargetSquare != "-"
                    && NotationEngine.CoordinateToPosition(gameState.EnPassantTargetSquare) == newPiecePosition
                )
                {
                    var enPassant = gameState.Attacks.Where(a =>
                                                a.Index == newPiecePosition
                                                && a.AttackingSquare.Piece.PieceType == piece.PieceType
                                                && a.AttackingSquare.Piece.Color == piece.Color
                                            ).First().AttackingSquare;
                    return enPassant;
                }
                var msg = $"No squares found. PGN Move: { pgnMove }";
                throw new Exception(msg);
            }
            if (potentialSquares.Count() == 1)
            {
                return potentialSquares.First().AttackingSquare;
            }
            // differentiate
            var potentialPositions = from s in gameState.Squares
                                     join p in potentialSquares on s.Index equals p.AttackingSquare.Index
                                     where s.Piece.Identity == piece.Identity
                                     select p;
            if (!potentialPositions.Any())
            {
                var msg = $"Attempting to differentiate. No squares found. PGN Move: { pgnMove }";
                throw new Exception(msg);
            }
            // x means capture and shouldn't be used in the equation below
            var capture = isCapture(pgnMove);
            var check = isCheck(pgnMove);

            // todo: refactor to eliminate redundancy
            // look at the beginning of the pgnMove string to determine which of the pieces are the one that should be moved.
            // this should only happen if there are two pieces of the same type that can attack here.
            var newPgnMove = pgnMove.Replace("x", "").Replace("+", "");
            var moveLength = newPgnMove.Length;
            switch (moveLength)
            {
                case 2:
                    return pgnLength2(piece, potentialPositions, capture);

                case 3:
                    // this should be a pawn attack that can be made by two pawns
                    // this can also be a pawn promotion
                    return pgnLength3(potentialPositions, newPgnMove);

                case 4: // this would be any other piece
                    return pgnLength4(gameState, potentialPositions, newPgnMove);

                case 5: // we have rank and file, so just find the piece. this should be very rare
                    return pgnLength5(gameState, newPgnMove);

                default:
                    throw new Exception("Failed to find square.");
            }
        }

        public bool IsRank(char potentialRank)
        {
            return char.IsNumber(potentialRank);
        }

        public (int piecePosition, int newPiecePosition, char promotedPiece) PGNMoveToSquarePair(GameState gameState, string pgnMove)
        {
            var positionFromPGNMove = getPositionFromPGNMove(pgnMove, gameState.ActiveColor, gameState.EnPassantTargetSquare);
            var newPiecePosition = positionFromPGNMove.position;
            var promotedPiece = positionFromPGNMove.promotedPiece;
            var pieceType = PGNEngine.GetPieceTypeFromPGNMove(pgnMove);
            var pieceIndicator = pgnMove[0];
            if (pieceIndicator == 'b')
            {
                if (gameState.ActiveColor == Color.White)
                {
                    // this is a pawn move, not a bishop because a white bishop would be B, not b
                    pieceType = PieceType.Pawn;
                }
                else
                {
                    // the code above will think that this move is done by a bishop
                    // so here we see if any pawn can do the job
                    // the attacking pawn would have to be on the b file for confusion to occur here
                    // ****************************************************
                    // I'm concerned that there might be a situation where the pgn move differentiates between
                    // the pawn and bishop move, yet we haven't looked close enough.
                    // should probably attempt to induce this scenario
                    var bFile = this._orthogonalService.GetEntireFile(1);
                    var potentialPawnSquares = gameState.Attacks.Where(a =>
                                            a.Index == newPiecePosition
                                            && bFile.Contains(a.AttackingSquare.Index)
                                            && a.AttackingSquare.Piece.Color == gameState.ActiveColor
                                            && (
                                                a.Occupied || (!a.Occupied && !a.MayOnlyMoveHereIfOccupiedByEnemy)
                                            )
                                            && a.AttackingSquare.Piece.PieceType == PieceType.Pawn
                                        );
                    if (potentialPawnSquares.Any())
                    {
                        // if there are pawn pieces that can fulfill here, then make the pieceType = Pawn
                        pieceType = PieceType.Pawn;
                    }
                }
            }
            var piece = new Piece(pieceType, gameState.ActiveColor);
            var piecePosition = GetCurrentPositionFromPGNMove(gameState, piece, newPiecePosition, pgnMove, positionFromPGNMove.isCastle);
            return (piecePosition.Index, newPiecePosition, promotedPiece);
        }

        public string SquarePairToPGNMove(GameState gameState, Color playerColor, string startSquare, string endSquare)
        {
            return squarePairToPGNMove(gameState, playerColor, startSquare, endSquare);
        }

        public string SquarePairToPGNMove(GameState gameState, Color playerColor, int startSquare, int endSquare)
        {
            return squarePairToPGNMove(gameState, playerColor, startSquare, endSquare);
        }

        public string SquarePairToPGNMove(GameState gameState, Color playerColor, int startSquare, int endSquare, PieceType promoteToPiece)
        {
            var pgnMove = squarePairToPGNMove(gameState, playerColor, startSquare, endSquare);
            return finishPGNMove(pgnMove, promoteToPiece, playerColor);

        }

        public string SquarePairToPGNMove(GameState gameState, Color playerColor, string startSquare, string endSquare, PieceType promoteToPiece)
        {
            var pgnMove = squarePairToPGNMove(gameState, playerColor, startSquare, endSquare);
            return finishPGNMove(pgnMove, promoteToPiece, playerColor);
        }

        private static string finishPGNMove(string pgnMove, PieceType promoteToPiece, Color playerColor) {
            var promotedPiece = PGNEngine.GetPieceCharFromPieceTypeColor(promoteToPiece, playerColor);
            if (promotedPiece == PGNService.NullPiece)
            {
                return pgnMove;
            }
            else
            {
                return $"{ pgnMove }{ PGNService.PawnPromotionIndicator }{ promotedPiece }";
            }
        }

        private static (int position, char promotedPiece) getCastlePositionFromPGNMove(string pgnMove, Color playerColor, char promotedPiece)
        {
            var move = pgnMove.Trim('+').Trim('#');
            if (playerColor == Color.White)
            {
                if (move == "O-O")
                {
                    return (6, promotedPiece);
                }
                else
                {
                    return (2, promotedPiece);
                }
            }
            else
            {
                if (move == "O-O")
                {
                    return (62, promotedPiece);
                }
                else
                {
                    return (58, promotedPiece);
                }
            }
        }

        private Square getOriginationPositionForCastling(GameState gameState, Color color)
        {
            var rank = color == Color.White ? 1 : 8;
            var file = 4;
            var fileChar = NotationEngine.IntToFile(file);
            var coord = string.Concat(fileChar, rank);
            var origination = NotationEngine.CoordinateToPosition(coord);
            return gameState.Squares.GetSquare(origination);
        }

        private string getPgnMove(char notationPiece, Piece piece, string coord, int startPos, int endPos, bool isCapture, GameState gameState)
        {
            var captureMarker = isCapture ? "x" : string.Empty;
            var pgnMove = getPGNMoveBeginState(notationPiece, coord, startPos, endPos, isCapture);
            var result = string.Empty;

            // figure out if additional information needs to be placed on the pgn move
            // this should only need to happen if the pieces are of the same type
            var movingSquare = gameState.Squares.Where(a => a.Index == startPos).First();
            var otherSquaresOfThisTypeWithThisAttack = from s in gameState.Attacks
                                                       where s.Index == endPos
                                                            && s.AttackingSquare.Index != movingSquare.Index
                                                            && s.AttackingSquare.Piece.PieceType == movingSquare.Piece.PieceType
                                                            && s.AttackingSquare.Piece.Color == gameState.ActiveColor
                                                       select s;
            if (otherSquaresOfThisTypeWithThisAttack.Count() <= 0)
            {
                return string.Concat(pgnMove.Substring(0, 1), captureMarker, pgnMove.Substring(1, pgnMove.Length - 1));
            }

            var secondPiece = otherSquaresOfThisTypeWithThisAttack.First();
            if (secondPiece.AttackingSquare.Piece.PieceType == PieceType.Pawn && !isCapture)
            {
                result = string.Concat(pgnMove.Substring(0, 1), captureMarker, pgnMove.Substring(1, pgnMove.Length - 1));
                return result;
            }

            // if other piece is on same the file of departure (if they differ); or
            var movingPieceFile = NotationEngine.PositionToFileChar(startPos);
            var otherPieceFile = NotationEngine.PositionToFileChar(secondPiece.Index);

            if (movingPieceFile != otherPieceFile)
            {
                if (notationPiece == 'P')
                {
                    result = string.Concat(movingPieceFile, captureMarker, pgnMove);
                }
                else
                {
                    result = string.Concat(pgnMove.Substring(0, 1), movingPieceFile, captureMarker, pgnMove.Substring(1, pgnMove.Length - 1));
                }
                return result;
            }
            else
            {
                // the rank of departure (if the files are the same but the ranks differ)
                // these are index = 0 rank/file, so add one for PGN output
                var movingPieceRank = NotationEngine.PositionToRankInt(startPos) + 1;
                var otherPieceRank = NotationEngine.PositionToRankInt(secondPiece.Index) + 1;
                if (movingPieceRank != otherPieceRank)
                {
                    result = string.Concat(pgnMove.Substring(0, 1), movingPieceRank, captureMarker, pgnMove.Substring(1, pgnMove.Length - 1));
                    return result;
                }
                else
                {
                    // both the rank and file
                    // (if neither alone is sufficient to identify the piece—which occurs only in rare cases where one or more pawns have promoted,
                    // resulting in a player having three or more identical pieces able to reach the same square).
                    result = string.Concat(pgnMove.Substring(0, 1), movingPieceFile, movingPieceRank, captureMarker, pgnMove.Substring(1, 2));
                    return result;
                }
            }
        }

        private string getPGNMoveBeginState(char notationPiece, string coord, int startPos, int endPos, bool isCapture)
        {
            string pgnMove = string.Empty;

            switch (notationPiece)
            {
                case 'P':
                    if (isCapture)
                    {
                        var file = NotationEngine.PositionToFileChar(startPos);
                        pgnMove = string.Concat(file, coord);
                    }
                    else
                    {
                        pgnMove = coord;
                    }
                    break;

                case 'K':
                    var moveDiff = startPos - endPos;
                    switch (moveDiff)
                    {
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

        private (int position, char promotedPiece, bool isCastle) getPositionFromPGNMove(string pgnMove, Color playerColor, string enPassantTargetSquare)
        {
            var promotedPiece = '-';
            var isCastle = castlePattern.IsMatch(pgnMove);
            if (isCastle)
            {
                var castleResult = getCastlePositionFromPGNMove(pgnMove, playerColor, promotedPiece);
                return (castleResult.position, castleResult.promotedPiece, true);
            }
            var isEnPassant = false;
            if (enPassantTargetSquare != "-")
            {
                var isCapture = pgnMove[1] == 'x';
                if (isCapture)
                {
                    var len = pgnMove.Length;
                    if (len >= 6)
                    {
                        var rightSide = pgnMove.Substring(4, len - 4);
                        isEnPassant = rightSide.Contains("ep") || rightSide.Contains("e.p.");
                        if (isEnPassant)
                        {
                            return (NotationEngine.CoordinateToPosition(enPassantTargetSquare), promotedPiece, false);
                        }
                    }
                }
            }

            // could be a pawn promotion
            var pawnCapturePromotion = pawnCapturePromotionPattern.IsMatch(pgnMove);
            var pawnPromotion = pawnPromotionPattern.IsMatch(pgnMove);
            if (pawnCapturePromotion || pawnPromotion)
            {
                // yeah, it is a pawn promotion
                promotedPiece = pgnMove.Last();
                if (promotedPiece == '+' || promotedPiece == '#')
                {
                    promotedPiece = pgnMove.Substring(pgnMove.Length - 2, 1)[0];
                }
                var dest = pawnCapturePromotion ? pgnMove.Substring(2, 2) : pgnMove.Substring(0, 2);
                return (NotationEngine.CoordinateToPosition(dest), promotedPiece, false);
            }
            // probably just a regular move
            pgnMove = pgnMove.Replace("x", "").Replace("+", "").Replace("#", "");
            // not sure why I needed this
            var x = 2;
            if (pgnMove.Contains("="))
            {
                x = 4;
                throw new Exception("I didn't think this could happen, so I tested it with an exception.");
            }
            return (NotationEngine.CoordinateToPosition(pgnMove.Substring(pgnMove.Length - x, 2)), promotedPiece, false);
        }

        private bool isCapture(string move)
        {
            bool retval = false;
            if (move.Contains('x'))
            {
                retval = true;
            }
            return retval;
        }

        private bool isCastleKingside(string move)
        {
            bool retval = false;
            if (move.Contains("O-O"))
            {
                retval = true;
            }
            return retval;
        }

        private bool isCastleQueenside(string move)
        {
            bool retval = false;
            if (move.Contains("O-O-O"))
            {
                retval = true;
            }
            return retval;
        }

        private bool isCheck(string move)
        {
            bool retval = false;
            if (move.Contains('+'))
            {
                retval = true;
            }
            return retval;
        }

        private Square pgnLength2(Piece piece, IEnumerable<AttackedSquare> potentialPositions, bool capture)
        {
            if (piece.PieceType != PieceType.Pawn || capture)
            {
                throw new Exception("Failed to find square.");
            }
            var moves = piece.Color == Color.White ? new int[2] { -8, -16 } : new int[2] { 8, 16 };
            return potentialPositions.Where(a => moves.Contains(a.Index)).First();
        }

        private Square pgnLength3(IEnumerable<AttackedSquare> potentialPositions, string newPgnMove)
        {
            var ambiguityResolver = newPgnMove[0];
            var files = this._orthogonalService.GetEntireFile(NotationEngine.FileToInt(ambiguityResolver)); // this will always be a file if this is a pawn
            var potentialSquares = potentialPositions.Where(a => files.Contains(a.AttackingSquare.Index)).ToList();
            if (potentialSquares.Count() > 1)
            {
                throw new Exception("There should not be more than one item found here.");
            }
            return potentialSquares.First().AttackingSquare;
        }

        private Square pgnLength4(GameState gameState, IEnumerable<AttackedSquare> potentialPositions, string newPgnMove)
        {
            var ambiguityResolver = newPgnMove[1];
            var isRank = IsRank(ambiguityResolver); // this could be either a rank or a file
            List<int> ambiguityResolutionSet;
            if (isRank)
            {
                int rank = 0;
                Int32.TryParse(ambiguityResolver.ToString(), out rank);
                ambiguityResolutionSet = this._orthogonalService.GetEntireRank(rank - 1);// needs to be using zero-based rank offset
            }
            else
            {
                var iFile = NotationEngine.FileToInt(ambiguityResolver);
                ambiguityResolutionSet = this._orthogonalService.GetEntireFile(iFile);
            }
            var intersection = potentialPositions.Select(a => a.AttackingSquare.Index).Intersect(ambiguityResolutionSet);
            if (intersection.Count() > 1)
            {
                throw new Exception("There should not be more than one item found here.");
            }
            return gameState.Squares.GetSquare(intersection.First());
        }

        private Square pgnLength5(GameState gameState, string newPgnMove)
        {
            var _file = NotationEngine.FileToInt(newPgnMove[1]);
            var _rank = 0;
            Int32.TryParse(newPgnMove[2].ToString(), out _rank);
            var pos = NotationEngine.CoordinatePairToPosition(_file, _rank);
            return gameState.Squares.GetSquare(pos);
        }

        private string squarePairToPGNMove(GameState gameState, Color playerColor, int startPos, int endPos)
        {
            var destinationSquare = gameState.Squares.GetSquare(endPos);
            var isCapture = destinationSquare.Occupied && destinationSquare.Piece.Color != playerColor;

            var attacks = gameState.Attacks.Where(a => a.AttackingSquare.Index == startPos);
            if (attacks == null || !attacks.Any() || !attacks.Any(a => a.Index == endPos))
            {
                throw new Exception("No attacks can be made on this ending square.");
            }

            var square = gameState.Squares.GetSquare(startPos);
            if (!square.Occupied)
            {
                throw new Exception("Bad coordinates were given.");
            }

            var piece = square.Piece;
            if (piece.Color != playerColor)
            {
                throw new Exception("Color doesn't match given positions.");
            }
            var notationPiece = char.ToUpper(piece.Identity);
            var coord = NotationEngine.PositionToCoordinate(endPos);
            var pgnMove = getPgnMove(notationPiece, piece, coord, startPos, endPos, isCapture, gameState);
            return pgnMove;
        }

        private string squarePairToPGNMove(GameState gameState, Color playerColor, string startSquare, string endSquare)
        {
            var startPos = NotationEngine.CoordinateToPosition(startSquare);
            var endPos = NotationEngine.CoordinateToPosition(endSquare);
            return squarePairToPGNMove(gameState, playerColor, startPos, endPos);
        }
    }
}