"use strict";
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
