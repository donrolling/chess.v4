import { color } from "../enums/color";

export interface snapshot {
    activeColor: color;
    castlingAvailability: string;
    enPassantTargetPosition: number;
    enPassantTargetSquare: string;
    fullmoveNumber: number;
    halfmoveClock: number;
    piecePlacement: string;
    pgn: string;
    pgnMoves: string[];
}
