define(["require", "exports", "../constants/ui/events"], function (require, exports, events_1) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.logging = void 0;
    class logging {
        static info(x) {
            console.log(x);
        }
        static error(x) {
            console.error(x);
        }
        static logDragStart(source, piece, position, orientation) {
            console.log({
                Event: events_1.events.onDragStart,
                Source: source,
                Piece: piece,
                //Position: Chessboard.objToFen(position),
                Orientation: orientation
            });
        }
        static logDrop(source, target, piece, newPos, oldPos, orientation, squareAttacks) {
            console.log({
                Event: events_1.events.onDrop,
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
});
