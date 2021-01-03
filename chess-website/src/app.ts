import { BoardConfig } from "chessboardjs";
import { gameObjects } from "./models/chessApp/gameObjects";
import { gameService } from "./services/gameService";
import { gameStateService } from "./services/gameStateService";
import { pawnPromotion } from "./utilities/pawnPromotion";

export class app {
    constructor() {
        console.log('App CTOR');

        let gameObjects: gameObjects = {
            board: null,
            gameState: null,
            pawnPromotionInfo: null,
            freeze: false,
            freezeNotify: 0
        };

        let config: BoardConfig = {
            position: '',
            draggable: true
        };

        let _gameService = new gameService();

        let _gameStateService = new gameStateService(
            gameObjects,
            config,
            _gameService
        );

        document
            .querySelector(constants.ui.selectors.fenSubmit)
            ?.addEventListener(constants.ui.events.click, () => _gameStateService.getFenAndUpdate());

        document
            .querySelector(constants.ui.selectors.backBtn)
            ?.addEventListener(constants.ui.events.click, async () => {
                let response = await _gameService.goBackOneMove(gameObjects);
                if (response.success) {
                    _gameStateService.setBoardState(response.result);
                } else {
                    // reset the board
                    if (gameObjects.gameState) {                        
                        _gameStateService.setBoardState(gameObjects.gameState);
                    }
                }
            });

        let promotionChoiceElement = document.querySelector(constants.ui.selectors.promotionChoice);
        if (promotionChoiceElement) {
            promotionChoiceElement.addEventListener(
                constants.ui.events.click,
                (e: Event) => { 
                    let element = e.target as HTMLElement;
                    let choice = element.getAttribute(constants.ui.attributes.dataPiece);
                    if (!choice) {
                        return;
                    }
                    let promotionPieceType = parseInt(choice);
                    if (!promotionPieceType || !gameObjects.pawnPromotionInfo || !gameObjects.gameState) {
                        return;
                    }
                    _gameService.move(
                        gameObjects.gameState,
                        gameObjects.pawnPromotionInfo.source, 
                        gameObjects.pawnPromotionInfo.target, 
                        promotionPieceType
                    );
                    pawnPromotion.hidePawnPromotion(gameObjects);
                }
            );
        }

        // start using any existing fen source, preferring the querystring
        _gameStateService.getAnyFenAndUpdate();
    }
}