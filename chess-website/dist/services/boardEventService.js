"use strict";
var services;
(function (services) {
    class boardEventService {
        constructor(gameObjects, gameService) {
            this.gameObjects = gameObjects;
            this.gameService = gameService;
        }
        onDragStart(source, piece, position, orientation) {
            // logging.logDragStart(source, piece, position, orientation);
            // protect against wrong side moves
            if ((piece[0] === 'w' && this.gameObjects.gameState.activeColor === 0)
                || (piece[0] === 'b' && this.gameObjects.gameState.activeColor === 1)) {
                return false;
            }
            document
                .querySelectorAll(constants.ui.selectors.attacking)
                .forEach(a => utilities.dom.removeClassName(a, constants.classes.attacking));
            document
                .querySelectorAll(constants.selectors.protecting)
                .forEach(a => utilities.dom.removeClassName(a, constants.classes.protecting));
            let squareAttacks = utilities.dom.getSquareAttacks(source);
            utilities.highlightSquares(squareAttacks);
        }
        onDrop(source, target, piece, newPos, oldPos, orientation) {
            if (gameObjects.freeze) {
                if (gameObjects.freezeNotify < 2) {
                    alert('You are looking at the historic state of the board. New moves are frozen until you go back to the current state using the history panel.');
                    gameObjects.freezeNotify++;
                }
                return constants.methodResponses.snapback;
            }
            let squareAttacks = utilities.getSquareAttacks(source);
            if (!squareAttacks.some(x => x.name === target)) {
                return constants.methodResponses.snapback;
            }
            // logging.logDrop(source, target, piece, newPos, oldPos, orientation, null);
            if ((piece[0] === 'w' && gameObjects.gameState.activeColor === 1 && piece[1] === 'P' && target[1] == 8)
                || (piece[0] === 'b' && gameObjects.gameState.activeColor === 0 && piece[1] === 'P' && target[1] == 1)) {
                utilities.displayPawnPromotion(source, target);
            }
            else {
                services.gameService.move(source, target, null);
            }
        }
        onPGNClick(e) {
            let target = e.target;
            if (!target) {
                return;
            }
            let index = target.getAttribute(constants.ui.attributes.dataIndex);
            if (!index) {
                return;
            }
            let dataIndex = parseInt(index);
            services.gameService.goToMove(dataIndex);
        }
        handleSquareClick(e) {
            //utilities.dom.removeOldClasses();
            //let currentSquare = utilities.dom.getCurrentSquare(e);
            //let squareAttacks = utilities.getSquareAttacks(currentSquare);
            //utilities.highlightSquares(squareAttacks);
        }
    }
    services.boardEventService = boardEventService;
})(services || (services = {}));
