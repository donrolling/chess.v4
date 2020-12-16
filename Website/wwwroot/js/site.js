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

let gameObjects = {
    board: {},
    gameState: {}
};

let utilities = {
    getFenAndUpdate: () => {
        let fen = utilities.getParameterByName('fen');
        if (utilities.isNullOrEmpty(fen)) {
            fen = document.querySelector(constants.selectors.fenInput).value;
        }
        gameService.getGameStateInfo(fen);
    },

    isNullOrEmpty: (x) => x === undefined || !x,

    setBoardState: (gameState) => {
        document.querySelector(constants.selectors.fenInput).value = gameState.fen;
        logging.log(gameState);
        config.position = gameState.fen;
        gameObjects.gameState = gameState;
        gameObjects.board = Chessboard(constants.chessBoard, config);
        if (!config.draggable) {
            document
                .querySelectorAll(constants.selectors.allSquares)
                .forEach(a =>
                    a.addEventListener('click', e => handlers.handleSquareClick(e))
                );
        }
        if (!gameObjects.gameState.feN_Records || gameObjects.gameState.feN_Records.length === 0) {
            return;
        }
        let itemContainer = document.querySelector(constants.selectors.items);
        let contentList = gameObjects.gameState
            .feN_Records
            .map(a => a.pgn ? a.pgn : 'test')
            .join('</div><div class="item">');
        let content = `<div class="item">${ contentList }</div>`;
        itemContainer.innerHTML = content;
    },

    getCurrentSquare: (squareElement) => {
        logging.log(squareElement.target);
        let square = document.querySelector(squareElement.target);
        logging.log(square);
        let piece = square.data(constants.piece);
        logging.log(piece);
        if (piece) {
            square = document.querySelector(squareElement.target).parentElement;
        }
        logging.log(square);
        return square.data(constants.square);
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
            var square = document.querySelector(squareSelector);
            square.className += ` ${ squareClass }`;
        }
    },

    removeOldClasses: () => {
        document.querySelectorAll(constants.selectors.attacking).forEach(a => a.className = a.className.replace(constants.attacking, ''));
        document.querySelectorAll(constants.selectors.protecting).forEach(a => a.className = a.className.replace(constants.protecting, ''));
    },

    getParameterByName: (name, url = window.location.href) => {
        name = name.replace(/[\[\]]/g, '\\$&');
        var regex = new RegExp('[?&]' + name + '(=([^&#]*)|&|#|$)'),
            results = regex.exec(url);
        if (!results) return null;
        if (!results[2]) return '';
        return decodeURIComponent(results[2].replace(/\+/g, ' '));
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
    handleSquareClick: (squareElement) => {
        utilities.removeOldClasses();
        let currentSquare = utilities.getCurrentSquare(squareElement);
        let squareAttacks = utilities.getSquareAttacks(currentSquare);
        utilities.highlightSquares(squareAttacks);
    }
};

let gameService = {
    getGameStateInfo: (fen) => {
        (async () => {
            let url = !utilities.isNullOrEmpty(fen) ? constants.urls.stateInfo + fen : constants.urls.stateInfo;
            let response = await fetch(url);
            if (!response.ok) {
                throw Error(response.statusText);
            }
            let gameStateResult = await response.json();
            if (gameStateResult.success) {
                utilities.setBoardState(gameStateResult.result);
            } else {
                // reset the board
                utilities.setBoardState(gameObjects.gameState);
            }
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
            if (gameStateResult.success) {
                utilities.setBoardState(gameStateResult.result);
            } else {
                // reset the board
                console.log(gameStateResult.message);
                utilities.setBoardState(gameObjects.gameState);
            }           
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

document.addEventListener('DOMContentLoaded', (event) => events.init());
