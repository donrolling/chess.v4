import { piece } from "./piece";

export interface square {
    index: number;
    name: string;
    occupied: boolean;
    piece: piece;
}
