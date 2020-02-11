var board;

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

    }).done(function (gamestate) {
        console.log(gamestate);
        board = Chessboard('chessBoard', fen);
    });
};

