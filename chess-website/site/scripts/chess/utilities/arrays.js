define(["require", "exports"], function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.arrays = void 0;
    class arrays {
        static removeItemFromArray(xs, x) {
            let index = xs.indexOf(x);
            if (index > -1) {
                xs.splice(index, 1);
            }
        }
    }
    exports.arrays = arrays;
});
