
let gameService = {
    getGameStateInfo: (fen) => {
        (async () => {
            let url = !utilities.isNullOrEmpty(fen) ? constants.urls.stateInfo + fen : constants.urls.stateInfo;
            let response = await fetch(url);
            if (!response.ok) {
                throw Error(response.statusText);
            }
            let gameStateResult = await response.json();
            if (gameStateResult.success) {
                utilities.setBoardState(gameStateResult.result);
            } else {
                // reset the board
                utilities.setBoardState(gameObjects.gameState);
            }
        })();
    },

    move: (beginning, destination, piecePromotionType) => {
        let data = JSON.stringify({
            GameState: gameObjects.gameState,
            Beginning: beginning,
            Destination: destination,
            PiecePromotionType: piecePromotionType
        });
        //logging.log(data);
        (async () => {
            let url = constants.urls.move;
            let response = await fetch(
                url,
                {
                    headers: {
                        'Accept': constants.http.contentTypes.applicationjson,
                        'Content-Type': constants.http.contentTypes.applicationjson
                    },
                    method: constants.http.post,
                    body: data
                }
            );
            if (!response.ok) {
                throw Error(response.statusText);
            }
            let gameStateResult = await response.json();
            if (gameStateResult.success) {
                utilities.setBoardState(gameStateResult.result);
            } else {
                // reset the board
                utilities.setBoardState(gameObjects.gameState);
            }
        })();
    },

    goToMove: (index) => {
        // if this is the most current move, then we're no longer frozen
        if (gameObjects.gameState.history.length === index + 1) {
            gameObjects.freeze = false;
            gameObjects.freezeNotify = 0;
            // reset the board
            utilities.setBoardState(gameObjects.gameState);
            return;
        } else {
            gameObjects.freeze = true;
        }

        // get historical position
        let history = gameObjects.gameState.history[index];
        let fen = utilities.historyToFEN(history);
        // set the input text box
        document.querySelector(constants.selectors.fenInput).value = fen;

        // copy existing config...this isn't a real move, it is a fake move, so copy the existing config
        let newConfig = {
            position: fen,
            draggable: false            
        }
        gameObjects.board = Chessboard(constants.classes.chessBoard, newConfig);
    },

    goBackOneMove: () => {
        if (gameObjects.gameState.fullmoveNumber > 0) {
            gameService.goToMove(gameObjects.gameState.fullmoveNumber - 1);
        }
    }
};