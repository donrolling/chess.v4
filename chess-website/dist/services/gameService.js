"use strict";
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
var services;
(function (services) {
    class gameService {
        constructor() {
            this.stateInfoUrl = 'api/game/state-info?fen=';
            this.moveUrl = 'api/game/move';
            this.gotoUrl = 'api/game/goto';
        }
        getGameStateInfo(fen) {
            let url = !utilities.strings.isNullOrEmpty(fen)
                ? this.stateInfoUrl + fen
                : this.stateInfoUrl;
            (() => __awaiter(this, void 0, void 0, function* () {
                let response = yield fetch(url);
                if (!response.ok) {
                    throw Error(response.statusText);
                }
                let gameStateResult = yield response.json();
                if (gameStateResult.success) {
                    //utilities.setBoardState(gameStateResult.result);
                }
                else {
                    // reset the board
                    //utilities.setBoardState(gameObjects.gameState);
                }
            }))();
        }
        move(gameState, beginning, destination, piecePromotionType) {
            let data = JSON.stringify({
                GameState: gameState,
                Beginning: beginning,
                Destination: destination,
                PiecePromotionType: piecePromotionType
            });
            utilities.logging.info(data);
            (() => __awaiter(this, void 0, void 0, function* () {
                let response = yield fetch(this.moveUrl, {
                    headers: {
                        'Accept': constants.http.contentTypes.applicationjson,
                        'Content-Type': constants.http.contentTypes.applicationjson
                    },
                    method: constants.http.httpMethods.post,
                    body: data
                });
                if (!response.ok) {
                    throw Error(response.statusText);
                }
                let gameStateResult = yield response.json();
                if (gameStateResult.success) {
                    //utilities.setBoardState(gameStateResult.result);
                }
                else {
                    // reset the board
                    utilities.logging.error(gameStateResult.message);
                    //utilities.setBoardState(gameObjects.gameState);
                }
            }))();
        }
        goToMove(gameObjects, index) {
            if (!gameObjects.gameState) {
                return;
            }
            // if this is the most current move, then we're no longer frozen
            if (gameObjects.gameState.history.length === index) {
                gameObjects.freeze = false;
                gameObjects.freezeNotify = 0;
                // reset the board
                //utilities.setBoardState(gameObjects.gameState);
                return;
            }
            else {
                gameObjects.freeze = true;
            }
            // get historical position
            let history = gameObjects.gameState.history[index];
            let fen = utilities.history.historyToFEN(history);
            // set the input text box
            //document.querySelector<HTMLElement>(constants.ui.selectors.fenInput).value = fen;
            // copy existing config...this isn't a real move, it is a fake move, so copy the existing config
            let newConfig = {
                position: fen,
                draggable: false
            };
            //gameObjects.board = Chessboard(constants.ui.classes.chessBoard, newConfig);
            utilities.history.selectPgnItemByIndex(index);
        }
        goBackOneMove(gameObjects) {
            if (!gameObjects.gameState) {
                return;
            }
            let newIndex = 0; // beginning index
            if (gameObjects.gameState.fullmoveNumber > 1) {
                var activePGNItem = document.querySelector(constants.ui.selectors.activeItem);
                if (!activePGNItem) {
                    return;
                }
                let dataIndex = activePGNItem.getAttribute(constants.ui.attributes.dataIndex);
                if (!dataIndex) {
                    return;
                }
                let index = parseInt(dataIndex);
                newIndex = index - 1;
            }
            let data = JSON.stringify({
                GameState: gameObjects.gameState,
                HistoryIndex: newIndex
            });
            (() => __awaiter(this, void 0, void 0, function* () {
                let response = yield fetch(this.gotoUrl, {
                    headers: {
                        'Accept': constants.http.contentTypes.applicationjson,
                        'Content-Type': constants.http.contentTypes.applicationjson
                    },
                    method: constants.http.httpMethods.post,
                    body: data
                });
                if (!response.ok) {
                    throw Error(response.statusText);
                }
                let gameStateResult = yield response.json();
                if (gameStateResult.success) {
                    //utilities.setBoardState(gameStateResult.result);
                }
                else {
                    // reset the board
                    //utilities.setBoardState(gameObjects.gameState);
                }
            }))();
        }
        getAnyFenAndUpdate() {
            let fen = utilities.dom.getParameterByName('fen');
            if (!utilities.strings.isNullOrEmpty(fen)) {
                this.getGameStateInfo(fen);
            }
            else {
                this.getFenAndUpdate();
            }
        }
        getFenAndUpdate() {
            let fenInput = document.querySelector(constants.ui.selectors.fenInput);
            if (fenInput) {
                this.getGameStateInfo(fenInput.value);
            }
        }
    }
    services.gameService = gameService;
})(services || (services = {}));
