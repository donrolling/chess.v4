import { arrays } from "./arrays";

export class dom {
    public static removeClassName(element: any, removeItem: any): void {
        if (!element.className) {
            element.className = '';
        }
        if (!removeItem) {
            element.className = element.className;
        }
        var classNames = element.className.split(' ');
        arrays.removeItemFromArray(classNames, removeItem);
        element.className = classNames.join(' ');
    }

    public static addClassName(element: any, addItem: any): void {
        if (!element.className) {
            element.className = addItem;
        }
        element.className = `${element.className} ${addItem}`;
    }

    public static getSquareSelector(name: string): string {
        return constants.ui.selectors.square + name
    }

    public static getParameterByName(name: string): string {
        let url = window.location.href;
        name = name.replace(/[\[\]]/g, '\\$&');
        let regex = new RegExp('[?&]' + name + '(=([^&#]*)|&|#|$)'),
            results = regex.exec(url);
        if (!results) return '';
        if (!results[2]) return '';
        return decodeURIComponent(results[2].replace(/\+/g, ' '));
    }

    // public static getCurrentSquare(squareElement: HTMLElement): string {
    //     //logging.log(squareElement.target);
    //     let square = document.querySelector(squareElement.target);
    //     //logging.log(square);
    //     let piece = square.data(constants.ui.classes.piece);
    //     if (piece) {
    //         square = document.querySelector(squareElement.target).parentElement;
    //     }
    //     //logging.log(square);
    //     return square.data(constants.ui.classes.square);
    // }
}