import { color } from "../enums/color";
import { pieceType } from "../enums/pieceType";

export interface piece {
    color: color;
    identity: string;
    pieceType: pieceType;
    orderOfOperation: number;
}
