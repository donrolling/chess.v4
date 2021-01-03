"use strict";
var utilities;
(function (utilities) {
    class history {
        static historyToFEN(history) {
            return `${history.piecePlacement} ${history.activeColor} ${history.castlingAvailability} ${history.enPassantTargetPosition} ${history.halfmoveClock} ${history.fullmoveNumber}`;
        }
        static setHistoryPanel(pgn) {
            utilities.logging.info({ pgn: pgn });
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
                utilities.dom.removeClassName(element, constants.ui.classes.active);
            });
            utilities.dom.addClassName(target, constants.ui.classes.active);
        }
        static selectPgnItemByIndex(index) {
            let target = document.querySelector(`.items .item[data-index="${index}"]`);
            let items = document.querySelectorAll(constants.ui.selectors.item);
            items.forEach(element => {
                utilities.dom.removeClassName(element, constants.ui.classes.active);
            });
            utilities.dom.addClassName(target, constants.ui.classes.active);
        }
    }
    utilities.history = history;
})(utilities || (utilities = {}));
