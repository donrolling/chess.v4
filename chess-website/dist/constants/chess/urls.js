"use strict";
var constants;
(function (constants) {
    var chess;
    (function (chess) {
        class urls {
        }
        urls.stateInfo = 'api/game/state-info?fen=';
        urls.move = 'api/game/move';
        urls.goto = 'api/game/goto';
        chess.urls = urls;
    })(chess = constants.chess || (constants.chess = {}));
})(constants || (constants = {}));
