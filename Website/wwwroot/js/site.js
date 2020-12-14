var board;
var gamestate;
var attacks;


var onDragStart = function (source, piece, position, orientation) {
    console.log('Drag started:')
    console.log('Source: ' + source)
    console.log('Piece: ' + piece)
    console.log('Position: ' + Chessboard.objToFen(position))
    console.log('Orientation: ' + orientation)
    console.log('~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~')

    removeOldClasses();
    var squareAttacks = getSquareAttacks(source);
    console.log(squareAttacks);
    highlightSquares(squareAttacks);
}

function onDrop(source, target, piece, newPos, oldPos, orientation) {
    console.log('Source: ' + source)
    console.log('Target: ' + target)
    console.log('Piece: ' + piece)
    console.log('New position: ' + Chessboard.objToFen(newPos))
    console.log('Old position: ' + Chessboard.objToFen(oldPos))
    console.log('Orientation: ' + orientation)
    console.log('~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~')
}

var config = {
    draggable: true,
    dropOffBoard: 'snapback', // this is the default
    onDragStart: onDragStart,
    onDrop: onDrop
}

$(document).ready(function () {
    setup();
});

var setup = function () {
    $('#fenSubmit').click(function () {
        getFenAndUpdate();
    });
    var fen = $('.fen').val();
    updateBoardStateViaFen(fen);
};

var updateBoardStateViaFen = function (fen) {
    if (!fen) {
        return;
    }
    var self = this;
    $.ajax({
        type: "POST",
        url: "api/game",
        data: fen,
        contentType: 'application/json',
        dataType: 'json'

    }).done(function (gamestateResult) {
        gamestate = gamestateResult.result;
        attacks = gamestate.attacks;
        self.setBoardState(fen);
    });
};

var setBoardState = function (fen) {
    config.position = fen;
    console.log(config);
    board = Chessboard('chessBoard', config);
    if (!config.draggable) {
        $('.square-55d63').click(function (e) {
            handleSquareClick(e);
        })
    }
}

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

