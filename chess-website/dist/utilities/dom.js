"use strict";
var utilities;
(function (utilities) {
    class dom {
        static removeClassName(element, removeItem) {
            if (!element.className) {
                element.className = '';
            }
            if (!removeItem) {
                element.className = element.className;
            }
            var classNames = element.className.split(' ');
            utilities.arrays.removeItemFromArray(classNames, removeItem);
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
    utilities.dom = dom;
})(utilities || (utilities = {}));
