let handlers = {
    handleSquareClick: (squareElement) => {
        utilities.removeOldClasses();
        let currentSquare = utilities.getCurrentSquare(squareElement);
        let squareAttacks = utilities.getSquareAttacks(currentSquare);
        utilities.highlightSquares(squareAttacks);
    }
};