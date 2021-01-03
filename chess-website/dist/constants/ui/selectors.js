"use strict";
var constants;
(function (constants) {
    var ui;
    (function (ui) {
        class selectors {
        }
        selectors.allSquares = '.square-55d63';
        selectors.fenSubmit = '#fenSubmit';
        selectors.fenInput = '.fen';
        selectors.attacking = '.attacking';
        selectors.protecting = '.protecting';
        selectors.square = '.square-';
        selectors.history = '.history';
        selectors.items = '.items';
        selectors.item = '.item';
        selectors.activeItem = '.item.active';
        selectors.backBtn = '#backBtn';
        selectors.pawnPromotion = '.pawnPromotion';
        selectors.promotionChoice = '.promotionChoice';
        ui.selectors = selectors;
    })(ui = constants.ui || (constants.ui = {}));
})(constants || (constants = {}));
