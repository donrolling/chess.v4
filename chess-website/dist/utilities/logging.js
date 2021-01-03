"use strict";
var utilities;
(function (utilities) {
    class logging {
        static info(x) {
            console.log(x);
        }
        static error(x) {
            console.error(x);
        }
        static logDragStart(source, piece, position, orientation) {
            console.log({
                Event: constants.ui.events.onDragStart,
                Source: source,
                Piece: piece,
                //Position: Chessboard.objToFen(position),
                Orientation: orientation
            });
        }
        static logDrop(source, target, piece, newPos, oldPos, orientation, squareAttacks) {
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
    utilities.logging = logging;
})(utilities || (utilities = {}));
