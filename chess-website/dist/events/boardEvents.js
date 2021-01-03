"use strict";
var events;
(function (events) {
    class boardEvents {
        constructor(gameObjects, gameService) {
            this.gameObjects = gameObjects;
            this.gameService = gameService;
        }
        init() {
            var _a, _b, _c;
            (_a = document
                .querySelector(constants.ui.selectors.fenSubmit)) === null || _a === void 0 ? void 0 : _a.addEventListener(constants.ui.events.click, () => this.gameService.getFenAndUpdate());
            (_b = document
                .querySelector(constants.ui.selectors.backBtn)) === null || _b === void 0 ? void 0 : _b.addEventListener(constants.ui.events.click, () => this.onBackClick());
            (_c = document
                .querySelector(constants.ui.selectors.promotionChoice)) === null || _c === void 0 ? void 0 : _c.addEventListener(constants.ui.events.click, (e) => this.onPawnPromotionChoice(e));
            this.gameService.getFenAndUpdate();
        }
        onDragStart(source, piece, position, orientation) {
            // logging.logDragStart(source, piece, position, orientation);
            // protect against wrong side moves
            if ((piece[0] === 'w' && this.gameObjects.gameState.activeColor === 0)
                || (piece[0] === 'b' && this.gameObjects.gameState.activeColor === 1)) {
                return false;
            }
            document
                .querySelectorAll(constants.selectors.attacking)
                .forEach(a => utilities.removeClassName(a, constants.classes.attacking));
            document
                .querySelectorAll(constants.selectors.protecting)
                .forEach(a => utilities.removeClassName(a, constants.classes.protecting));
            let squareAttacks = utilities.getSquareAttacks(source);
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
                gameService.move(source, target, null);
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
            gameService.goToMove(dataIndex);
        }
        onBackClick() {
            gameService.goBackOneMove();
        }
        onPawnPromotionChoice(e) {
            let choice = e.target.getAttribute(constants.attributes.dataPiece);
            gameService.move(gameObjects.pawnPromotionMoveInfo.source, gameObjects.pawnPromotionMoveInfo.target, parseInt(choice));
            utilities.hidePawnPromotion();
        }
    }
    events.boardEvents = boardEvents;
})(events || (events = {}));
