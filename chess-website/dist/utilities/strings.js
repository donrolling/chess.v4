"use strict";
var utilities;
(function (utilities) {
    class strings {
        static isNullOrEmpty(x) {
            return x === undefined || !x;
        }
    }
    utilities.strings = strings;
})(utilities || (utilities = {}));
