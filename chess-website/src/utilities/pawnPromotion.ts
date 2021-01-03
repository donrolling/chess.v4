import { classes } from "../constants/ui/classes";
import { selectors } from "../constants/ui/selectors";
import { gameObjects } from "../models/chessApp/gameObjects";
import { dom } from "./dom";

export class pawnPromotion {
    public static displayPawnPromotion(gameObjects: gameObjects, source: string, target: string): void {
        gameObjects.pawnPromotionInfo = {
            source: source,
            target: target
        };
        gameObjects.freeze = true;
        let ppui = document.querySelector(selectors.pawnPromotion);
        dom.removeClassName(ppui, classes.hidden);
    }

    public static hidePawnPromotion(gameObjects: gameObjects): void {
        gameObjects.pawnPromotionInfo = null;
        gameObjects.freeze = false;
        let ppui = document.querySelector(selectors.pawnPromotion);
        dom.addClassName(ppui, classes.hidden);
    }
}