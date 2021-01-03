define(["require", "exports", "../constants/ui/classes", "../constants/ui/selectors", "./dom"], function (require, exports, classes_1, selectors_1, dom_1) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.pawnPromotion = void 0;
    class pawnPromotion {
        static displayPawnPromotion(gameObjects, source, target) {
            gameObjects.pawnPromotionInfo = {
                source: source,
                target: target
            };
            gameObjects.freeze = true;
            let ppui = document.querySelector(selectors_1.selectors.pawnPromotion);
            dom_1.dom.removeClassName(ppui, classes_1.classes.hidden);
        }
        static hidePawnPromotion(gameObjects) {
            gameObjects.pawnPromotionInfo = null;
            gameObjects.freeze = false;
            let ppui = document.querySelector(selectors_1.selectors.pawnPromotion);
            dom_1.dom.addClassName(ppui, classes_1.classes.hidden);
        }
    }
    exports.pawnPromotion = pawnPromotion;
});
