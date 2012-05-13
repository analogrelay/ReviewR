/// <reference path="namespace.js" />
define(function () {
    "use strict";
    var exports = {};
    var _assert = false;

    exports.getFunctionParameterNames = function (fn) {
        /// <param name="fn" type="Function" />
        /// <returexports type="Array" />
        var fstr = fn.toString();
        return fstr.match(/\(.*?\)/)[0].replace(/[()]/gi, '').replace(/\s/gi, '').split(',');
    };

    exports.enableAsserts = function () {
        _assert = true;
    };

    exports.fail = function (msg) {
        if (_assert) {
            if (confirm('Assert failed: ' + msg + '\nPress OK to throw, Cancel to continue')) {
                throw 'Assert failed: ' + msg;
            }
        }
    };

    exports.assert = function (val, msg) {
        if (!val) {
            syrah.utils.fail(msg);
        }
    };

    exports.update = function (source, modified) {
        for (var key in modified) {
            if (modified.hasOwnProperty(key) && source.hasOwnProperty(key)) {
                var current = source[key];
                if (ko && ko.isWriteableObservable(current)) {
                    current(modified[key]);
                } else {
                    source[key] = modified[key];
                }
            }
        }
    };
    return exports;
});