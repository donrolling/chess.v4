"use strict";
var utilities;
(function (utilities) {
    class arrays {
        static removeItemFromArray(xs, x) {
            let index = xs.indexOf(x);
            if (index > -1) {
                xs.splice(index, 1);
            }
        }
    }
    utilities.arrays = arrays;
})(utilities || (utilities = {}));
