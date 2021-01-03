namespace services {
    export class gameService {
        private readonly stateInfoUrl: string = 'api/game/state-info?fen=';
        private readonly moveUrl: string = 'api/game/move';
        private readonly gotoUrl: string = 'api/game/goto';
        
        public getGameStateInfo(fen: string): void {
            let url = !utilities.strings.isNullOrEmpty(fen)
                ? this.stateInfoUrl + fen
                : this.stateInfoUrl;
            (async () => {
                let response = await fetch(url);
                if (!response.ok) {
                    throw Error(response.statusText);
                }
                let gameStateResult = await response.json();
                if (gameStateResult.success) {
                    //utilities.setBoardState(gameStateResult.result);
                } else {
                    // reset the board
                    //utilities.setBoardState(gameObjects.gameState);
                }
            })();
        }

        public move(gameState: models.gameStateResource, beginning: string, destination: string, piecePromotionType: number | null) {
            let data = JSON.stringify({
                GameState: gameState,
                Beginning: beginning,
                Destination: destination,
                PiecePromotionType: piecePromotionType
            });
            utilities.logging.info(data);
            (async () => {
                let response = await fetch(
                    this.moveUrl,
                    {
                        headers: {
                            'Accept': constants.http.contentTypes.applicationjson,
                            'Content-Type': constants.http.contentTypes.applicationjson
                        },
                        method: constants.http.httpMethods.post,
                        body: data
                    }
                );
                if (!response.ok) {
                    throw Error(response.statusText);
                }
                let gameStateResult = await response.json();
                if (gameStateResult.success) {
                    //utilities.setBoardState(gameStateResult.result);
                } else {
                    // reset the board
                    utilities.logging.error(gameStateResult.message);
                    //utilities.setBoardState(gameObjects.gameState);
                }
            })();
        }

        public goToMove(gameObjects: models.gameObjects, index: any) {
            if (!gameObjects.gameState) {
                return;
            }
            // if this is the most current move, then we're no longer frozen
            if (gameObjects.gameState.history.length === index) {
                gameObjects.freeze = false;
                gameObjects.freezeNotify = 0;
                // reset the board
                //utilities.setBoardState(gameObjects.gameState);
                return;
            } else {
                gameObjects.freeze = true;
            }

            // get historical position
            let history = gameObjects.gameState.history[index];
            let fen = utilities.history.historyToFEN(history);
            // set the input text box
            //document.querySelector<HTMLElement>(constants.ui.selectors.fenInput).value = fen;

            // copy existing config...this isn't a real move, it is a fake move, so copy the existing config
            let newConfig = {
                position: fen,
                draggable: false
            }
            //gameObjects.board = Chessboard(constants.ui.classes.chessBoard, newConfig);

            utilities.history.selectPgnItemByIndex(index);
        }

        public goBackOneMove(gameObjects: models.gameObjects) {
            if (!gameObjects.gameState) {
                return;
            }
            let newIndex = 0; // beginning index
            if (gameObjects.gameState.fullmoveNumber > 1) {
                var activePGNItem = document.querySelector<HTMLElement>(constants.ui.selectors.activeItem);
                if (!activePGNItem) {
                    return;
                }
                let dataIndex = activePGNItem.getAttribute(constants.ui.attributes.dataIndex);
                if (!dataIndex) {
                    return;
                }
                let index = parseInt(dataIndex);
                newIndex = index - 1;
            }
            let data = JSON.stringify({
                GameState: gameObjects.gameState,
                HistoryIndex: newIndex
            });
            (async () => {
                let response = await fetch(
                    this.gotoUrl,
                    {
                        headers: {
                            'Accept': constants.http.contentTypes.applicationjson,
                            'Content-Type': constants.http.contentTypes.applicationjson
                        },
                        method: constants.http.httpMethods.post,
                        body: data
                    }
                );
                if (!response.ok) {
                    throw Error(response.statusText);
                }
                let gameStateResult = await response.json();
                if (gameStateResult.success) {
                    //utilities.setBoardState(gameStateResult.result);
                } else {
                    // reset the board
                    //utilities.setBoardState(gameObjects.gameState);
                }
            })();
        }

        public getAnyFenAndUpdate(): void {
            let fen = utilities.dom.getParameterByName('fen');
            if (!utilities.strings.isNullOrEmpty(fen)) {
                this.getGameStateInfo(fen);
            } else {
                this.getFenAndUpdate();
            }
        }

        public getFenAndUpdate(): void {            
            let fenInput = document.querySelector<HTMLInputElement>(constants.ui.selectors.fenInput);
            if(fenInput){
                this.getGameStateInfo(fenInput.value);
            }
        }
    }
}