define(["require", "exports", "../constants/ui/attributes", "../constants/ui/classes", "../constants/ui/selectors", "./dom", "./logging"], function (require, exports, attributes_1, classes_1, selectors_1, dom_1, logging_1) {
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
                let itemContainer = document.querySelector(selectors_1.selectors.items);
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
                    let template = `<div class="${classes_1.classes.number}">${item}</div>`;
                    contentList.push(template);
                }
                else {
                    let template = i === pgnItems.length - 1
                        ? `<div class="${classes_1.classes.item} ${classes_1.classes.active}" ${attributes_1.attributes.dataIndex}="${pgnIndex}">${item}</div>`
                        : `<div class="${classes_1.classes.item}" ${attributes_1.attributes.dataIndex}="${pgnIndex}">${item}</div>`;
                    contentList.push(template);
                    pgnIndex++;
                }
            }
            // modify content
            let itemContainer = document.querySelector(selectors_1.selectors.items);
            if (itemContainer) {
                itemContainer.innerHTML = contentList.join('');
            }
        }
        static selectPgnItemByTarget(target) {
            let index = target.getAttribute(attributes_1.attributes.dataIndex);
            let items = document.querySelectorAll(selectors_1.selectors.item);
            items.forEach(element => {
                dom_1.dom.removeClassName(element, classes_1.classes.active);
            });
            dom_1.dom.addClassName(target, classes_1.classes.active);
        }
        static selectPgnItemByIndex(index) {
            let target = document.querySelector(`.items .item[data-index="${index}"]`);
            let items = document.querySelectorAll(selectors_1.selectors.item);
            items.forEach(element => {
                dom_1.dom.removeClassName(element, classes_1.classes.active);
            });
            dom_1.dom.addClassName(target, classes_1.classes.active);
        }
    }
    exports.historyEngine = historyEngine;
});
