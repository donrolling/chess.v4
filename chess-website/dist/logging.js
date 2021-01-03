"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.logging = void 0;
class logging {
    info(x) {
        console.log(x);
    }
    error(x) {
        console.error(x);
    }
    logDragStart(source, piece, position, orientation) {
        console.log({
            Event: constants.ui.events.onDragStart,
            Source: source,
            Piece: piece,
            //Position: Chessboard.objToFen(position),
            Orientation: orientation
        });
    }
    logDrop(source, target, piece, newPos, oldPos, orientation, squareAttacks) {
        console.log({
            Event: constants.ui.events.onDrop,
            Source: source,
            Target: target,
            Piece: piece,
            NewPosition: newPos,
            //NewFen: Chessboard.objToFen(newPos),
            OldPosition: oldPos,
            //OldFen: Chessboard.objToFen(oldPos),
            Orientation: orientation,
            SquareAttacks: squareAttacks,
        });
    }
}
exports.logging = logging;
