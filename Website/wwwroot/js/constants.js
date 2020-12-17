let constants = {
    attacking: 'attacking',
    chessBoard: 'chessBoard',
    piece: 'piece',
    protecting: 'protecting',
    snapback: 'snapback',
    square: 'square',

    pieceTypes: {
        pawn: 0,
        knight: 1,
        bishop: 2,
        rook: 3,
        queen: 4,
        king: 5
    },

    http: {
        post: 'POST',
        get: 'GET',

        contentTypes: {
            applicationjson: 'application/json',
            text: 'text'
        },

        dataTypes: {
            json: 'json'
        }
    },

    events: {
        onDragStart: 'onDragStart',
        onDrop: 'onDrop',
    },

    selectors: {
        allSquares: '.square-55d63',
        fenSubmit: '#fenSubmit',
        fenInput: '.fen',
        attacking: '.attacking',
        protecting: '.protecting',
        square: '.square-',
        history: '.history',
        items: '.items'
    },

    urls: {
        stateInfo: 'api/game/state-info?fen=',
        move: 'api/game/move'
    }
};