"use strict";
var utilities;
(function (utilities) {
    class boardState {
        static setBoardState(gameObjects, gameState, config) {
            let x = document.querySelector(constants.ui.selectors.fenInput);
            if (!x) {
                return;
            }
            x.value = gameState.fen;
            config.position = gameState.fen;
            gameObjects.gameState = gameState;
            //gameObjects.board = Chessboard(constants.ui.classes.chessBoard, config);
            if (!config.draggable) {
                document
                    .querySelectorAll(constants.ui.selectors.allSquares)
                    .forEach(a => a.addEventListener(constants.ui.events.click, e => events.handlers.handleSquareClick(e)));
            }
            utilities.history.setHistoryPanel(gameState.pgn);
            // remove event listeners
            events.listeners.removeEventListeners(constants.ui.selectors.item, constants.ui.events.click, events.boardEvents.onPGNClick);
            // add event listeners
            events.listeners.addEventListeners(constants.ui.selectors.item, constants.ui.events.click, events.boardEvents.onPGNClick);
        }
    }
    utilities.boardState = boardState;
})(utilities || (utilities = {}));
