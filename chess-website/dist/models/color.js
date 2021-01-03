"use strict";
var models;
(function (models) {
    let color;
    (function (color) {
        color[color["black"] = 0] = "black";
        color[color["white"] = 1] = "white";
    })(color = models.color || (models.color = {}));
})(models || (models = {}));
