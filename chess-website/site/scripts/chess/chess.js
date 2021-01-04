var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
define(["require", "exports", "./constants/ui/attributes", "./constants/ui/events", "./constants/ui/selectors", "./services/gameService", "./services/gameStateService", "./utilities/pawnPromotion"], function (require, exports, attributes_1, events_1, selectors_1, gameService_1, gameStateService_1, pawnPromotion_1) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.chess = void 0;
    class chess {
        constructor(config) {
            // config values
            let baseurl = config.get('webapi.baseurl');
            let _gameService = new gameService_1.gameService(baseurl);
            let gameObjects = {
                board: null,
                gameState: null,
                pawnPromotionInfo: null,
                freeze: false,
                freezeNotify: 0
            };
            let boardConfig = {
                position: '',
                draggable: true
            };
            let _gameStateService = new gameStateService_1.gameStateService(gameObjects, boardConfig, _gameService);
            let fenSubmit = document.querySelector(selectors_1.selectors.fenSubmit);
            if (fenSubmit) {
                fenSubmit.addEventListener(events_1.events.click, () => _gameStateService.getFenAndUpdate());
            }
            let backBtn = document.querySelector(selectors_1.selectors.fenSubmit);
            if (backBtn) {
                backBtn.addEventListener(events_1.events.click, () => __awaiter(this, void 0, void 0, function* () {
                    let response = yield _gameService.goBackOneMove(gameObjects);
                    if (response.success) {
                        _gameStateService.setBoardState(response.result);
                    }
                    else {
                        // reset the board
                        if (gameObjects.gameState) {
                            _gameStateService.setBoardState(gameObjects.gameState);
                        }
                    }
                }));
            }
            let promotionChoiceElement = document.querySelector(selectors_1.selectors.promotionChoice);
            if (promotionChoiceElement) {
                promotionChoiceElement.addEventListener(events_1.events.click, (e) => {
                    let element = e.target;
                    let choice = element.getAttribute(attributes_1.attributes.dataPiece);
                    if (!choice) {
                        return;
                    }
                    let promotionPieceType = parseInt(choice);
                    if (!promotionPieceType || !gameObjects.pawnPromotionInfo || !gameObjects.gameState) {
                        return;
                    }
                    _gameService.move(gameObjects.gameState, gameObjects.pawnPromotionInfo.source, gameObjects.pawnPromotionInfo.target, promotionPieceType);
                    pawnPromotion_1.pawnPromotion.hidePawnPromotion(gameObjects);
                });
            }
            // start using any existing fen source, preferring the querystring
            _gameStateService.getAnyFenAndUpdate();
        }
    }
    exports.chess = chess;
});
