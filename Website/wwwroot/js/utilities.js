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
        if (!gameObjects.gameState.history || gameObjects.gameState.history.length === 0) {
            return;
        }
        let itemContainer = document.querySelector(constants.selectors.items);
        logging.log(gameObjects.gameState.history);
        let contentList = gameObjects.gameState
            .history
            .map(a => {
                let activeColor = a.activeColor === 0 ? 'w' : 'b'
                return `${a.piecePlacement} ${activeColor} ${a.castlingAvailability} ${a.enPassantTargetSquare} ${a.halfmoveClock} ${a.fullmoveNumber}`;
            })
            .join('</div><div class="item">');
        let content = `<div class="item">${contentList}</div><div class="item">${gameState.fen}</div>`;
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