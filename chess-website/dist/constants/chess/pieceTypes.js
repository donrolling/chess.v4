"use strict";
var constants;
(function (constants) {
    var chess;
    (function (chess) {
        let pieceTypes;
        (function (pieceTypes) {
            pieceTypes[pieceTypes["pawn"] = 0] = "pawn";
            pieceTypes[pieceTypes["knight"] = 1] = "knight";
            pieceTypes[pieceTypes["bishop"] = 2] = "bishop";
            pieceTypes[pieceTypes["rook"] = 3] = "rook";
            pieceTypes[pieceTypes["queen"] = 4] = "queen";
            pieceTypes[pieceTypes["king"] = 5] = "king";
        })(pieceTypes = chess.pieceTypes || (chess.pieceTypes = {}));
    })(chess = constants.chess || (constants.chess = {}));
})(constants || (constants = {}));
