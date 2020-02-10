$(document).ready(function () {
    setup();
});

var setup = function () {
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
    $.ajax({
        url: "Game/?fen=" + fen,
        context: document.body
    }).done(function (gamestate) {
        console.log(gamestate);
        board = Chessboard('chessBoard', fen);
    });
};

