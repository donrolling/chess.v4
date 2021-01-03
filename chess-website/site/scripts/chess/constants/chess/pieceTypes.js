define(["require", "exports"], function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.pieceTypes = void 0;
    var pieceTypes;
    (function (pieceTypes) {
        pieceTypes[pieceTypes["pawn"] = 0] = "pawn";
        pieceTypes[pieceTypes["knight"] = 1] = "knight";
        pieceTypes[pieceTypes["bishop"] = 2] = "bishop";
        pieceTypes[pieceTypes["rook"] = 3] = "rook";
        pieceTypes[pieceTypes["queen"] = 4] = "queen";
        pieceTypes[pieceTypes["king"] = 5] = "king";
    })(pieceTypes = exports.pieceTypes || (exports.pieceTypes = {}));
});
