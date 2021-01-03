"use strict";
var constants;
(function (constants) {
    var ui;
    (function (ui) {
        class classes {
        }
        classes.active = "active";
        classes.items = "items";
        classes.item = "item";
        classes.number = "number";
        classes.attacking = "attacking";
        classes.chessBoard = "chessBoard";
        classes.piece = "piece";
        classes.protecting = "protecting";
        classes.square = "square";
        classes.pawnPromotion = "pawnPromotion";
        classes.hidden = "hidden";
        ui.classes = classes;
    })(ui = constants.ui || (constants.ui = {}));
})(constants || (constants = {}));
