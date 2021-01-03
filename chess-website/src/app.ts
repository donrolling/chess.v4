export class app {
    constructor() {
        console.log('App CTOR');

        let gameObjects: models.gameObjects = {
            board: null,
            gameState: null,
            pawnPromotionInfo: null,
            freeze: false,
            freezeNotify: 0
        };

        let config: models.config = {
            position: '',
            draggable: true
        };

        let gameService = new services.gameService();

        let gameStateService = new services.gameStateService(
            gameObjects,
            config,
            gameService
        );

        document
            .querySelector(constants.ui.selectors.fenSubmit)
            ?.addEventListener(constants.ui.events.click, () => gameService.getFenAndUpdate());

        document
            .querySelector(constants.ui.selectors.backBtn)
            ?.addEventListener(constants.ui.events.click, () => gameService.goBackOneMove(gameObjects));

        let promotionChoiceElement = document.querySelector(constants.ui.selectors.promotionChoice);
        if (promotionChoiceElement) {
            promotionChoiceElement.addEventListener(
                constants.ui.events.click,
                (e: Event) => { 
                    let element = e.target as HTMLElement;
                    let choice = element.getAttribute(constants.ui.attributes.dataPiece);
                    if (!choice) {
                        return;
                    }
                    let promotionPieceType = parseInt(choice);
                    if (!promotionPieceType || !gameObjects.pawnPromotionInfo || !gameObjects.gameState) {
                        return;
                    }
                    gameService.move(
                        gameObjects.gameState,
                        gameObjects.pawnPromotionInfo.source, 
                        gameObjects.pawnPromotionInfo.target, 
                        promotionPieceType
                    );
                    utilities.pawnPromotion.hidePawnPromotion(gameObjects);
                }
            );
        }

        // start using any existing fen source, preferring the querystring
        gameService.getAnyFenAndUpdate();
    }
}