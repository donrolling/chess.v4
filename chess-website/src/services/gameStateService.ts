import { attackedSquare } from "../models/chessEngine/attackedSquare";
import { dom } from "../utilities/dom";
import { pawnPromotion } from "../utilities/pawnPromotion";
import { gameObjects } from "../models/chessApp/gameObjects";
import { gameStateResource } from "../models/chessEngine/gameStateResource";
import { gameService } from "./gameService";
import { historyEngine } from "../utilities/historyEngine";
import { eventlisteners } from "../utilities/listeners";
import { BoardConfig } from "chessboardjs";
import { ChessBoard } from "chessboardjs";
import { strings } from "../utilities/strings";
import { logging } from "../utilities/logging";

export class gameStateService {
    constructor(
        private gameObjects: gameObjects,
        private config: BoardConfig,
        private gameService: gameService
    ) { }

    public setBoardState(gameState: gameStateResource): void {
        let fenInputElement = document.querySelector<HTMLInputElement>(constants.ui.selectors.fenInput);
        if (!fenInputElement) {
            return;
        }
        fenInputElement.value = gameState.fen;
        this.config.position = gameState.fen;
        this.gameObjects.gameState = gameState;
        this.gameObjects.board = ChessBoard(constants.ui.classes.chessBoard, this.config);
        if (!this.config.draggable) {
            document
                .querySelectorAll(constants.ui.selectors.allSquares)
                .forEach(a =>
                    a.addEventListener(
                        constants.ui.events.click,
                        e => this.handleSquareClick(e)
                    )
                );
        }

        historyEngine.setHistoryPanel(gameState.pgn);

        // remove event listeners
        eventlisteners.removeEventListeners(
            constants.ui.selectors.item,
            constants.ui.events.click,
            this.onPGNClick
        );

        // add event listeners
        eventlisteners.addEventListeners(
            constants.ui.selectors.item,
            constants.ui.events.click,
            this.onPGNClick
        );
    }

    public getSquareAttacks(squareName: string): Array<attackedSquare> {
        if (!this.gameObjects.gameState) {
            return [];
        }
        return this.gameObjects.gameState.attacks.filter(attack => attack.attackingSquare.name !== squareName);
    }

    public onDragStart(source: string, piece: string, position: string, orientation: string): void | boolean {
        if (!this.gameObjects.gameState) {
            return;
        }
        // logging.logDragStart(source, piece, position, orientation);
        // protect against wrong side moves
        if (
            (piece[0] === 'w' && this.gameObjects.gameState.activeColor === 0)
            || (piece[0] === 'b' && this.gameObjects.gameState.activeColor === 1)
        ) {
            return false;
        }
        document
            .querySelectorAll(constants.ui.selectors.attacking)
            .forEach(a => dom.removeClassName(a, constants.ui.classes.attacking));
        document
            .querySelectorAll(constants.ui.selectors.protecting)
            .forEach(a => dom.removeClassName(a, constants.ui.classes.protecting));
        let squareAttacks = this.getSquareAttacks(source);
        this.highlightSquares(squareAttacks);
    }

    public onDrop(source: string, target: string, piece: string, newPos: string, oldPos: string, orientation: string): string | void {
        if (!this.gameObjects.gameState) {
            return;
        }

        if (this.gameObjects.freeze) {
            if (this.gameObjects.freezeNotify < 2) {
                alert('You are looking at the historic state of the board. New moves are frozen until you go back to the current state using the history panel.');
                this.gameObjects.freezeNotify++;
            }
            return constants.chess.methodResponses.snapback;
        }

        let squareAttacks = this.getSquareAttacks(source);
        if (!squareAttacks.some(x => x.name === target)) {
            return constants.chess.methodResponses.snapback;
        }

        // logging.logDrop(source, target, piece, newPos, oldPos, orientation, null);
        let rank = parseInt(target[1]);
        if (
            (piece[0] === 'w' && this.gameObjects.gameState.activeColor === 1 && piece[1] === 'P' && rank === 8)
            || (piece[0] === 'b' && this.gameObjects.gameState.activeColor === 0 && piece[1] === 'P' && rank === 1)
        ) {
            pawnPromotion.displayPawnPromotion(this.gameObjects, source, target);
        } else {
            (async () => {
                if (!this.gameObjects.gameState) { return; }

                let gameStateResult = await this.gameService.move(this.gameObjects.gameState, source, target, null);
                if (gameStateResult.success) {
                    this.setBoardState(gameStateResult.result);
                } else {
                    // reset the board
                    logging.error(gameStateResult.message);
                    if (this.gameObjects.gameState) {
                        this.setBoardState(this.gameObjects.gameState);
                    }
                }
            })();
        }
    }

    public onPGNClick(e: Event): void {
        let target = e.target as HTMLElement;
        if (!target) {
            return;
        }
        let index = target.getAttribute(constants.ui.attributes.dataIndex);
        if (!index) {
            return;
        }
        let dataIndex = parseInt(index);
        this.gameService.goToMove(this.gameObjects, dataIndex);
    }

    private highlightSquares(attacks: Array<any>): void {
        for (let i = 0; i < attacks.length; i++) {
            let attack = attacks[i];
            let squareClass = attack.isProtecting ? constants.ui.classes.protecting : constants.ui.classes.attacking;
            let squareSelector = dom.getSquareSelector(attack.name);
            var square = document.querySelector(squareSelector);
            if (square) {
                square.className += ` ${squareClass}`;
            }
        }
    }

    public async getAnyFenAndUpdate(): Promise<void> {
        let fen = dom.getParameterByName('fen');
        if (!strings.isNullOrEmpty(fen)) {
            let gameStateResult = await this.gameService.getGameStateInfo(fen);
            if (gameStateResult.success) {
                this.setBoardState(gameStateResult.result);
            } else {
                // reset the board
                if (this.gameObjects.gameState) {
                    this.setBoardState(this.gameObjects.gameState);
                }
            }
        } else {
            this.getFenAndUpdate();
        }
    }

    public getFenAndUpdate(): void {
        let fenInput = document.querySelector<HTMLInputElement>(constants.ui.selectors.fenInput);
        if (fenInput) {
            this.gameService.getGameStateInfo(fenInput.value);
        }
        // update??
    }

    // this is for non-draggable situations - need to get it working again, but don't really care
    private handleSquareClick(e: Event): void {
        //dom.removeOldClasses();
        //let currentSquare = dom.getCurrentSquare(e);
        //let squareAttacks = utilities.getSquareAttacks(currentSquare);
        //utilities.highlightSquares(squareAttacks);
    }
}