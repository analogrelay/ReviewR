/// <reference path="namespace.js" />
(function (undefined) {
    "use strict";
    namespace.define('syrah.utils', function (ns) {
        var _assert = false;

        ns.getFunctionParameterNames = function (fn) {
            /// <param name="fn" type="Function" />
            /// <returns type="Array" />
            var fstr = fn.toString();
            return fstr.match(/\(.*?\)/)[0].replace(/[()]/gi, '').replace(/\s/gi, '').split(',');
        };

        ns.enableAsserts = function () {
            _assert = true;
        };

        ns.fail = function (msg) {
            if (_assert) {
                if (confirm('Assert failed: ' + msg + '\nPress OK to throw, Cancel to continue')) {
                    throw 'Assert failed: ' + msg;
                }
            }
        };

        ns.assert = function (val, msg) {
            if (!val) {
                syrah.utils.fail(msg);
            }
        };

        ns.update = function (source, modified) {
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

        ns.getSetting = function (name) {
            var attrName = 'data-' + name;
            if (document.documentElement.hasAttribute(attrName)) {
                return document.documentElement.getAttribute(attrName);
            }
            return null;
        }
    });
})();