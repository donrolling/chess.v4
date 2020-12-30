let constants = {
    methodResponses: {
        snapback: 'snapback'
    },

    pieceTypes: {
        pawn: 0,
        knight: 1,
        bishop: 2,
        rook: 3,
        queen: 4,
        king: 5
    },

    attributes: {
        dataIndex: 'data-index'
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

    classes: {
        active: 'active',
        items: 'items',
        item: 'item',
        number: 'number',
        attacking: 'attacking',
        chessBoard: 'chessBoard',
        piece: 'piece',
        protecting: 'protecting',
        square: 'square'
    },

    events: {
        onDragStart: 'onDragStart',
        onDrop: 'onDrop',
        click: 'click',
    },

    selectors: {
        allSquares: '.square-55d63',
        fenSubmit: '#fenSubmit',
        fenInput: '.fen',
        attacking: '.attacking',
        protecting: '.protecting',
        square: '.square-',
        history: '.history',
        items: '.items',
        item: '.item',
        activeItem: '.item.active',
        backBtn: '#backBtn'
    },

    urls: {
        stateInfo: 'api/game/state-info?fen=',
        move: 'api/game/move',
        goto: 'api/game/goto'
    }
};