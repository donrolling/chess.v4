
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
                console.log(gameStateResult.message);
                utilities.setBoardState(gameObjects.gameState);
            }
        })();
    },

    goToMove: (index) => {
        // if this is the most current move, then we're no longer frozen
        if (gameObjects.gameState.history.length === index + 1) {
            gameObjects.freeze = false;
            gameObjects.freezeNotify = 0;
        } else {
            gameObjects.freeze = true;
        }
        let history = gameObjects.gameState.history[index];
        document.querySelector(constants.selectors.fenInput).value = utilities.historyToFEN(history);
        gameObjects.board = Chessboard(constants.classes.chessBoard, config);
    },

    goBackOneMove: () => {
        gameService.goToMove(gameObjects.gameState.fullmoveNumber - 1);
    }
};