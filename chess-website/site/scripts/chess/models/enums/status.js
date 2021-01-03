define(["require", "exports"], function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.status = void 0;
    var status;
    (function (status) {
        status[status["Success"] = 0] = "Success";
        status[status["Failure"] = 1] = "Failure";
        status[status["ItemNotFound"] = 2] = "ItemNotFound";
        status[status["Cancelled"] = 3] = "Cancelled";
        status[status["Aborted"] = 4] = "Aborted";
        status[status["Expired"] = 5] = "Expired";
    })(status = exports.status || (exports.status = {}));
});
