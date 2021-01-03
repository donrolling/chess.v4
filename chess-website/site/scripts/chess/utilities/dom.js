define(["require", "exports", "../constants/ui/selectors", "./arrays"], function (require, exports, selectors_1, arrays_1) {
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
            return selectors_1.selectors.square + name;
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
