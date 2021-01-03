define(["require", "exports"], function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.eventlisteners = void 0;
    class eventlisteners {
        static addEventListeners(selector, event, handler) {
            let items = document.querySelectorAll(selector);
            if (items) {
                for (const item of items) {
                    item.addEventListener(event, handler);
                }
            }
        }
        static removeEventListeners(selector, event, handler) {
            let items = document.querySelectorAll(selector);
            if (items) {
                for (const item of items) {
                    item.removeEventListener(event, handler);
                }
            }
        }
    }
    exports.eventlisteners = eventlisteners;
});
