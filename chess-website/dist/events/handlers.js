"use strict";
var events;
(function (events) {
    class handlers {
        static handleSquareClick(e) {
            //utilities.dom.removeOldClasses();
            let currentSquare = utilities.dom.getCurrentSquare(e);
            let squareAttacks = utilities.getSquareAttacks(currentSquare);
            utilities.highlightSquares(squareAttacks);
        }
    }
    events.handlers = handlers;
})(events || (events = {}));
