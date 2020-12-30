let utilities = {
    getFenAndUpdate: () => {
        let fen = utilities.getParameterByName('fen');
        if (utilities.isNullOrEmpty(fen)) {
            fen = document.querySelector(constants.selectors.fenInput).value;
        }
        gameService.getGameStateInfo(fen);
    },

    isNullOrEmpty: (x) => x === undefined || !x,

    getCurrentSquare: (squareElement) => {
        //logging.log(squareElement.target);
        let square = document.querySelector(squareElement.target);
        //logging.log(square);
        let piece = square.data(constants.classes.piece);
        if (piece) {
            square = document.querySelector(squareElement.target).parentElement;
        }
        //logging.log(square);
        return square.data(constants.classes.square);
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
            let squareClass = attack.isProtecting ? constants.classes.protecting : constants.classes.attacking;
            let squareSelector = utilities.getSquareSelector(attack.name);
            var square = document.querySelector(squareSelector);
            square.className += ` ${squareClass}`;
        }
    },

    addEventListeners: (selector, event, handler) => {
        let items = document.querySelectorAll(selector);
        if (items) {
            for (const item of items) {
                item.addEventListener(event, handler);
            }
        }
    },

    removeEventListeners: (selector, event, handler) => {
        let items = document.querySelectorAll(selector);
        if (items) {
            for (const item of items) {
                item.removeEventListener(event, handler);
            }
        }
    },

    getParameterByName: (name, url = window.location.href) => {
        name = name.replace(/[\[\]]/g, '\\$&');
        var regex = new RegExp('[?&]' + name + '(=([^&#]*)|&|#|$)'),
            results = regex.exec(url);
        if (!results) return null;
        if (!results[2]) return '';
        return decodeURIComponent(results[2].replace(/\+/g, ' '));
    },

    setBoardState: (gameState) => {
        document.querySelector(constants.selectors.fenInput).value = gameState.fen;
        config.position = gameState.fen;
        gameObjects.gameState = gameState;
        gameObjects.board = Chessboard(constants.classes.chessBoard, config);
        if (!config.draggable) {
            document
                .querySelectorAll(constants.selectors.allSquares)
                .forEach(a =>
                    a.addEventListener(constants.events.click, e => handlers.handleSquareClick(e))
                );
        }

        utilities.setHistoryPanel(gameState.pgn);
        
        // remove event listeners
        utilities.removeEventListeners(constants.selectors.item, constants.events.click, events.onPGNClick);

        // add event listeners
        utilities.addEventListeners(constants.selectors.item, constants.events.click, events.onPGNClick);
    },

    removeClassName: (element, removeItem) => {
        if (!element.className) {
            element.className = '';
        }
        if (!removeItem) {
            element.className = element.className;
        }
        var classNames = element.className.split(' ');
        utilities.removeItemFromArray(classNames, removeItem);
        element.className = classNames.join(' ');
    },

    addClassName: (element, addItem) => {
        if (!element.className) {
            element.className = addItem;
        }
        element.className = `${element.className} ${addItem}`;
    },

    removeItemFromArray: (xs, x) => {
        let index = xs.indexOf(x);
        if (index > -1) {
            xs.splice(index, 1);
        }
    },

    historyToFEN: (history) => {
        return `${history.piecePlacement} ${history.activeColor} ${history.castlingAvailability} ${history.enPassantTargetPosition} ${history.halfmoveClock} ${history.fullmoveNumber}`;
    },

    setHistoryPanel: (pgn) => {
        logging.info({pgn: pgn});
        if (!pgn) {// modify content
            let itemContainer = document.querySelector(constants.selectors.items);
            itemContainer.innerHTML = '';
            return;
        }
        let pgnItems = pgn.split(' ');
        let contentList = [];
        var pgnIndex = 1;
        for (let i = 0; i < pgnItems.length; i++) {
            let item = pgnItems[i];
            if (item.includes('.')) {
                let template = `<div class="${constants.classes.number}">${item}</div>`;
                contentList.push(template);
            } else {
                let template = i === pgnItems.length - 1
                ? `<div class="${constants.classes.item} ${constants.classes.active}" ${constants.attributes.dataIndex}="${pgnIndex}">${item}</div>`
                : `<div class="${constants.classes.item}" ${constants.attributes.dataIndex}="${pgnIndex}">${item}</div>`;
                contentList.push(template);
                pgnIndex++;
            }
        }

        // modify content
        let itemContainer = document.querySelector(constants.selectors.items);
        itemContainer.innerHTML = contentList.join('');
    },

    selectPgnItemByTarget: (target) => {
        let index = target.getAttribute(constants.attributes.dataIndex);
        let items = document.querySelectorAll(constants.selectors.item);
        items.forEach(element => {
            utilities.removeClassName(element, constants.classes.active);
        });
        utilities.addClassName(target, constants.classes.active);
    },

    selectPgnItemByIndex: (index) => {
        let target = document.querySelector(`.items .item[data-index="${ index }"]`)
        let items = document.querySelectorAll(constants.selectors.item);
        items.forEach(element => {
            utilities.removeClassName(element, constants.classes.active);
        });
        utilities.addClassName(target, constants.classes.active);
    }
};