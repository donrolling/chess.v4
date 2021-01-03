"use strict";
var events;
(function (events) {
    class listeners {
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
    events.listeners = listeners;
})(events || (events = {}));
