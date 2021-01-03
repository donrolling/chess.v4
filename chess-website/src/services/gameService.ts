import { gameObjects } from "../models/chessApp/gameObjects";
import { gameStateResource } from "../models/chessEngine/gameStateResource";
import { operationResult } from "../models/common/operationResult";
import { dom } from "../utilities/dom";
import { historyEngine } from "../utilities/historyEngine";
import { logging } from "../utilities/logging";
import { strings } from "../utilities/strings";

export class gameService {
    private readonly stateInfoUrl: string = 'api/game/state-info?fen=';
    private readonly moveUrl: string = 'api/game/move';
    private readonly gotoUrl: string = 'api/game/goto';

    public async getGameStateInfo(fen: string)
        : Promise<operationResult<gameStateResource>> {
        let url = !strings.isNullOrEmpty(fen)
            ? this.stateInfoUrl + fen
            : this.stateInfoUrl;
        let response = await fetch(url);
        if (!response.ok) {
            throw Error(response.statusText);
        }
        let json = await response.json();
        return json as operationResult<gameStateResource>;
    }

    public async move(
        gameState: gameStateResource,
        beginning: string,
        destination: string,
        piecePromotionType: number | null
    ): Promise<operationResult<gameStateResource>> {
        let data = JSON.stringify({
            GameState: gameState,
            Beginning: beginning,
            Destination: destination,
            PiecePromotionType: piecePromotionType
        });
        logging.info(data);
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
        let json = await response.json();
        return json as operationResult<gameStateResource>;
    }

    public goToMove(gameObjects: gameObjects, index: any) {
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
        let fen = historyEngine.historyToFEN(history);
        // set the input text box
        //document.querySelector<HTMLElement>(constants.ui.selectors.fenInput).value = fen;

        // copy existing config...this isn't a real move, it is a fake move, so copy the existing config
        let newConfig = {
            position: fen,
            draggable: false
        }
        //gameObjects.board = Chessboard(constants.ui.classes.chessBoard, newConfig);

        historyEngine.selectPgnItemByIndex(index);
    }

    public async goBackOneMove(gameObjects: gameObjects)
        : Promise<operationResult<gameStateResource>> {
        if (!gameObjects.gameState) {
            return Promise.reject(new Error('gamestate was null'));
        }
        let newIndex = 0; // beginning index
        if (gameObjects.gameState.fullmoveNumber > 1) {
            var activePGNItem = document.querySelector<HTMLElement>(constants.ui.selectors.activeItem);
            if (!activePGNItem) {
                return Promise.reject(new Error('activePGNItem was null'));
            }
            let dataIndex = activePGNItem.getAttribute(constants.ui.attributes.dataIndex);
            if (!dataIndex) {
                return Promise.reject(new Error('dataIndex was null'));
            }
            let index = parseInt(dataIndex);
            newIndex = index - 1;
        }
        let data = JSON.stringify({
            GameState: gameObjects.gameState,
            HistoryIndex: newIndex
        });
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
        let json = await response.json();
        return json as operationResult<gameStateResource>;
    }
}