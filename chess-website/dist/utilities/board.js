"use strict";
var utilities;
(function (utilities) {
    class board {
        static highlightSquares(attacks) {
            for (let i = 0; i < attacks.length; i++) {
                let attack = attacks[i];
                let squareClass = attack.isProtecting ? constants.ui.classes.protecting : constants.ui.classes.attacking;
                let squareSelector = utilities.dom.getSquareSelector(attack.name);
                var square = document.querySelector(squareSelector);
                if (square) {
                    square.className += ` ${squareClass}`;
                }
            }
        }
    }
    utilities.board = board;
})(utilities || (utilities = {}));
