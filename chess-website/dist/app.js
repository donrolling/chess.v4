"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.app = void 0;
class app {
    constructor() {
        var _a, _b;
        let gameObjects = {
            board: null,
            gameState: null,
            pawnPromotionInfo: null,
            freeze: false,
            freezeNotify: 0
        };
        let config = {
            position: '',
            draggable: true
        };
        let gameService = new services.gameService();
        let gameStateService = new services.gameStateService(gameObjects, config, gameService);
        (_a = document
            .querySelector(constants.ui.selectors.fenSubmit)) === null || _a === void 0 ? void 0 : _a.addEventListener(constants.ui.events.click, () => gameService.getFenAndUpdate());
        (_b = document
            .querySelector(constants.ui.selectors.backBtn)) === null || _b === void 0 ? void 0 : _b.addEventListener(constants.ui.events.click, () => gameService.goBackOneMove(gameObjects));
        let promotionChoiceElement = document.querySelector(constants.ui.selectors.promotionChoice);
        if (promotionChoiceElement) {
            promotionChoiceElement.addEventListener(constants.ui.events.click, (e) => {
                let element = e.target;
                let choice = element.getAttribute(constants.ui.attributes.dataPiece);
                if (!choice) {
                    return;
                }
                let promotionPieceType = parseInt(choice);
                if (!promotionPieceType || !gameObjects.pawnPromotionInfo || !gameObjects.gameState) {
                    return;
                }
                gameService.move(gameObjects.gameState, gameObjects.pawnPromotionInfo.source, gameObjects.pawnPromotionInfo.target, promotionPieceType);
                utilities.pawnPromotion.hidePawnPromotion(gameObjects);
            });
        }
        // start using any existing fen source, preferring the querystring
        gameService.getAnyFenAndUpdate();
    }
}
exports.app = app;
