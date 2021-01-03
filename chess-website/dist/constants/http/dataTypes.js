"use strict";
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
