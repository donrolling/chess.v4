let events = {
    init: () => {
        $(constants.selectors.fenSubmit).click(() => utilities.getFenAndUpdate());
        $(constants.selectors.backBtn).click(() => events.onBackClick());
        $(constants.selectors.promotionChoice).click((e) => events.onPawnPromotionChoice(e));
        utilities.getFenAndUpdate();
    },

    onDragStart: (source, piece, position, orientation) => {
        // logging.logDragStart(source, piece, position, orientation);
        // protect against wrong side moves
        if (
            (piece[0] === 'w' && gameObjects.gameState.activeColor === 0)
            || (piece[0] === 'b' && gameObjects.gameState.activeColor === 1)
        ) {
            return false;
        }
        document
            .querySelectorAll(constants.selectors.attacking)
            .forEach(a => utilities.removeClassName(a, constants.classes.attacking));
        document
            .querySelectorAll(constants.selectors.protecting)
            .forEach(a => utilities.removeClassName(a, constants.classes.protecting));
        let squareAttacks = utilities.getSquareAttacks(source);
        utilities.highlightSquares(squareAttacks);
    },

    onDrop: (source, target, piece, newPos, oldPos, orientation) => {
        if (gameObjects.freeze) {
            if (gameObjects.freezeNotify < 2) {
                alert('You are looking at the historic state of the board. New moves are frozen until you go back to the current state using the history panel.');
                gameObjects.freezeNotify++;
            }
            return constants.methodResponses.snapback;
        }

        let squareAttacks = utilities.getSquareAttacks(source);
        if (!squareAttacks.some(x => x.name === target)) {
            return constants.methodResponses.snapback;
        }

        // logging.logDrop(source, target, piece, newPos, oldPos, orientation, null);
        if (
            (piece[0] === 'w' && gameObjects.gameState.activeColor === 1 && piece[1] === 'P' && target[1] == 8)
            || (piece[0] === 'b' && gameObjects.gameState.activeColor === 0 && piece[1] === 'P' && target[1] == 1)
        ) {
            utilities.displayPawnPromotion(source, target);
        } else {
            gameService.move(source, target, null);
        }
    },

    onPGNClick: (e) => {
        let target = e.target;        
        let index = target.getAttribute(constants.attributes.dataIndex);
        gameService.goToMove(parseInt(index));
    },

    onBackClick: () => {
        gameService.goBackOneMove();
    },

    onPawnPromotionChoice: (e) => {   
        let choice = e.target.getAttribute(constants.attributes.dataPiece);
        gameService.move(gameObjects.pawnPromotionMoveInfo.source, gameObjects.pawnPromotionMoveInfo.target, parseInt(choice));
        utilities.hidePawnPromotion();
    }
};