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
        if (!squareAttacks.some(x => x.name === target)) {
            return constants.snapback;
        }
        // todo: piece promotion selection
        // constants.pieceTypes.Bishop....
        let piecePromotionType = null;
        gameService.move(source, target, piecePromotionType);
    },

    onPGNClick: (e) => {
        let index = e.target.getAttribute('data-index');

    }
};