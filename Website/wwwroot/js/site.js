$(document).ready(function () {
    setup();
});

var setup = function () {
    getFenAndUpdate();;
    $('#fenSubmit').click(function () {
        getFenAndUpdate();
    });
};

var getFenAndUpdate = function () {
    fen = $('.fen').val();
    updateFen(fen);
}

var updateFen = function (fen) {
    board = Chessboard('chessBoard', fen);
};

