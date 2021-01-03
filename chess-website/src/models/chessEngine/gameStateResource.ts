import { attackedSquare } from "./attackedSquare";
import { snapshot } from "./snapshot";
import { square } from "./square";
import { stateInfo } from "./stateInfo";

export interface gameStateResource extends snapshot {
    attacks: attackedSquare[];
    history: snapshot[];
    squares: square[];
    stateInfo: stateInfo;
    fen: string;
} 
