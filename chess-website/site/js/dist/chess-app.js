"use strict";
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
define("models/enums/color", ["require", "exports"], function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.color = void 0;
    var color;
    (function (color) {
        color[color["black"] = 0] = "black";
        color[color["white"] = 1] = "white";
    })(color = exports.color || (exports.color = {}));
});
define("models/enums/pieceType", ["require", "exports"], function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.pieceType = void 0;
    var pieceType;
    (function (pieceType) {
        pieceType[pieceType["Pawn"] = 0] = "Pawn";
        pieceType[pieceType["Knight"] = 1] = "Knight";
        pieceType[pieceType["Bishop"] = 2] = "Bishop";
        pieceType[pieceType["Rook"] = 3] = "Rook";
        pieceType[pieceType["Queen"] = 4] = "Queen";
        pieceType[pieceType["King"] = 5] = "King";
    })(pieceType = exports.pieceType || (exports.pieceType = {}));
});
define("models/chessEngine/piece", ["require", "exports"], function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
});
define("models/chessEngine/square", ["require", "exports"], function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
});
define("models/chessEngine/attackedSquare", ["require", "exports"], function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
});
define("models/chessEngine/snapshot", ["require", "exports"], function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
});
define("models/chessEngine/stateInfo", ["require", "exports"], function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
});
define("models/chessEngine/gameStateResource", ["require", "exports"], function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
});
define("models/chessApp/pawnPromotionInfo", ["require", "exports"], function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
});
define("models/chessApp/gameObjects", ["require", "exports"], function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
});
define("models/enums/status", ["require", "exports"], function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.status = void 0;
    var status;
    (function (status) {
        status[status["Success"] = 0] = "Success";
        status[status["Failure"] = 1] = "Failure";
        status[status["ItemNotFound"] = 2] = "ItemNotFound";
        status[status["Cancelled"] = 3] = "Cancelled";
        status[status["Aborted"] = 4] = "Aborted";
        status[status["Expired"] = 5] = "Expired";
    })(status = exports.status || (exports.status = {}));
});
define("models/common/operationResult", ["require", "exports"], function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
});
define("utilities/arrays", ["require", "exports"], function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.arrays = void 0;
    class arrays {
        static removeItemFromArray(xs, x) {
            let index = xs.indexOf(x);
            if (index > -1) {
                xs.splice(index, 1);
            }
        }
    }
    exports.arrays = arrays;
});
define("utilities/dom", ["require", "exports", "utilities/arrays"], function (require, exports, arrays_1) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.dom = void 0;
    class dom {
        static removeClassName(element, removeItem) {
            if (!element.className) {
                element.className = '';
            }
            if (!removeItem) {
                element.className = element.className;
            }
            var classNames = element.className.split(' ');
            arrays_1.arrays.removeItemFromArray(classNames, removeItem);
            element.className = classNames.join(' ');
        }
        static addClassName(element, addItem) {
            if (!element.className) {
                element.className = addItem;
            }
            element.className = `${element.className} ${addItem}`;
        }
        static getSquareSelector(name) {
            return constants.ui.selectors.square + name;
        }
        static getParameterByName(name) {
            let url = window.location.href;
            name = name.replace(/[\[\]]/g, '\\$&');
            let regex = new RegExp('[?&]' + name + '(=([^&#]*)|&|#|$)'), results = regex.exec(url);
            if (!results)
                return '';
            if (!results[2])
                return '';
            return decodeURIComponent(results[2].replace(/\+/g, ' '));
        }
    }
    exports.dom = dom;
});
define("utilities/logging", ["require", "exports"], function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.logging = void 0;
    class logging {
        static info(x) {
            console.log(x);
        }
        static error(x) {
            console.error(x);
        }
        static logDragStart(source, piece, position, orientation) {
            console.log({
                Event: constants.ui.events.onDragStart,
                Source: source,
                Piece: piece,
                //Position: Chessboard.objToFen(position),
                Orientation: orientation
            });
        }
        static logDrop(source, target, piece, newPos, oldPos, orientation, squareAttacks) {
            console.log({
                Event: constants.ui.events.onDrop,
                Source: source,
                Target: target,
                Piece: piece,
                NewPosition: newPos,
                //NewFen: Chessboard.objToFen(newPos),
                OldPosition: oldPos,
                //OldFen: Chessboard.objToFen(oldPos),
                Orientation: orientation,
                SquareAttacks: squareAttacks,
            });
        }
    }
    exports.logging = logging;
});
define("utilities/historyEngine", ["require", "exports", "utilities/dom", "utilities/logging"], function (require, exports, dom_1, logging_1) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.historyEngine = void 0;
    class historyEngine {
        static historyToFEN(history) {
            return `${history.piecePlacement} ${history.activeColor} ${history.castlingAvailability} ${history.enPassantTargetPosition} ${history.halfmoveClock} ${history.fullmoveNumber}`;
        }
        static setHistoryPanel(pgn) {
            logging_1.logging.info({ pgn: pgn });
            if (!pgn) { // modify content
                let itemContainer = document.querySelector(constants.ui.selectors.items);
                if (itemContainer) {
                    itemContainer.innerHTML = '';
                }
                return;
            }
            let pgnItems = pgn.split(' ');
            let contentList = [];
            var pgnIndex = 1;
            for (let i = 0; i < pgnItems.length; i++) {
                let item = pgnItems[i];
                if (item.includes('.')) {
                    let template = `<div class="${constants.ui.classes.number}">${item}</div>`;
                    contentList.push(template);
                }
                else {
                    let template = i === pgnItems.length - 1
                        ? `<div class="${constants.ui.classes.item} ${constants.ui.classes.active}" ${constants.ui.attributes.dataIndex}="${pgnIndex}">${item}</div>`
                        : `<div class="${constants.ui.classes.item}" ${constants.ui.attributes.dataIndex}="${pgnIndex}">${item}</div>`;
                    contentList.push(template);
                    pgnIndex++;
                }
            }
            // modify content
            let itemContainer = document.querySelector(constants.ui.selectors.items);
            if (itemContainer) {
                itemContainer.innerHTML = contentList.join('');
            }
        }
        static selectPgnItemByTarget(target) {
            let index = target.getAttribute(constants.ui.attributes.dataIndex);
            let items = document.querySelectorAll(constants.ui.selectors.item);
            items.forEach(element => {
                dom_1.dom.removeClassName(element, constants.ui.classes.active);
            });
            dom_1.dom.addClassName(target, constants.ui.classes.active);
        }
        static selectPgnItemByIndex(index) {
            let target = document.querySelector(`.items .item[data-index="${index}"]`);
            let items = document.querySelectorAll(constants.ui.selectors.item);
            items.forEach(element => {
                dom_1.dom.removeClassName(element, constants.ui.classes.active);
            });
            dom_1.dom.addClassName(target, constants.ui.classes.active);
        }
    }
    exports.historyEngine = historyEngine;
});
define("utilities/strings", ["require", "exports"], function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.strings = void 0;
    class strings {
        static isNullOrEmpty(x) {
            return x === undefined || !x;
        }
    }
    exports.strings = strings;
});
define("services/gameService", ["require", "exports", "utilities/historyEngine", "utilities/logging", "utilities/strings"], function (require, exports, historyEngine_1, logging_2, strings_1) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.gameService = void 0;
    class gameService {
        constructor() {
            this.stateInfoUrl = 'api/game/state-info?fen=';
            this.moveUrl = 'api/game/move';
            this.gotoUrl = 'api/game/goto';
        }
        getGameStateInfo(fen) {
            return __awaiter(this, void 0, void 0, function* () {
                let url = !strings_1.strings.isNullOrEmpty(fen)
                    ? this.stateInfoUrl + fen
                    : this.stateInfoUrl;
                let response = yield fetch(url);
                if (!response.ok) {
                    throw Error(response.statusText);
                }
                let json = yield response.json();
                return json;
            });
        }
        move(gameState, beginning, destination, piecePromotionType) {
            return __awaiter(this, void 0, void 0, function* () {
                let data = JSON.stringify({
                    GameState: gameState,
                    Beginning: beginning,
                    Destination: destination,
                    PiecePromotionType: piecePromotionType
                });
                logging_2.logging.info(data);
                let response = yield fetch(this.moveUrl, {
                    headers: {
                        'Accept': constants.http.contentTypes.applicationjson,
                        'Content-Type': constants.http.contentTypes.applicationjson
                    },
                    method: constants.http.httpMethods.post,
                    body: data
                });
                if (!response.ok) {
                    throw Error(response.statusText);
                }
                let json = yield response.json();
                return json;
            });
        }
        goToMove(gameObjects, index) {
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
            }
            else {
                gameObjects.freeze = true;
            }
            // get historical position
            let history = gameObjects.gameState.history[index];
            let fen = historyEngine_1.historyEngine.historyToFEN(history);
            // set the input text box
            //document.querySelector<HTMLElement>(constants.ui.selectors.fenInput).value = fen;
            // copy existing config...this isn't a real move, it is a fake move, so copy the existing config
            let newConfig = {
                position: fen,
                draggable: false
            };
            //gameObjects.board = Chessboard(constants.ui.classes.chessBoard, newConfig);
            historyEngine_1.historyEngine.selectPgnItemByIndex(index);
        }
        goBackOneMove(gameObjects) {
            return __awaiter(this, void 0, void 0, function* () {
                if (!gameObjects.gameState) {
                    return Promise.reject(new Error('gamestate was null'));
                }
                let newIndex = 0; // beginning index
                if (gameObjects.gameState.fullmoveNumber > 1) {
                    var activePGNItem = document.querySelector(constants.ui.selectors.activeItem);
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
                let response = yield fetch(this.gotoUrl, {
                    headers: {
                        'Accept': constants.http.contentTypes.applicationjson,
                        'Content-Type': constants.http.contentTypes.applicationjson
                    },
                    method: constants.http.httpMethods.post,
                    body: data
                });
                if (!response.ok) {
                    throw Error(response.statusText);
                }
                let json = yield response.json();
                return json;
            });
        }
    }
    exports.gameService = gameService;
});
define("utilities/pawnPromotion", ["require", "exports", "utilities/dom"], function (require, exports, dom_2) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.pawnPromotion = void 0;
    class pawnPromotion {
        static displayPawnPromotion(gameObjects, source, target) {
            gameObjects.pawnPromotionInfo = {
                source: source,
                target: target
            };
            gameObjects.freeze = true;
            let ppui = document.querySelector(constants.ui.selectors.pawnPromotion);
            dom_2.dom.removeClassName(ppui, constants.ui.classes.hidden);
        }
        static hidePawnPromotion(gameObjects) {
            gameObjects.pawnPromotionInfo = null;
            gameObjects.freeze = false;
            let ppui = document.querySelector(constants.ui.selectors.pawnPromotion);
            dom_2.dom.addClassName(ppui, constants.ui.classes.hidden);
        }
    }
    exports.pawnPromotion = pawnPromotion;
});
define("utilities/listeners", ["require", "exports"], function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.eventlisteners = void 0;
    class eventlisteners {
        static addEventListeners(selector, event, handler) {
            let items = document.querySelectorAll(selector);
            if (items) {
                for (const item of items) {
                    item.addEventListener(event, handler);
                }
            }
        }
        static removeEventListeners(selector, event, handler) {
            let items = document.querySelectorAll(selector);
            if (items) {
                for (const item of items) {
                    item.removeEventListener(event, handler);
                }
            }
        }
    }
    exports.eventlisteners = eventlisteners;
});
define("services/gameStateService", ["require", "exports", "utilities/dom", "utilities/pawnPromotion", "utilities/historyEngine", "utilities/listeners", "chessboardjs", "utilities/strings", "utilities/logging"], function (require, exports, dom_3, pawnPromotion_1, historyEngine_2, listeners_1, chessboardjs_1, strings_2, logging_3) {
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
            let fenInputElement = document.querySelector(constants.ui.selectors.fenInput);
            if (!fenInputElement) {
                return;
            }
            fenInputElement.value = gameState.fen;
            this.config.position = gameState.fen;
            this.gameObjects.gameState = gameState;
            this.gameObjects.board = chessboardjs_1.ChessBoard(constants.ui.classes.chessBoard, this.config);
            if (!this.config.draggable) {
                document
                    .querySelectorAll(constants.ui.selectors.allSquares)
                    .forEach(a => a.addEventListener(constants.ui.events.click, e => this.handleSquareClick(e)));
            }
            historyEngine_2.historyEngine.setHistoryPanel(gameState.pgn);
            // remove event listeners
            listeners_1.eventlisteners.removeEventListeners(constants.ui.selectors.item, constants.ui.events.click, this.onPGNClick);
            // add event listeners
            listeners_1.eventlisteners.addEventListeners(constants.ui.selectors.item, constants.ui.events.click, this.onPGNClick);
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
                .querySelectorAll(constants.ui.selectors.attacking)
                .forEach(a => dom_3.dom.removeClassName(a, constants.ui.classes.attacking));
            document
                .querySelectorAll(constants.ui.selectors.protecting)
                .forEach(a => dom_3.dom.removeClassName(a, constants.ui.classes.protecting));
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
                return constants.chess.methodResponses.snapback;
            }
            let squareAttacks = this.getSquareAttacks(source);
            if (!squareAttacks.some(x => x.name === target)) {
                return constants.chess.methodResponses.snapback;
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
                        logging_3.logging.error(gameStateResult.message);
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
            let index = target.getAttribute(constants.ui.attributes.dataIndex);
            if (!index) {
                return;
            }
            let dataIndex = parseInt(index);
            this.gameService.goToMove(this.gameObjects, dataIndex);
        }
        highlightSquares(attacks) {
            for (let i = 0; i < attacks.length; i++) {
                let attack = attacks[i];
                let squareClass = attack.isProtecting ? constants.ui.classes.protecting : constants.ui.classes.attacking;
                let squareSelector = dom_3.dom.getSquareSelector(attack.name);
                var square = document.querySelector(squareSelector);
                if (square) {
                    square.className += ` ${squareClass}`;
                }
            }
        }
        getAnyFenAndUpdate() {
            return __awaiter(this, void 0, void 0, function* () {
                let fen = dom_3.dom.getParameterByName('fen');
                if (!strings_2.strings.isNullOrEmpty(fen)) {
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
            let fenInput = document.querySelector(constants.ui.selectors.fenInput);
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
define("app", ["require", "exports", "services/gameService", "services/gameStateService", "utilities/pawnPromotion"], function (require, exports, gameService_1, gameStateService_1, pawnPromotion_2) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.app = void 0;
    class app {
        constructor() {
            var _a, _b;
            console.log('App CTOR');
            let gameObjects = {
                board: null,
                gameState: null,
                pawnPromotionInfo: null,
                freeze: false,
                freezeNotify: 0
            };
            let config = {
                position: '',
                draggable: true
            };
            let _gameService = new gameService_1.gameService();
            let _gameStateService = new gameStateService_1.gameStateService(gameObjects, config, _gameService);
            (_a = document
                .querySelector(constants.ui.selectors.fenSubmit)) === null || _a === void 0 ? void 0 : _a.addEventListener(constants.ui.events.click, () => _gameStateService.getFenAndUpdate());
            (_b = document
                .querySelector(constants.ui.selectors.backBtn)) === null || _b === void 0 ? void 0 : _b.addEventListener(constants.ui.events.click, () => __awaiter(this, void 0, void 0, function* () {
                let response = yield _gameService.goBackOneMove(gameObjects);
                if (response.success) {
                    _gameStateService.setBoardState(response.result);
                }
                else {
                    // reset the board
                    if (gameObjects.gameState) {
                        _gameStateService.setBoardState(gameObjects.gameState);
                    }
                }
            }));
            let promotionChoiceElement = document.querySelector(constants.ui.selectors.promotionChoice);
            if (promotionChoiceElement) {
                promotionChoiceElement.addEventListener(constants.ui.events.click, (e) => {
                    let element = e.target;
                    let choice = element.getAttribute(constants.ui.attributes.dataPiece);
                    if (!choice) {
                        return;
                    }
                    let promotionPieceType = parseInt(choice);
                    if (!promotionPieceType || !gameObjects.pawnPromotionInfo || !gameObjects.gameState) {
                        return;
                    }
                    _gameService.move(gameObjects.gameState, gameObjects.pawnPromotionInfo.source, gameObjects.pawnPromotionInfo.target, promotionPieceType);
                    pawnPromotion_2.pawnPromotion.hidePawnPromotion(gameObjects);
                });
            }
            // start using any existing fen source, preferring the querystring
            _gameStateService.getAnyFenAndUpdate();
        }
    }
    exports.app = app;
});
var constants;
(function (constants) {
    var chess;
    (function (chess) {
        class methodResponses {
        }
        methodResponses.snapback = "snapback";
        chess.methodResponses = methodResponses;
    })(chess = constants.chess || (constants.chess = {}));
})(constants || (constants = {}));
var constants;
(function (constants) {
    var chess;
    (function (chess) {
        let pieceTypes;
        (function (pieceTypes) {
            pieceTypes[pieceTypes["pawn"] = 0] = "pawn";
            pieceTypes[pieceTypes["knight"] = 1] = "knight";
            pieceTypes[pieceTypes["bishop"] = 2] = "bishop";
            pieceTypes[pieceTypes["rook"] = 3] = "rook";
            pieceTypes[pieceTypes["queen"] = 4] = "queen";
            pieceTypes[pieceTypes["king"] = 5] = "king";
        })(pieceTypes = chess.pieceTypes || (chess.pieceTypes = {}));
    })(chess = constants.chess || (constants.chess = {}));
})(constants || (constants = {}));
var constants;
(function (constants) {
    var http;
    (function (http) {
        class contentTypes {
        }
        contentTypes.applicationjson = 'application/json';
        contentTypes.text = 'text';
        http.contentTypes = contentTypes;
    })(http = constants.http || (constants.http = {}));
})(constants || (constants = {}));
var constants;
(function (constants) {
    var http;
    (function (http) {
        class dataTypes {
        }
        dataTypes.json = 'json';
        http.dataTypes = dataTypes;
    })(http = constants.http || (constants.http = {}));
})(constants || (constants = {}));
var constants;
(function (constants) {
    var http;
    (function (http) {
        class httpMethods {
        }
        httpMethods.post = 'POST';
        httpMethods.get = 'GET';
        http.httpMethods = httpMethods;
    })(http = constants.http || (constants.http = {}));
})(constants || (constants = {}));
var constants;
(function (constants) {
    var ui;
    (function (ui) {
        class attributes {
        }
        attributes.dataIndex = 'data-index';
        attributes.dataPiece = 'data-piece';
        ui.attributes = attributes;
    })(ui = constants.ui || (constants.ui = {}));
})(constants || (constants = {}));
var constants;
(function (constants) {
    var ui;
    (function (ui) {
        class classes {
        }
        classes.active = "active";
        classes.items = "items";
        classes.item = "item";
        classes.number = "number";
        classes.attacking = "attacking";
        classes.chessBoard = "chessBoard";
        classes.piece = "piece";
        classes.protecting = "protecting";
        classes.square = "square";
        classes.pawnPromotion = "pawnPromotion";
        classes.hidden = "hidden";
        ui.classes = classes;
    })(ui = constants.ui || (constants.ui = {}));
})(constants || (constants = {}));
var constants;
(function (constants) {
    var ui;
    (function (ui) {
        class events {
        }
        events.onDragStart = "onDragStart";
        events.onDrop = "onDrop";
        events.click = "click";
        ui.events = events;
    })(ui = constants.ui || (constants.ui = {}));
})(constants || (constants = {}));
var constants;
(function (constants) {
    var ui;
    (function (ui) {
        class selectors {
        }
        selectors.allSquares = '.square-55d63';
        selectors.fenSubmit = '#fenSubmit';
        selectors.fenInput = '.fen';
        selectors.attacking = '.attacking';
        selectors.protecting = '.protecting';
        selectors.square = '.square-';
        selectors.history = '.history';
        selectors.items = '.items';
        selectors.item = '.item';
        selectors.activeItem = '.item.active';
        selectors.backBtn = '#backBtn';
        selectors.pawnPromotion = '.pawnPromotion';
        selectors.promotionChoice = '.promotionChoice';
        ui.selectors = selectors;
    })(ui = constants.ui || (constants.ui = {}));
})(constants || (constants = {}));
