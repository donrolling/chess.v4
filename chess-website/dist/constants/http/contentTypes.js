"use strict";
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
