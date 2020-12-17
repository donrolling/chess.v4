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