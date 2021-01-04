define(function (require) {
    const chessModule = require('./chess');
    //require('requirejs-text/text');
    var json = require('text!../../appsettings.json');
    console.log(json);
    // if (json) {
    //     new chessModule.chess(JSON.parse(config));
    // }
});