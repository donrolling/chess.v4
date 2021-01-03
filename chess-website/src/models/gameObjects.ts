namespace models {
    export interface gameObjects {
        board: Chessboard | null;
        gameState: gameStateResource | null;
        freeze: boolean;
        freezeNotify: number;
        pawnPromotionInfo: pawnPromotionInfo | null;
    }
}