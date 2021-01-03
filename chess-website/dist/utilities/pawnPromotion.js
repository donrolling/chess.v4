"use strict";
var utilities;
(function (utilities) {
    class pawnPromotion {
        static displayPawnPromotion(gameObjects, source, target) {
            gameObjects.pawnPromotionInfo = {
                source: source,
                target: target
            };
            gameObjects.freeze = true;
            let ppui = document.querySelector(constants.ui.selectors.pawnPromotion);
            utilities.dom.removeClassName(ppui, constants.ui.classes.hidden);
        }
        static hidePawnPromotion(gameObjects) {
            gameObjects.pawnPromotionInfo = null;
            gameObjects.freeze = false;
            let ppui = document.querySelector(constants.ui.selectors.pawnPromotion);
            utilities.dom.addClassName(ppui, constants.ui.classes.hidden);
        }
    }
    utilities.pawnPromotion = pawnPromotion;
})(utilities || (utilities = {}));
