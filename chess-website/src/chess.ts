import { BoardConfig } from "chessboardjs";
import { IConfig } from "config";

import { attributes } from "./constants/ui/attributes";
import { events } from "./constants/ui/events";
import { selectors } from "./constants/ui/selectors";
import { gameObjects } from "./models/chessApp/gameObjects";
import { gameService } from "./services/gameService";
import { gameStateService } from "./services/gameStateService";
import { pawnPromotion } from "./utilities/pawnPromotion";

export class chess {
    constructor(config: IConfig) {
        // config values
        let baseurl: string = config.get('webapi.baseurl');
        let _gameService = new gameService(baseurl);

        let gameObjects: gameObjects = {
            board: null,
            gameState: null,
            pawnPromotionInfo: null,
            freeze: false,
            freezeNotify: 0
        };

        let boardConfig: BoardConfig = {
            position: '',
            draggable: true
        };

        let _gameStateService = new gameStateService(
            gameObjects,
            boardConfig,
            _gameService
        );

        let fenSubmit = document.querySelector(selectors.fenSubmit);
        if (fenSubmit) {
            fenSubmit.addEventListener(events.click, () => _gameStateService.getFenAndUpdate());
        }

        let backBtn = document.querySelector(selectors.fenSubmit);
        if (backBtn) {
            backBtn.addEventListener(events.click, async () => {
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
        }

        let promotionChoiceElement = document.querySelector(selectors.promotionChoice);
        if (promotionChoiceElement) {
            promotionChoiceElement.addEventListener(
                events.click,
                (e: Event) => {
                    let element = e.target as HTMLElement;
                    let choice = element.getAttribute(attributes.dataPiece);
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