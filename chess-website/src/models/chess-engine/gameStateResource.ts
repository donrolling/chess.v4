namespace models {
    export interface gameStateResource extends snapshot {
        attacks: attackedSquare[];
        history: snapshot[];
        squares: square[];
        stateInfo: stateInfo;
        fen: string;
    }
}