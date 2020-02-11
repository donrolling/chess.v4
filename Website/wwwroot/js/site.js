var board;
var gamestate;
var attacks;

$(document).ready(function () {
    setup();
});

var setup = function () {
    board = Chessboard('chessBoard', 'start');
    getFenAndUpdate();
    $('#fenSubmit').click(function () {
        getFenAndUpdate();
    });
};

var getFenAndUpdate = function () {
    fen = $('.fen').val();
    updateFen(fen);
};

var updateFen = function (fen) {
    if (!fen) {
        return;
    }
    var data = fen;
    $.ajax({
        type: "POST",
        url: "api/game",
        data: data,
        contentType: 'application/json',
        dataType: 'json'

    }).done(function (gamestateResult) {
        gamestate = gamestateResult.result;
        attacks = gamestate.attacks;
        board = Chessboard('chessBoard', fen);
        $('.square-55d63').click(function (e) {
            handleSquareClick(e);
        });
    });
};

var getCurrentSquare = function (e) {
    var obj = $(e.target);
    var piece = obj.data('piece');
    if (piece) {
        obj = $(e.target).parent();
    }
    return obj.data('square');
};

var getSquareAttacks = function(square) {
    var squareAttacks = [];
    for (var i = 0; i < attacks.length; i++) {
        var attack = attacks[i];
        if (attack.attackingSquare.name === square) {
            squareAttacks.push(attack);
        }
    }
    return squareAttacks;
};

var highlightSquares = function (attacks) {
    for (var i = 0; i < attacks.length; i++) {
        var attack = attacks[i];
        var squareClass = attack.isProtecting ? 'protecting' : 'attacking';
        $('.square-' + attack.name).addClass(squareClass);
    }
};

var removeOldClasses = function () {
    $('.attacking').removeClass('attacking');
    $('.protecting').removeClass('protecting');
};

var handleSquareClick = function (e) {
    removeOldClasses();
    var currentSquare = getCurrentSquare(e);
    var squareAttacks = getSquareAttacks(currentSquare);
    highlightSquares(squareAttacks);
};

