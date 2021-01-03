import { gameObjects } from "../models/chessApp/gameObjects";
import { dom } from "./dom";

export class pawnPromotion {
    public static displayPawnPromotion(gameObjects: gameObjects, source: string, target: string): void {
        gameObjects.pawnPromotionInfo = {
            source: source,
            target: target
        };
        gameObjects.freeze = true;
        let ppui = document.querySelector(constants.ui.selectors.pawnPromotion);
        dom.removeClassName(ppui, constants.ui.classes.hidden);
    }

    public static hidePawnPromotion(gameObjects: gameObjects): void {
        gameObjects.pawnPromotionInfo = null;
        gameObjects.freeze = false;
        let ppui = document.querySelector(constants.ui.selectors.pawnPromotion);
        dom.addClassName(ppui, constants.ui.classes.hidden);
    }
}