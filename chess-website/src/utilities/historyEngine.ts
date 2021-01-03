import { attributes } from "../constants/ui/attributes";
import { classes } from "../constants/ui/classes";
import { selectors } from "../constants/ui/selectors";
import { dom } from "./dom";
import { logging } from "./logging";

export class historyEngine {
    public static historyToFEN(history: any): string {
        return `${history.piecePlacement} ${history.activeColor} ${history.castlingAvailability} ${history.enPassantTargetPosition} ${history.halfmoveClock} ${history.fullmoveNumber}`;
    }

    public static setHistoryPanel(pgn: string): void {
        logging.info({ pgn: pgn });
        if (!pgn) {// modify content
            let itemContainer = document.querySelector(selectors.items);
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
                let template = `<div class="${classes.number}">${item}</div>`;
                contentList.push(template);
            } else {
                let template = i === pgnItems.length - 1
                    ? `<div class="${classes.item} ${classes.active}" ${attributes.dataIndex}="${pgnIndex}">${item}</div>`
                    : `<div class="${classes.item}" ${attributes.dataIndex}="${pgnIndex}">${item}</div>`;
                contentList.push(template);
                pgnIndex++;
            }
        }

        // modify content
        let itemContainer = document.querySelector(selectors.items);
        if (itemContainer) {
            itemContainer.innerHTML = contentList.join('');
        }
    }

    public static selectPgnItemByTarget(target: any): void {
        let index = target.getAttribute(attributes.dataIndex);
        let items = document.querySelectorAll(selectors.item);
        items.forEach(element => {
            dom.removeClassName(element, classes.active);
        });
        dom.addClassName(target, classes.active);
    }

    public static selectPgnItemByIndex(index: number): void {
        let target = document.querySelector(`.items .item[data-index="${index}"]`)
        let items = document.querySelectorAll(selectors.item);
        items.forEach(element => {
            dom.removeClassName(element, classes.active);
        });
        dom.addClassName(target, classes.active);
    }
}