define(["require", "exports"], function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.pieceType = void 0;
    var pieceType;
    (function (pieceType) {
        pieceType[pieceType["Pawn"] = 0] = "Pawn";
        pieceType[pieceType["Knight"] = 1] = "Knight";
        pieceType[pieceType["Bishop"] = 2] = "Bishop";
        pieceType[pieceType["Rook"] = 3] = "Rook";
        pieceType[pieceType["Queen"] = 4] = "Queen";
        pieceType[pieceType["King"] = 5] = "King";
    })(pieceType = exports.pieceType || (exports.pieceType = {}));
});
