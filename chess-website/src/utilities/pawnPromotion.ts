namespace utilities {
    export class pawnPromotion {
        public static displayPawnPromotion(gameObjects: models.gameObjects, source: string, target: string): void {
            gameObjects.pawnPromotionInfo = {
                source: source,
                target: target
            };
            gameObjects.freeze = true;
            let ppui = document.querySelector(constants.ui.selectors.pawnPromotion);
            utilities.dom.removeClassName(ppui, constants.ui.classes.hidden);
        }

        public static hidePawnPromotion(gameObjects: models.gameObjects): void {
            gameObjects.pawnPromotionInfo = null;
            gameObjects.freeze = false;
            let ppui = document.querySelector(constants.ui.selectors.pawnPromotion);
            utilities.dom.addClassName(ppui, constants.ui.classes.hidden);
        }
    }
}