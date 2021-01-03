var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
define(["require", "exports", "chessboardjs", "../constants/chess/methodResponses", "../constants/ui/attributes", "../constants/ui/classes", "../constants/ui/events", "../constants/ui/selectors", "../utilities/dom", "../utilities/historyEngine", "../utilities/listeners", "../utilities/logging", "../utilities/pawnPromotion", "../utilities/strings"], function (require, exports, chessboardjs_1, methodResponses_1, attributes_1, classes_1, events_1, selectors_1, dom_1, historyEngine_1, listeners_1, logging_1, pawnPromotion_1, strings_1) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.gameStateService = void 0;
    class gameStateService {
        constructor(gameObjects, config, gameService) {
            this.gameObjects = gameObjects;
            this.config = config;
            this.gameService = gameService;
        }
        setBoardState(gameState) {
            let fenInputElement = document.querySelector(selectors_1.selectors.fenInput);
            if (!fenInputElement) {
                return;
            }
            fenInputElement.value = gameState.fen;
            this.config.position = gameState.fen;
            this.gameObjects.gameState = gameState;
            this.gameObjects.board = chessboardjs_1.ChessBoard(classes_1.classes.chessBoard, this.config);
            if (!this.config.draggable) {
                document
                    .querySelectorAll(selectors_1.selectors.allSquares)
                    .forEach(a => a.addEventListener(events_1.events.click, e => this.handleSquareClick(e)));
            }
            historyEngine_1.historyEngine.setHistoryPanel(gameState.pgn);
            // remove event listeners
            listeners_1.eventlisteners.removeEventListeners(selectors_1.selectors.item, events_1.events.click, this.onPGNClick);
            // add event listeners
            listeners_1.eventlisteners.addEventListeners(selectors_1.selectors.item, events_1.events.click, this.onPGNClick);
        }
        getSquareAttacks(squareName) {
            if (!this.gameObjects.gameState) {
                return [];
            }
            return this.gameObjects.gameState.attacks.filter(attack => attack.attackingSquare.name !== squareName);
        }
        onDragStart(source, piece, position, orientation) {
            if (!this.gameObjects.gameState) {
                return;
            }
            // logging.logDragStart(source, piece, position, orientation);
            // protect against wrong side moves
            if ((piece[0] === 'w' && this.gameObjects.gameState.activeColor === 0)
                || (piece[0] === 'b' && this.gameObjects.gameState.activeColor === 1)) {
                return false;
            }
            document
                .querySelectorAll(selectors_1.selectors.attacking)
                .forEach(a => dom_1.dom.removeClassName(a, classes_1.classes.attacking));
            document
                .querySelectorAll(selectors_1.selectors.protecting)
                .forEach(a => dom_1.dom.removeClassName(a, classes_1.classes.protecting));
            let squareAttacks = this.getSquareAttacks(source);
            this.highlightSquares(squareAttacks);
        }
        onDrop(source, target, piece, newPos, oldPos, orientation) {
            if (!this.gameObjects.gameState) {
                return;
            }
            if (this.gameObjects.freeze) {
                if (this.gameObjects.freezeNotify < 2) {
                    alert('You are looking at the historic state of the board. New moves are frozen until you go back to the current state using the history panel.');
                    this.gameObjects.freezeNotify++;
                }
                return methodResponses_1.methodResponses.snapback;
            }
            let squareAttacks = this.getSquareAttacks(source);
            if (!squareAttacks.some(x => x.name === target)) {
                return methodResponses_1.methodResponses.snapback;
            }
            // logging.logDrop(source, target, piece, newPos, oldPos, orientation, null);
            let rank = parseInt(target[1]);
            if ((piece[0] === 'w' && this.gameObjects.gameState.activeColor === 1 && piece[1] === 'P' && rank === 8)
                || (piece[0] === 'b' && this.gameObjects.gameState.activeColor === 0 && piece[1] === 'P' && rank === 1)) {
                pawnPromotion_1.pawnPromotion.displayPawnPromotion(this.gameObjects, source, target);
            }
            else {
                (() => __awaiter(this, void 0, void 0, function* () {
                    if (!this.gameObjects.gameState) {
                        return;
                    }
                    let gameStateResult = yield this.gameService.move(this.gameObjects.gameState, source, target, null);
                    if (gameStateResult.success) {
                        this.setBoardState(gameStateResult.result);
                    }
                    else {
                        // reset the board
                        logging_1.logging.error(gameStateResult.message);
                        if (this.gameObjects.gameState) {
                            this.setBoardState(this.gameObjects.gameState);
                        }
                    }
                }))();
            }
        }
        onPGNClick(e) {
            let target = e.target;
            if (!target) {
                return;
            }
            let index = target.getAttribute(attributes_1.attributes.dataIndex);
            if (!index) {
                return;
            }
            let dataIndex = parseInt(index);
            this.gameService.goToMove(this.gameObjects, dataIndex);
        }
        highlightSquares(attacks) {
            for (let i = 0; i < attacks.length; i++) {
                let attack = attacks[i];
                let squareClass = attack.isProtecting ? classes_1.classes.protecting : classes_1.classes.attacking;
                let squareSelector = dom_1.dom.getSquareSelector(attack.name);
                var square = document.querySelector(squareSelector);
                if (square) {
                    square.className += ` ${squareClass}`;
                }
            }
        }
        getAnyFenAndUpdate() {
            return __awaiter(this, void 0, void 0, function* () {
                let fen = dom_1.dom.getParameterByName('fen');
                if (!strings_1.strings.isNullOrEmpty(fen)) {
                    let gameStateResult = yield this.gameService.getGameStateInfo(fen);
                    if (gameStateResult.success) {
                        this.setBoardState(gameStateResult.result);
                    }
                    else {
                        // reset the board
                        if (this.gameObjects.gameState) {
                            this.setBoardState(this.gameObjects.gameState);
                        }
                    }
                }
                else {
                    this.getFenAndUpdate();
                }
            });
        }
        getFenAndUpdate() {
            let fenInput = document.querySelector(selectors_1.selectors.fenInput);
            if (fenInput) {
                this.gameService.getGameStateInfo(fenInput.value);
            }
            // update??
        }
        // this is for non-draggable situations - need to get it working again, but don't really care
        handleSquareClick(e) {
            //dom.removeOldClasses();
            //let currentSquare = dom.getCurrentSquare(e);
            //let squareAttacks = utilities.getSquareAttacks(currentSquare);
            //utilities.highlightSquares(squareAttacks);
        }
    }
    exports.gameStateService = gameStateService;
});
