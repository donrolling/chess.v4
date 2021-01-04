define(function (require) {
    const chessModule = require('./chess');
    const config = require('config');
    new chessModule.chess(config);
});