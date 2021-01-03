var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
define(["require", "exports", "chessboardjs", "../constants/http/contentTypes", "../constants/http/httpMethods", "../constants/ui/attributes", "../constants/ui/classes", "../constants/ui/selectors", "../utilities/historyEngine", "../utilities/logging", "../utilities/strings"], function (require, exports, chessboardjs_1, contentTypes_1, httpMethods_1, attributes_1, classes_1, selectors_1, historyEngine_1, logging_1, strings_1) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.gameService = void 0;
    class gameService {
        constructor() {
            this.stateInfoUrl = 'api/game/state-info?fen=';
            this.moveUrl = 'api/game/move';
            this.gotoUrl = 'api/game/goto';
        }
        getGameStateInfo(fen) {
            return __awaiter(this, void 0, void 0, function* () {
                let url = !strings_1.strings.isNullOrEmpty(fen)
                    ? this.stateInfoUrl + fen
                    : this.stateInfoUrl;
                let response = yield fetch(url);
                if (!response.ok) {
                    throw Error(response.statusText);
                }
                let json = yield response.json();
                return json;
            });
        }
        move(gameState, beginning, destination, piecePromotionType) {
            return __awaiter(this, void 0, void 0, function* () {
                let data = JSON.stringify({
                    GameState: gameState,
                    Beginning: beginning,
                    Destination: destination,
                    PiecePromotionType: piecePromotionType
                });
                logging_1.logging.info(data);
                let response = yield fetch(this.moveUrl, {
                    headers: {
                        'Accept': contentTypes_1.contentTypes.applicationjson,
                        'Content-Type': contentTypes_1.contentTypes.applicationjson
                    },
                    method: httpMethods_1.httpMethods.post,
                    body: data
                });
                if (!response.ok) {
                    throw Error(response.statusText);
                }
                let json = yield response.json();
                return json;
            });
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
            let fen = historyEngine_1.historyEngine.historyToFEN(history);
            // set the input text box
            let fenInput = document.querySelector(selectors_1.selectors.fenInput);
            if (fenInput) {
                fenInput.value = fen;
            }
            // copy existing config...this isn't a real move, it is a fake move, so copy the existing config
            let newConfig = {
                position: fen,
                draggable: false
            };
            gameObjects.board = chessboardjs_1.ChessBoard(classes_1.classes.chessBoard, newConfig);
            historyEngine_1.historyEngine.selectPgnItemByIndex(index);
        }
        goBackOneMove(gameObjects) {
            return __awaiter(this, void 0, void 0, function* () {
                if (!gameObjects.gameState) {
                    return Promise.reject(new Error('gamestate was null'));
                }
                let newIndex = 0; // beginning index
                if (gameObjects.gameState.fullmoveNumber > 1) {
                    var activePGNItem = document.querySelector(selectors_1.selectors.activeItem);
                    if (!activePGNItem) {
                        return Promise.reject(new Error('activePGNItem was null'));
                    }
                    let dataIndex = activePGNItem.getAttribute(attributes_1.attributes.dataIndex);
                    if (!dataIndex) {
                        return Promise.reject(new Error('dataIndex was null'));
                    }
                    let index = parseInt(dataIndex);
                    newIndex = index - 1;
                }
                let data = JSON.stringify({
                    GameState: gameObjects.gameState,
                    HistoryIndex: newIndex
                });
                let response = yield fetch(this.gotoUrl, {
                    headers: {
                        'Accept': contentTypes_1.contentTypes.applicationjson,
                        'Content-Type': contentTypes_1.contentTypes.applicationjson
                    },
                    method: httpMethods_1.httpMethods.post,
                    body: data
                });
                if (!response.ok) {
                    throw Error(response.statusText);
                }
                let json = yield response.json();
                return json;
            });
        }
    }
    exports.gameService = gameService;
});
