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
        getGameStateInfo(fen) {
            let url = !utilities.strings.isNullOrEmpty(fen)
                ? constants.chess.urls.stateInfo + fen
                : constants.chess.urls.stateInfo;
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
            let url = constants.chess.urls.move;
            (() => __awaiter(this, void 0, void 0, function* () {
                let response = yield fetch(url, {
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
                let url = constants.chess.urls.goto;
                let response = yield fetch(url, {
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
        getFenAndUpdate() {
            let fen = utilities.getParameterByName('fen');
            if (utilities.isNullOrEmpty(fen)) {
                fen = document.querySelector(constants.selectors.fenInput).value;
            }
            gameService.getGameStateInfo(fen);
        }
    }
    services.gameService = gameService;
})(services || (services = {}));
