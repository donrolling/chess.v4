var constants = {
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

var gameObjects = {
    board: {},
    gamestate: {}
};

var utilities = {
    getFenAndUpdate: () => {
        var fen = $(constants.selectors.fenInput).val();
        gameService.getGameStateInformation(fen);
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
        // todo: piece promotion selection
        // constants.pieceTypes.Bishop....
        var piecePromotionType = null;
        gameService.move(source, target, piecePromotionType);
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
    getGameStateInformation: (fen) => {
        if (!fen) {
            return;
        }
        $.ajax({
            type: constants.http.get,
            url: constants.urls.stateInfo + fen,
            dataType: constants.http.dataTypes.json
        }).done(function (gamestateResult) {
            logging.log(gamestateResult);
            if (gamestateResult.success) {
                gameObjects.gamestate = gamestateResult.result;
                utilities.setBoardState(gamestateResult.result.fen);
            } else {
                console.log(gamestateResult.message);
            }
        });
    },

    move: (beginning, destination, piecePromotionType) => {
        var data = JSON.stringify({
            GameState: gameObjects.gamestate,
            Beginning: beginning,
            Destination: destination,
            PiecePromotionType: piecePromotionType
        });
        $.ajax({
            type: constants.http.post,
            url: constants.urls.move,
            data: data,
            contentType: constants.http.contentTypes.applicationjson,
            dataType: constants.http.dataTypes.json
        }).done(function (gamestateResult) {
            logging.log(gamestateResult);
            if (gamestateResult.success) {
                gameObjects.gamestate = gamestateResult.result;
                utilities.setBoardState(gamestateResult.result.fen);
            } else {
                console.log(gamestateResult.message);
                utilities.setBoardState(gameObjects.gamestate.fen);
            }
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
    position: '',
    draggable: true,
    dropOffBoard: constants.snapback, // this is the default
    onDragStart: events.onDragStart,
    onDrop: events.onDrop
}

$(document).ready(function () {
    events.init();
});
