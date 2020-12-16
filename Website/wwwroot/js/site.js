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
        square: '.square-'
    },

    urls: {
        stateInfo: 'api/game/state-info?fen=',
        move: 'api/game/move'
    }
};

let gameObjects = {
    board: {},
    gameState: {}
};

let utilities = {
    getFenAndUpdate: () => {
        let fen = $(constants.selectors.fenInput).val();
        gameService.getGameStateInfo(fen);
    },

    setBoardState: (fen) => {
        config.position = fen;
        gameObjects.board = Chessboard(constants.chessBoard, config);
        if (!config.draggable) {
            $(constants.selectors.allSquares).click((e) => handlers.handleSquareClick(e));
        }
    },

    getCurrentSquare: (e) => {
        let obj = $(e.target);
        let piece = obj.data(constants.piece);
        if (piece) {
            obj = $(e.target).parent();
        }
        return obj.data(constants.square);
    },

    getSquareAttacks: (square) => {
        let squareAttacks = [];
        for (let i = 0; i < gameObjects.gameState.attacks.length; i++) {
            let attack = gameObjects.gameState.attacks[i];
            if (attack.attackingSquare.name === square) {
                squareAttacks.push(attack);
            }
        }
        return squareAttacks;
    },

    getSquareSelector: (name) => {
        return constants.selectors.square + name
    },

    highlightSquares: (attacks) => {
        for (let i = 0; i < attacks.length; i++) {
            let attack = attacks[i];
            let squareClass = attack.isProtecting ? constants.protecting : constants.attacking;
            let squareSelector = utilities.getSquareSelector(attack.name);
            $(squareSelector).addClass(squareClass);
        }
    },

    removeOldClasses: () => {
        $(constants.selectors.attacking).removeClass(constants.attacking);
        $(constants.selectors.protecting).removeClass(constants.protecting);
    }
};

let events = {
    init: () => {
        $(constants.selectors.fenSubmit).click(() => utilities.getFenAndUpdate());
        utilities.getFenAndUpdate();
    },

    onDragStart: (source, piece, position, orientation) => {
        utilities.removeOldClasses();
        let squareAttacks = utilities.getSquareAttacks(source);
        utilities.highlightSquares(squareAttacks);
    },

    onDrop: (source, target, piece, newPos, oldPos, orientation) => {
        let squareAttacks = utilities.getSquareAttacks(source);
        // logging.logDrop(source, target, piece, newPos, oldPos, orientation, squareAttacks);
        if (!squareAttacks.some(x => x.name === target)) {
            return constants.snapback;
        }
        // todo: piece promotion selection
        // constants.pieceTypes.Bishop....
        let piecePromotionType = null;
        gameService.move(source, target, piecePromotionType);
    }
};

let handlers = {
    handleSquareClick: (e) => {
        utilities.removeOldClasses();
        let currentSquare = utilities.getCurrentSquare(e);
        let squareAttacks = utilities.getSquareAttacks(currentSquare);
        utilities.highlightSquares(squareAttacks);
    }
};

let gameService = {
    getGameStateInfo: (fen) => {
        if (!fen) { return; }
        (async () => {
            let url = constants.urls.stateInfo + fen;
            logging.log(url);
            let response = await fetch(url);
            if (!response.ok) {
                throw Error(response.statusText);
            }
            let gameStateResult = await response.json();
            logging.log(gameStateResult);
            if (gameStateResult.success) {
                gameObjects.gameState = gameStateResult.result;
            } else {
                console.log(gameStateResult.message);
            }
            // will either advance or reset the game
            utilities.setBoardState(gameObjects.gameState.fen);
        })();
    },

    move: (beginning, destination, piecePromotionType) => {
        let data = JSON.stringify({
            GameState: gameObjects.gameState,
            Beginning: beginning,
            Destination: destination,
            PiecePromotionType: piecePromotionType
        });
        (async () => {
            let url = constants.urls.move;
            let response = await fetch(
                url,
                {
                    headers: {
                        'Accept': constants.http.contentTypes.applicationjson,
                        'Content-Type': constants.http.contentTypes.applicationjson
                    },
                    method: constants.http.post,
                    body: data
                }
            );
            if (!response.ok) {
                throw Error(response.statusText);
            }
            let gameStateResult = await response.json();
            logging.log(gameStateResult);
            if (gameStateResult.success) {
                gameObjects.gameState = gameStateResult.result;
            } else {
                console.log(gameStateResult.message);
            }
            // will either advance or reset the game
            utilities.setBoardState(gameObjects.gameState.fen);
        })();
    }
};

let logging = {
    logTest: () => console.log('test'),

    log: (x) => console.log(x),

    logDragStart: (source, piece, position, orientation) =>
        console.log({
            Event: constants.events.onDragStart,
            Source: source,
            Piece: piece,
            Position: Chessboard.objToFen(position),
            Orientation: orientation
        }),

    logDrop: (source, target, piece, newPos, oldPos, orientation, squareAttacks) =>
        console.log({
            Event: constants.events.onDrop,
            Source: source,
            Target: target,
            Piece: piece,
            NewPosition: newPos,
            NewFen: Chessboard.objToFen(newPos),
            OldPosition: oldPos,
            OldFen: Chessboard.objToFen(oldPos),
            Orientation: orientation,
            SquareAttacks: squareAttacks,
        })
}

let config = {
    position: '',
    draggable: true,
    dropOffBoard: constants.snapback, // this is the default
    onDragStart: events.onDragStart,
    onDrop: events.onDrop
}

$(document).ready(() => events.init());
