namespace services {
    export class gameStateService {
        constructor(
            private gameObjects: models.gameObjects, 
            private config: models.config,
            private gameService: services.gameService
        ) {}

        public setBoardState(gameState: models.gameStateResource): void {
            let fenInputElement = document.querySelector<HTMLInputElement>(constants.ui.selectors.fenInput);
            if(!fenInputElement) {
                return;
            }
            fenInputElement.value = gameState.fen;
            this.config.position = gameState.fen;
            this.gameObjects.gameState = gameState;
            this.gameObjects.board = new models.Chessboard(constants.ui.classes.chessBoard, this.config);
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
    
            utilities.history.setHistoryPanel(gameState.pgn);
    
            // remove event listeners
            utilities.eventlisteners.removeEventListeners(
                constants.ui.selectors.item, 
                constants.ui.events.click, 
                this.onPGNClick
            );
    
            // add event listeners
            utilities.eventlisteners.addEventListeners(
                constants.ui.selectors.item, 
                constants.ui.events.click, 
                this.onPGNClick
            );
        }        

        public getSquareAttacks(squareName: string): Array<models.attackedSquare> {
            if (!this.gameObjects.gameState) {
                return [];
            }
            return this.gameObjects.gameState.attacks.filter(attack => attack.attackingSquare.name !== squareName);
        }

        public onDragStart(source: string, piece: string, position: string, orientation: string) : void | boolean {
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
                .forEach(a => utilities.dom.removeClassName(a, constants.ui.classes.attacking));
            document
                .querySelectorAll(constants.ui.selectors.protecting)
                .forEach(a => utilities.dom.removeClassName(a, constants.ui.classes.protecting));
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
                utilities.pawnPromotion.displayPawnPromotion(this.gameObjects, source, target);
            } else {
                this.gameService.move(this.gameObjects.gameState, source, target, null);
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
                let squareSelector = utilities.dom.getSquareSelector(attack.name);
                var square = document.querySelector(squareSelector);
                if (square) {                    
                    square.className += ` ${squareClass}`;
                }
            }
        }

        private handleSquareClick(e: Event): void {
            //utilities.dom.removeOldClasses();
            //let currentSquare = utilities.dom.getCurrentSquare(e);
            //let squareAttacks = utilities.getSquareAttacks(currentSquare);
            //utilities.highlightSquares(squareAttacks);
        }
    }
}