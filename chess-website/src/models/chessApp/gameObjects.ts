import { ChessBoardInstance } from "chessboardjs";

import { gameStateResource } from "../chessEngine/gameStateResource";
import { pawnPromotionInfo } from "./pawnPromotionInfo";

export interface gameObjects {
    board: ChessBoardInstance | null;
    gameState: gameStateResource | null;
    freeze: boolean;
    freezeNotify: number;
    pawnPromotionInfo: pawnPromotionInfo | null;
}