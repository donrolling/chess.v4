import { square } from "./square";

export interface attackedSquare extends square {
    attackingSquare: square;
    isPassiveAttack: boolean;
    isProtecting: boolean;
    mayOnlyMoveHereIfOccupiedByEnemy: boolean;
}
