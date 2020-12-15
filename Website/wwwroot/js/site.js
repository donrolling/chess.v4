var constants = {
    attacking: 'attacking',
    chessBoard: 'chessBoard',
    piece: 'piece',
    protecting: 'protecting',
    snapback: 'snapback',
    square: 'square',

    http: {
        post: 'POST',

        contentTypes: {
            applicationjson: 'application/json'
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
        game: 'api/game'
    }
};

var gameObjects = {
    board: {},
    gamestate: {},
    attacks: [],
};

var utilities = {
    getFenAndUpdate: () => {
        var fen = $(constants.selectors.fenInput).val();
        gameService.updateBoardStateViaFen(fen);
    },

    setBoardState: (fen) => {
        config.position = fen;
        gameObjects.board = Chessboard(constants.chessBoard, config);
        if (!config.draggable) {
            $(constants.selectors.allSquares).click(function (e) {
                handlers.handleSquareClick(e);
            })
        }
    },

    getCurrentSquare: (e) => {
        var obj = $(e.target);
        var piece = obj.data(constants.piece);
        if (piece) {
            obj = $(e.target).parent();
        }
        return obj.data(constants.square);
    },

    getSquareAttacks: (square) => {
        var squareAttacks = [];
        for (var i = 0; i < gameObjects.gamestate.attacks.length; i++) {
            var attack = gameObjects.gamestate.attacks[i];
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
        for (var i = 0; i < attacks.length; i++) {
            var attack = attacks[i];
            var squareClass = attack.isProtecting ? constants.protecting : constants.attacking;
            var squareSelector = utilities.getSquareSelector(attack.name);
            $(squareSelector).addClass(squareClass);
        }
    },

    removeOldClasses: () => {
        $(constants.selectors.attacking).removeClass(constants.attacking);
        $(constants.selectors.protecting).removeClass(constants.protecting);
    }
};

var events = {
    init: () => {
        $(constants.selectors.fenSubmit).click(function () {
            utilities.getFenAndUpdate();
        });
        utilities.getFenAndUpdate();
    },

    onDragStart: (source, piece, position, orientation) => {
        logging.logDragStart(source, piece, position, orientation);
        utilities.removeOldClasses();
        var squareAttacks = utilities.getSquareAttacks(source);
        utilities.highlightSquares(squareAttacks);
    },

    onDrop: (source, target, piece, newPos, oldPos, orientation) => {
        var squareAttacks = utilities.getSquareAttacks(source);
        logging.logDrop(source, target, piece, newPos, oldPos, orientation, squareAttacks);
        if (!squareAttacks.some(x => x.name === target)) {
            return constants.snapback;
        }
    }
};

var handlers = {
    handleSquareClick: (e) => {
        utilities.removeOldClasses();
        var currentSquare = utilities.getCurrentSquare(e);
        var squareAttacks = utilities.getSquareAttacks(currentSquare);
        utilities.highlightSquares(squareAttacks);
    }
};

var gameService = {
    updateBoardStateViaFen: (fen) => {
        if (!fen) {
            return;
        }
        logging.log(fen);
        $.ajax({
            type: constants.http.post,
            url: constants.urls.game,
            data: fen,
            contentType: constants.http.contentTypes.applicationjson,
            dataType: constants.http.dataTypes.json
        }).done(function (gamestateResult) {
            logging.logTest();
            gameObjects.gamestate = gamestateResult.result;
            gameObjects.attacks = gameObjects.gamestate.attacks;
            utilities.setBoardState(fen);
        });
    }
};

var logging = {
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

var config = {
    draggable: true,
    dropOffBoard: constants.snapback, // this is the default
    onDragStart: events.onDragStart,
    onDrop: events.onDrop
}

$(document).ready(function () {
    events.init();
});
