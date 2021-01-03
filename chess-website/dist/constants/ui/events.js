"use strict";
var constants;
(function (constants) {
    var ui;
    (function (ui) {
        class events {
        }
        events.onDragStart = "onDragStart";
        events.onDrop = "onDrop";
        events.click = "click";
        ui.events = events;
    })(ui = constants.ui || (constants.ui = {}));
})(constants || (constants = {}));
