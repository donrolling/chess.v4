import { pieceType } from "../enums/pieceType";

export interface stateInfo {
    hasThreefoldRepition: boolean;
    isBlackCheck: boolean;
    isCastle: boolean;
    isCheck: boolean;
    isCheckmate: boolean;
    isDraw: boolean;
    isEnPassant: boolean;
    isPawnPromotion: boolean;
    isWhiteCheck: boolean;
    pawnPromotedTo: pieceType;
    result: string;
}
