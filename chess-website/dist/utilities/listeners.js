"use strict";
var utilities;
(function (utilities) {
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
    utilities.eventlisteners = eventlisteners;
})(utilities || (utilities = {}));
