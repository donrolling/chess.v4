define(["require", "exports"], function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.strings = void 0;
    class strings {
        static isNullOrEmpty(x) {
            return x === undefined || !x;
        }
    }
    exports.strings = strings;
});
