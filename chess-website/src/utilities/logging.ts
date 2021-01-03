namespace utilities {
    export class logging {
        public static info(x: any): void { 
            console.log(x);
        }
    
        public static error(x: any): void { 
            console.error(x); 
        }
    
        public static logDragStart(source: any, piece: any, position: any, orientation: any): void {
            console.log({
                Event: constants.ui.events.onDragStart,
                Source: source,
                Piece: piece,
                //Position: Chessboard.objToFen(position),
                Orientation: orientation
            });
        }
    
        public static logDrop(source: any, target: any, piece: any, newPos: any, oldPos: any, orientation: any, squareAttacks: any): void {
            console.log({
                Event: constants.ui.events.onDrop,
                Source: source,
                Target: target,
                Piece: piece,
                NewPosition: newPos,
                //NewFen: Chessboard.objToFen(newPos),
                OldPosition: oldPos,
                //OldFen: Chessboard.objToFen(oldPos),
                Orientation: orientation,
                SquareAttacks: squareAttacks,
            });
        }
    }
}